namespace Spy
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public class SpyAttribute : Attribute
    {}
}