namespace Spy.Tests
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NUnit.Framework;

    public class SpyEngineTest
    {
        [Test]
        public void Test()
        {
            var spyable = new FakeSpyable();
            var provider = new FakeProvider {SpyableObject = spyable};

            var spy = new SpyEngine(new[] {provider});

            Console.WriteLine(spy.GetSpyableFieldValue(spy.SpyableObjects.First().Fields.First()));
        }

        class FakeSpyable
        {
            private readonly int _random;

            public FakeSpyable()
            {
                _random = new Random().Next();
            }
        }

        class FakeProvider : ISpyableProvider
        {
            public string Name
            {
                get { return "Fake Provider"; }
            }

            public string Description
            {
                get { return "Fake Provider"; }
            }

            public object SpyableObject { get; set; }
        }
    }
}