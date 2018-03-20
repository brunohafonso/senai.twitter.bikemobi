using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace senai.twitter.domain.Entities
{
    public class TokenConfigurations
    {
        public SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("c1f51f42-5727-4d15-b787-c6bbbb645024"));
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
    }
}