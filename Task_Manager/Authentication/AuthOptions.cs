using Microsoft.IdentityModel.Tokens;

namespace Task_Manager.Authentication
{
    public class AuthOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public byte[] Key { get; set; }
        public int TokenLifetime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Key);
        }

    }
}
