namespace Spy.Extensions
{
    using System.Reflection;

    public static class PropertyInfoExtentions
    {
        public static bool IsIndexed(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Length > 0;
        }
    }
}