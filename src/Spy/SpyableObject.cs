namespace Spy
{
    using System;

    internal class SpyableObject
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

        internal string Name { get; private set; }
        internal string Description { get; private set; }
        internal object Object { get; private set; }
        internal Field[] Fields { get; private set; }
        internal Method[] Methods { get; private set; }
        internal Property[] Properties { get; private set; }

        internal class Field
        {
            internal object SpyableObject { get; set; }
            internal string Name { get; set; }
            internal Type Type { get; set; }

            public object GetValue()
            {
                var info = SpyableObject.GetType().GetField(Name, SpyEngine.Flags);
                if (info == null) { throw new Exception(String.Format("Unable to find field {0}", this)); }
                return info.GetValue(SpyableObject);
            }
        }

        internal class Method
        {
            internal object SpyableObject { get; set; }
            internal string Name { get; set; }
            internal Type Type { get; set; }

            public object GetValue()
            {
                var info = SpyableObject.GetType().GetMethod(Name, SpyEngine.Flags);
                if (info == null) { throw new Exception(String.Format("Unable to find method {0}", this)); }
                return info.Invoke(SpyableObject, null);
            }
        }

        internal class Property
        {
            internal object SpyableObject { get; set; }
            internal string Name { get; set; }
            internal Type Type { get; set; }

            internal object GetValue()
            {
                var info = SpyableObject.GetType().GetProperty(Name, SpyEngine.Flags);
                if (info == null) { throw new Exception(String.Format("Unable to find property {0}", this)); }
                return info.GetValue(SpyableObject, null);
            }
        }
    }
}