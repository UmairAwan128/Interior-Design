using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InteriorDesign.Controllers
{
    public class InteriorController : Controller
    {
        [HttpPost]
        public ActionResult Create()
        {
            return View();
        }
    }
}