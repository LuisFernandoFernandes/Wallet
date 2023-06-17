namespace Wallet.Tools.helpers
{
    public static class GuidHelpers
    {
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 30);
        }
    }
}
