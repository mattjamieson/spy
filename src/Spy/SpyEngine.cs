namespace Spy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using Http;
    using TinyHttp;

    public class SpyEngine
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        private readonly ISpyableProvider[] _providers;
        private readonly TinyHttpHost _tinyHttpHost;
        public IEnumerable<SpyableObject> SpyableObjects { get; private set; }

        public SpyEngine(IEnumerable<ISpyableProvider> providers)
        {
            _providers = providers.ToArray();
            _tinyHttpHost = new TinyHttpHost("http://*:12345/", new SpyRequestProcessor(this));

            BuildSpyableObjects();
        }

        public void Start()
        {
            _tinyHttpHost.Start();
            while (true) ;
        }

        public object GetSpyableFieldValue(SpyableObject.Field field)
        {
            var fieldInfo = field.SpyableObject.GetType().GetField(field.Name, Flags);
            if (fieldInfo == null) { throw new Exception(String.Format("Unable to find field {0}", field)); }

            return fieldInfo.GetValue(field.SpyableObject);
        }

        public object GetSpyablePropertyValue(SpyableObject.Property property)
        {
            var propertyInfo = property.SpyableObject.GetType().GetProperty(property.Name, Flags);
            if (propertyInfo == null) { throw new Exception(String.Format("Unable to find property {0}", property)); }

            return propertyInfo.GetValue(property.SpyableObject, null);
        }

        private void BuildSpyableObjects()
        {
            SpyableObjects = _providers
                .Select(spyableProvider => new SpyableObject
                                               {
                                                   Name = spyableProvider.Name,
                                                   Description = spyableProvider.Description,
                                                   Object = spyableProvider.SpyableObject,
                                                   Fields = GetSpyableObjectFields(spyableProvider).ToArray(),
                                                   Properties = GetSpyableObjectProperties(spyableProvider).ToArray()
                                               })
                .ToArray();
        }

        private static IEnumerable<SpyableObject.Field> GetSpyableObjectFields(ISpyableProvider spyableProvider)
        {
            return spyableProvider.SpyableObject
                .GetType()
                .GetFields(Flags)
                .Where(f => !f.IsSpecialName && f.HasAttribute(typeof (SpyAttribute)))
                .Select(fieldInfo => new SpyableObject.Field
                                         {
                                             Name = fieldInfo.Name,
                                             Type = fieldInfo.FieldType,
                                             SpyableObject = spyableProvider.SpyableObject
                                         });
        }

        private static IEnumerable<SpyableObject.Property> GetSpyableObjectProperties(ISpyableProvider spyableProvider)
        {
            return spyableProvider.SpyableObject
                .GetType()
                .GetProperties(Flags)
                .Where(f => !f.IsSpecialName && !f.IsIndexed() && f.HasAttribute(typeof (SpyAttribute)))
                .Select(fieldInfo => new SpyableObject.Property
                                         {
                                             Name = fieldInfo.Name,
                                             Type = fieldInfo.PropertyType,
                                             SpyableObject = spyableProvider.SpyableObject
                                         });
        }
    }
}