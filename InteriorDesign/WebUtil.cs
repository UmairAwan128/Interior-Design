using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteriorDesign
{
    public class WebUtil
    {
        public static readonly int ADMIN_ROLE;
        public static readonly string CURRENT_USER;
        static WebUtil()
        {
            CURRENT_USER = "CurrentUser";
            ADMIN_ROLE = 1;
         
        }
    }
}