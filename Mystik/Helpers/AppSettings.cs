using System;
namespace Mystik.Helpers
{
    public static class AppSettings
    {
        private static string _variableName = "MYSTIK_SECRET";
        public static string Secret
        {
            get => Environment.GetEnvironmentVariable(_variableName);
            set
            {
                Environment.SetEnvironmentVariable(_variableName, value);
            }
        }
    }
}
