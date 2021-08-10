namespace FlakeyBit.DigestAuthentication.AspNetCore
{
    public static class DigestAuthentication
    {
        public const string DefaultScheme = "Digest";

        public static string GetScheme(string scheme)
        {
            if (scheme is null)
                scheme = DefaultScheme;
            return scheme;
        }
    }
}
