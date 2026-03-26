namespace EBCJobPortalAdmin.Security
{
    public static class PawwordEncryption
    {
        public static string EncryptPasswordBase64Strig(string password)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string DecryptPasswordBase64String(string encodedString)
        {
            var base64Encoded = System.Convert.FromBase64String(encodedString);
            return System.Text.Encoding.UTF8.GetString(base64Encoded);
        }
    }
}
