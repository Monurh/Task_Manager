using System.Security.Cryptography;

namespace Task_Manager.Authentication
{
    public class AuthOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; } // Изменили тип ключа на string
        public int TokenLifetime { get; set; }

        // Генерация ключа размером 256 бит и его преобразование в строку Base64
        public static string GenerateBase64Key()
        {
            // Генерация ключа размером 256 бит (32 байта)
            byte[] keyBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }

            // Преобразование ключа в строку Base64
            string base64Key = Convert.ToBase64String(keyBytes);
            return base64Key;
        }
    }
}
