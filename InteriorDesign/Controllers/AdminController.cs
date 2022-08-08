using InteriorDesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using FireSharp.Config;  //configuration files for firesharp are here
using FireSharp.Interfaces;  //connection b/w c# and firebase managed here and builtin functions
using FireSharp.Response;  //manage redponse from firebase
using System.Threading.Tasks;


namespace InteriorDesign.Controllers
{
    public class AdminController : Controller
    {
        IFirebaseConfig FBaseconfig = new FirebaseConfig
        { //to identify our FireBase DB this will be passed to FireBaseClient

            //AuthSecret = "fSpctuCFfDUAfqYMOMtbRpohZMDqzBUm4UcH0x7h",
            //BasePath = "https://interiordesign-e61dc.firebaseio.com/"
            AuthSecret = "YslE0H9hFH1VFSEdvR7bWK1ZZqSVG1tzLbdA0IC9",
            BasePath = "https://interiordesign-6d720.firebaseio.com/"
        };

        IFirebaseClient fBaseClient;  //will open connection to FireBase

        // GET: Admin
        public AdminController() {
            fBaseClient = new FireSharp.FirebaseClient(FBaseconfig);
        }

        public async Task<ActionResult> Index()
        {
            UserSessionModel user = Session[WebUtil.CURRENT_USER] as UserSessionModel;
            if (!((user != null) && (user.Role == "Admin")))
                return RedirectToAction("Login", "Home");

            List<ProductModel> products = new List<ProductModel>();
            FirebaseResponse responseCounter = await fBaseClient.GetTaskAsync("Counter/node");
            CounterModel cm = responseCounter.ResultAs<CounterModel>();
            int totalPosts = Convert.ToInt32(cm.TotalPosts);

            for (int i = 1; i <= totalPosts; i++)
            {

                try
                {
                    FirebaseResponse responsePosts = await fBaseClient.GetTaskAsync("Posts/" + i);

                    if (responsePosts.Body != "null")
                    {
                        ProductModel pm = responsePosts.ResultAs<ProductModel>();
                        products.Add(pm);

                    }

                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(500, ex.Message);
                }

            }
            return View(products);
            
        }
    }
}