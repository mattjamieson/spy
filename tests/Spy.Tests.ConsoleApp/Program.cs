namespace Spy.Tests.ConsoleApp
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ServiceStack.Text;

    class Program
    {
        static void Main(string[] args)
        {
            new SpyEngine("http://*:12345/",
                          o => o.ToJson(),
                          new List<ISpyableProvider> {new FakeProvider {SpyableObject = new FakeSpyable()}})
                .Start();
        }
    }

    class FakeSpyable
    {
        [Spy] private int _randomValue;
        private IDictionary<string, IDictionary<string, object>> _cache;

        public FakeSpyable()
        {
            var random = new Random();
            _randomValue = random.Next();

            _cache = new Dictionary<string, IDictionary<string, object>>();
            _cache["XS1234567890"] = new Dictionary<string, object> {{"ISIN", "XS1234567890"}};

            Task.Factory.StartNew(() =>
                                      {
                                          while (true)
                                          {
                                              Thread.Sleep(500);
                                              _randomValue = random.Next();
                                          }
                                      });
        }
    }

    class FakeProvider : ISpyableProvider
    {
        public string Name
        {
            get { return "fake"; }
        }

        public string Description
        {
            get { return "Fake Provider"; }
        }

        public object SpyableObject { get; set; }
    }
}