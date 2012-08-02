namespace Spy.Extensions
{
    using System;
    using System.Reflection;

    public static class MemberInfoExtensions
    {
        public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType, bool inherit = true)
        {
            if (!attributeType.IsAttributeType()) { return false; }
            return memberInfo.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        public static bool IsAttributeType(this Type type)
        {
            return typeof (Attribute).IsAssignableFrom(type);
        }
    }
}