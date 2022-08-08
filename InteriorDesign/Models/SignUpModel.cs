using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteriorDesign.Models
{
    public class SignUpModel
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Password { get; set; }
        public string role { get; set; }
        public string ValidationCode { get; set; }
        public string ValidationCodeGen { get; set; }
    }
}