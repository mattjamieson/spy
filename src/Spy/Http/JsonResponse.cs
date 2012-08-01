namespace Spy.Http
{
    using System;
    using System.IO;
    using TinyHttp;

    public class JsonResponse<T> : Response
    {
        public JsonResponse(T obj, Action<object, Stream> serializer)
        {
            ContentType = "application/json";
            StatusCode = System.Net.HttpStatusCode.OK;
            Body = stream => serializer.Invoke(obj, stream);
        }
    }

    public class JsonResponse : JsonResponse<object>
    {
        public JsonResponse(object obj, Action<object, Stream> serializer) : base(obj, serializer)
        {}
    }
}