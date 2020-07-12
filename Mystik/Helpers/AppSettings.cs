using System;
namespace Mystik.Helpers
{
    public static class AppSettings
    {
        private static string _secretVariableName = "MYSTIK_SECRET";
        public static string Secret
        {
            get => Environment.GetEnvironmentVariable(_secretVariableName);
            set
            {
                Environment.SetEnvironmentVariable(_secretVariableName, value);
            }
        }

        public static string EncryptedMessagesPath
        {
            get => Environment.GetEnvironmentVariable("MYSTIK_ENCRYPTED_MESSAGES_PATH");
        }
    }
}
