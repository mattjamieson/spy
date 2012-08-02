namespace Spy.Http
{
    using System.IO;
    using System.Linq;
    using TinyHttp;

    public class SpyRequestProcessor : RequestProcessor
    {
        private readonly SpyEngine _spyEngine;

        public SpyRequestProcessor(SpyEngine spyEngine)
        {
            _spyEngine = spyEngine;

            Get["/spy/{spyable}/field/{field}"] = p => GetSpyableFieldValue(p.spyable, p.field);
            Get["/spy/{spyable}/method/{method}"] = p => GetSpyableMethodValue(p.spyable, p.method);
            Get["/spy/{spyable}/property/{property}"] = p => GetSpyablePropertyValue(p.spyable, p.property);
            Get["/spy/{spyable}"] = p => GetSpyableObject(p.spyable);
            Get["/spy"] = _ => GetSpyableObjects();
        }

        private Response GetSpyableObjects()
        {
            return new JsonResponse(_spyEngine.SpyableObjects, ToJson);
        }

        private Response GetSpyableObject(string spyableName)
        {
            var spyable = _spyEngine.SpyableObjects.FirstOrDefault(o => o.Name == spyableName);
            return spyable != null ? (Response) new JsonResponse(spyable, ToJson) : new NotFoundResponse();
        }

        private Response GetSpyableFieldValue(string spyableName, string fieldName)
        {
            var spyable = _spyEngine.SpyableObjects.FirstOrDefault(o => o.Name == spyableName);
            if (spyable != null)
            {
                var field = spyable.Fields.FirstOrDefault(f => f.Name == fieldName);
                if (field != null)
                {
                    var result = field.GetValue();
                    return new JsonResponse(result, ToJson);
                }
            }

            return new NotFoundResponse();
        }

        private Response GetSpyableMethodValue(string spyableName, string methodName)
        {
            var spyable = _spyEngine.SpyableObjects.FirstOrDefault(o => o.Name == spyableName);
            if (spyable != null)
            {
                var method = spyable.Methods.FirstOrDefault(m => m.Name == methodName);
                if (method != null)
                {
                    var result = method.GetValue();
                    return new JsonResponse(result, ToJson);
                }
            }

            return new NotFoundResponse();
        }

        private Response GetSpyablePropertyValue(string spyableName, string propertyName)
        {
            var spyable = _spyEngine.SpyableObjects.FirstOrDefault(o => o.Name == spyableName);
            if (spyable != null)
            {
                var property = spyable.Properties.FirstOrDefault(p => p.Name == propertyName);
                if (property != null)
                {
                    var result = property.GetValue();
                    return new JsonResponse(result, ToJson);
                }
            }

            return new NotFoundResponse();
        }

        private void ToJson<T>(T obj, Stream stream)
        {
            using (var writer = new StreamWriter(stream) { AutoFlush = true })
            {
                writer.Write(_spyEngine.JsonSerializer.Invoke(obj));
            }
        }
    }
}