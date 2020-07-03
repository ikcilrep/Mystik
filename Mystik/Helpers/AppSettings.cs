using System;
namespace Mystik.Helpers
{
    public class AppSettings
    {
        public string Secret => Environment.GetEnvironmentVariable("MYSTIK_SECRET");
    }
}
