namespace Spy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Extensions;
    using Http;
    using TinyHttp;

    public class SpyEngine
    {
        internal const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        private readonly ISpyableProvider[] _providers;
        private readonly TinyHttpHost _tinyHttpHost;
        private readonly EventWaitHandle _eventWaitHandle;
        
        public SpyEngine(string baseUri, Func<object, string> jsonSerializer, IEnumerable<ISpyableProvider> providers)
        {
            _providers = providers.ToArray();
            _tinyHttpHost = new TinyHttpHost(baseUri, new SpyRequestProcessor(this));
            _eventWaitHandle = new AutoResetEvent(false);

            JsonSerializer = jsonSerializer;

            BuildSpyableObjects();
        }

        internal Func<object, string> JsonSerializer { get; private set; }
        internal IEnumerable<SpyableObject> SpyableObjects { get; private set; }

        public void Start()
        {
            _tinyHttpHost.Start();
            _eventWaitHandle.WaitOne();
        }

        public void Stop()
        {
            _eventWaitHandle.Set();
        }
        
        private void BuildSpyableObjects()
        {
            SpyableObjects = _providers
                .Select(spyableProvider => new SpyableObject(spyableProvider.Name,
                                                             spyableProvider.Description,
                                                             spyableProvider.SpyableObject,
                                                             GetSpyableObjectFields(spyableProvider).ToArray(),
                                                             GetSpyableObjectMethods(spyableProvider).ToArray(),
                                                             GetSpyableObjectProperties(spyableProvider).ToArray()))
                .ToArray();
        }

        private static IEnumerable<SpyableObject.Field> GetSpyableObjectFields(ISpyableProvider spyableProvider)
        {
            return spyableProvider.SpyableObject
                .GetType()
                .GetFields(Flags)
                .Where(f => !f.IsSpecialName && f.HasAttribute(typeof (SpyAttribute)))
                .Select(info => new SpyableObject.Field
                                    {
                                        Name = info.Name,
                                        Type = info.FieldType,
                                        SpyableObject = spyableProvider.SpyableObject
                                    });
        }

        private static IEnumerable<SpyableObject.Method> GetSpyableObjectMethods(ISpyableProvider spyableProvider)
        {
            return spyableProvider.SpyableObject
                .GetType()
                .GetMethods(Flags)
                .Where(f => !f.IsSpecialName && f.HasAttribute(typeof (SpyAttribute)) && f.HasNoArguments())
                .Select(info => new SpyableObject.Method
                                    {
                                        Name = info.Name,
                                        Type = info.ReturnType,
                                        SpyableObject = spyableProvider.SpyableObject
                                    });
        }

        private static IEnumerable<SpyableObject.Property> GetSpyableObjectProperties(ISpyableProvider spyableProvider)
        {
            return spyableProvider.SpyableObject
                .GetType()
                .GetProperties(Flags)
                .Where(f => !f.IsSpecialName && !f.IsIndexed() && f.HasAttribute(typeof (SpyAttribute)))
                .Select(info => new SpyableObject.Property
                                    {
                                        Name = info.Name,
                                        Type = info.PropertyType,
                                        SpyableObject = spyableProvider.SpyableObject
                                    });
        }
    }
}