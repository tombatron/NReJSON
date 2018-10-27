using System;
using StackExchange.Redis;

namespace NReJSON.IntegrationTests
{
    public abstract class BaseIntegrationTest : IDisposable
    {
        private readonly ConnectionMultiplexer _muxer;
        protected readonly IDatabase _db;

        protected BaseIntegrationTest()
        {
            _muxer = ConnectionMultiplexer.Connect("127.0.0.1");
            _db = _muxer.GetDatabase(1);
        }

        public void Dispose()
        {
            _muxer.Dispose();
        }
    }
}
