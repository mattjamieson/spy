namespace Spy.Http
{
    using System.Linq;
    using TinyHttp;
    
    public class SpyRequestProcessor : RequestProcessor
    {
        private readonly SpyEngine _spyEngine;
        
        public SpyRequestProcessor(SpyEngine spyEngine)
        {
            _spyEngine = spyEngine;
        
            Get["/"] = _ => new FileResponse(@"D:\Users\Matt\Documents\Development\Spy\Source\Spy\Http\html\index.html");
            Get["/js/jsonreport.js"] = _ => new FileResponse(@"D:\Users\Matt\Documents\Development\Spy\Source\Spy\Http\js\jsonreport.js");
            Get["/spy/{spyable}/field/{field}"] = p => GetSpyableFieldValue(p.spyable, p.field);
            Get["/spy/{spyable}"] = p => GetSpyableObject(p.spyable);
            Get["/spy/"] = _ => GetSpyableObjects();
        }

        private Response GetSpyableObjects()
        {
            return new JsonResponse(_spyEngine.SpyableObjects);
        }

        private Response GetSpyableObject(string spyableName)
        {
            var spyable = _spyEngine.SpyableObjects.FirstOrDefault(o => o.Name == spyableName);
            if (spyable != null) { return new JsonResponse(spyable); }
            return new NotFoundResponse();
        }

        private Response GetSpyableFieldValue(string spyableName, string fieldName)
        {
            var spyable = _spyEngine.SpyableObjects.FirstOrDefault(o => o.Name == spyableName);
            if (spyable != null)
            {
                var field = spyable.Fields.FirstOrDefault(f => f.Name == fieldName);
                if (field != null)
                {
                    var result = _spyEngine.GetSpyableFieldValue(field);
                    return new JsonResponse(result);
                }
            }

            return new NotFoundResponse();
        }
    }
}