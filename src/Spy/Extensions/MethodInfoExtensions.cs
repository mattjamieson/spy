namespace Spy.Extensions
{
    using System.Reflection;

    public static class MethodInfoExtensions
    {
        public static bool HasNoArguments(this MethodInfo memberInfo)
        {
            return memberInfo.GetParameters().Length == 0;
        }
    }
}