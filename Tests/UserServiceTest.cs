using System;
using Tests.Helpers;

namespace Tests
{
    public class UserServiceTest : IDisposable
    {
        private UserServiceProvider _provider;

        public UserServiceTest()
        {
            _provider = new UserServiceProvider();
        }

        public void Dispose()
        {
            _provider.Dispose();
        }
    }
}