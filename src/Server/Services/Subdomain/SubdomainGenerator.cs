using System.Security.Cryptography;

namespace Server.Services.Subdomain
{
    public static class SubdomainGenerator
    {
        // Without 'l', '1', 'o', '0' for better URL readability
        private static readonly char[] UrlFriendlyChars = "abcdefghijkmnpqrstuvwxyz23456789".ToCharArray();

        public static string Generate()
        {
            return string.Create(8, UrlFriendlyChars, (buffer, alphabet) =>
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
                }
            });
        }
    }
}
