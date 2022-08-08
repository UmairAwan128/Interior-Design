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
using System.Net.Mail;

namespace InteriorDesign.Controllers
{
    public class HomeController : Controller
    {
        IFirebaseConfig FBaseconfig = new FirebaseConfig
        { //to identify our FireBase DB this will be passed to FireBaseClient

            //AuthSecret = "fSpctuCFfDUAfqYMOMtbRpohZMDqzBUm4UcH0x7h",
            //BasePath= "https://interiordesign-e61dc.firebaseio.com/"
            
            AuthSecret = "YslE0H9hFH1VFSEdvR7bWK1ZZqSVG1tzLbdA0IC9",
            BasePath = "https://interiordesign-6d720.firebaseio.com/"
            
        };

        IFirebaseClient fBaseClient;  //will open connection to FireBase

        public HomeController() {
            //to identify our FireBase DB FBconfig will be passed to FireBaseClient
            fBaseClient = new FireSharp.FirebaseClient(FBaseconfig);
            
        }

        // GET: Home
        public async Task<ActionResult> Index()
        {
            UserSessionModel currentUser = new UserSessionModel();
            currentUser = (UserSessionModel)Session[WebUtil.CURRENT_USER];

            List<ProductModel> allProducts = new List<ProductModel>();

            List<ProductModel> userSpecificProducts = new List<ProductModel>();

            FirebaseResponse responseCounter = await fBaseClient.GetTaskAsync("Counter/node");
            CounterModel cm = responseCounter.ResultAs<CounterModel>();
            int totalPosts = Convert.ToInt32(cm.TotalPosts);

            for (int i = 1; i <= totalPosts; i++) //write       i <= totalPosts
            {

                try
                {
                    FirebaseResponse responsePosts = await fBaseClient.GetTaskAsync("Posts/" + i);

                    if (responsePosts.Body != "null")
                    {
                        ProductModel pm = responsePosts.ResultAs<ProductModel>();

                        if (currentUser != null && pm.phoneNo == currentUser.PhoneNo)
                        {
                            userSpecificProducts.Add(pm);
                        }
                        allProducts.Add(pm);
                    }

                }
                catch (Exception ex)
                {
                    return new HttpStatusCodeResult(500, ex.Message);
                }

            }

            List<BannersModel> banners = new List<BannersModel>();

            for (int j = 1; j <= 3; j++)
            {

                FirebaseResponse responsePosts = await fBaseClient.GetTaskAsync("Banners/" + j);

                BannersModel banner = responsePosts.ResultAs<BannersModel>();

                banners.Add(banner);

            }

            ViewBag.userSpecificProducts = userSpecificProducts;
            ViewBag.allBanners = banners;
            return View(allProducts);

        }
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SignUp(SignUpModel model)
        {

            if (ModelState.IsValid )
            {
                if(model.ValidationCode == model.ValidationCodeGen)
                {
                    model.ID = Guid.NewGuid() + "_" + model.FirstName; //generating a unique ID for record

                    //checking if the PhoneNo already exists
                    SignUpModel found = new SignUpModel();
                    FirebaseResponse response = await fBaseClient.GetTaskAsync("Users/" + model.PhoneNo);

                    if (response.Body == "null")
                    {
                        //model.role = "Admin";
                        model.role = "User";
                        var name = model.FirstName;
                        //save data in firebase in User tree if not exist then first create it
                        SetResponse responseSave = await fBaseClient.SetTaskAsync("Users/" + model.PhoneNo, model);
                        SignUpModel result = responseSave.ResultAs<SignUpModel>();

                        TempData.Add("AlertMessage", new AlertModel($"You have Successully SignedUp", AlertModel.AlertType.Information));
                        return RedirectToAction("Login");
                    }
                    if (response.Body != "null")
                    {
                        TempData.Add("AlertMessage", new AlertModel($"Phone No should be Unique", AlertModel.AlertType.Error));
                        return RedirectToAction("SignUp");
                    }

                }
                else{
                    TempData.Add("AlertMessage", new AlertModel($"The Verification code you entered is not valid", AlertModel.AlertType.Error));
                    return RedirectToAction("SignUp");
                }
            }
            else
            {
                return new HttpStatusCodeResult(500);
            }
            return RedirectToAction("Login");
        }



        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                SignUpModel result = new SignUpModel();
                //to retrieve data from fireBase first Name of tree then + name of branch e.g
                //Users   //tree
                // --umairawan@gmail.com  //branch
                //   --Name               //leaves
                //   --Password

                FirebaseResponse response = await fBaseClient.GetTaskAsync("Users/"+model.PhoneNo);
                result = response.ResultAs<SignUpModel>();
                UserSessionModel currentUser = new UserSessionModel();

                if (model.Password == result.Password && model.PhoneNo == result.PhoneNo)
                {
                    if (result!=null) {
                        currentUser.FirstName = result.FirstName;
                        currentUser.Password = result.Password;
                        currentUser.Role = result.role;
                        currentUser.Email = result.Email;
                        currentUser.Id = result.ID;
                        currentUser.PhoneNo = result.PhoneNo;

                        Session.Add(WebUtil.CURRENT_USER, currentUser);
                        return RedirectToAction("Index");
                    }
                }
                else {
                    return RedirectToAction("Login");
                }
               
            }

            return RedirectToAction("Login");

        }


        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index");
        }

    }
}