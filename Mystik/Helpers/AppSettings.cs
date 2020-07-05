using System;
namespace Mystik.Helpers
{
    public static class AppSettings
    {
        public static string Secret => Environment.GetEnvironmentVariable("MYSTIK_SECRET");
    }
}
