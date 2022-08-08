using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteriorDesign.Models
{
    public class UserSessionModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }
        
        public string Email { get; set; }
        public string PhoneNo { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

    }
}