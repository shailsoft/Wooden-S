using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoodenWeb_S.Models
{
    public class JWTTokenResponse
    {
        public string Token{ get; set; }
        public string Expiration { get; set; }
    }
}
