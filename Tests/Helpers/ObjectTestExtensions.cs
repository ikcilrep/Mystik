namespace Tests.Helpers
{
    public static class ObjectTestExtensions
    {
        public static object GetProperty(this object o, string name)
        {
            return o?.GetType().GetProperty(name)?.GetValue(o, null);
        }
    }
}