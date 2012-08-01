namespace Spy
{
    using System;

    public class SpyableObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object Object { get; set; }
        public Field[] Fields { get; set; }
        public Property[] Properties { get; set; }

        public class Field
        {
            public object SpyableObject { get; set; }
            public string Name { get; set; }
            public Type Type { get; set; }
        }

        public class Property
        {
            public object SpyableObject { get; set; }
            public string Name { get; set; }
            public Type Type { get; set; }
        }
    }
}