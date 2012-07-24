namespace Spy.Http
{
    using System.IO;
    using Json;
    using TinyHttp;

    public class JsonResponse<T> : Response
    {
        public JsonResponse(T obj)
        {
            ContentType = "application/json";
            StatusCode = System.Net.HttpStatusCode.OK;
            Body = stream => SerializeObject(obj, stream);
        }

        private static void SerializeObject(T obj, Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                var serializer = new JavaScriptSerializer(null, false, JsonSettings.MaxJsonLength, JsonSettings.MaxRecursions);
                serializer.RegisterConverters(JsonSettings.Converters);
                serializer.Serialize(obj, writer);
            }
        }
    }

    public class JsonResponse : JsonResponse<object>
    {
        public JsonResponse(object obj) : base(obj)
        {}
    }
}