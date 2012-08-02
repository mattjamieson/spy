namespace Spy
{
    using System;

    public class SpyableObject
    {
        public SpyableObject(string name, string description, object o, Field[] fields, Method[] methods, Property[] properties)
        {
            Name = name;
            Description = description;
            Object = o;
            Fields = fields;
            Methods = methods;
            Properties = properties;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public object Object { get; private set; }
        public Field[] Fields { get; private set; }
        public Method[] Methods { get; private set; }
        public Property[] Properties { get; private set; }

        public class Field
        {
            public object SpyableObject { get; set; }
            public string Name { get; set; }
            public Type Type { get; set; }

            public object GetValue()
            {
                var info = SpyableObject.GetType().GetField(Name, SpyEngine.Flags);
                if (info == null) { throw new Exception(String.Format("Unable to find field {0}", this)); }
                return info.GetValue(SpyableObject);
            }
        }

        public class Method
        {
            public object SpyableObject { get; set; }
            public string Name { get; set; }
            public Type Type { get; set; }

            public object GetValue()
            {
                var info = SpyableObject.GetType().GetMethod(Name, SpyEngine.Flags);
                if (info == null) { throw new Exception(String.Format("Unable to find method {0}", this)); }
                return info.Invoke(SpyableObject, null);
            }
        }

        public class Property
        {
            public object SpyableObject { get; set; }
            public string Name { get; set; }
            public Type Type { get; set; }

            public object GetValue()
            {
                var info = SpyableObject.GetType().GetProperty(Name, SpyEngine.Flags);
                if (info == null) { throw new Exception(String.Format("Unable to find property {0}", this)); }
                return info.GetValue(SpyableObject, null);
            }
        }
    }
}