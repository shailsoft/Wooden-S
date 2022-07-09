using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoodenAPI_S.Models
{
    public class UserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
    }
}
