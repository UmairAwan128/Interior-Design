using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FireSharp.Config;  //configuration files for firesharp are here
using FireSharp.Interfaces;  //connection b/w c# and firebase managed here and builtin functions
using FireSharp.Response;  //manage redponse from firebase
using System.Threading.Tasks;
using InteriorDesign.Models;
using Firebase.Storage;
using System.Net.Mail;

namespace InteriorDesign.Controllers
{
    public class SharedController : Controller
    {

        IFirebaseConfig FBaseconfig = new FirebaseConfig
        { //to identify our FireBase DB this will be passed to FireBaseClient

            //AuthSecret = "fSpctuCFfDUAfqYMOMtbRpohZMDqzBUm4UcH0x7h",
            //BasePath = "https://interiordesign-e61dc.firebaseio.com/"
            AuthSecret = "YslE0H9hFH1VFSEdvR7bWK1ZZqSVG1tzLbdA0IC9",
            BasePath = "https://interiordesign-6d720.firebaseio.com/"
        };

        IFirebaseClient fBaseClient;  //will open connection to FireBase

        public SharedController()
        {
            //to identify our FireBase DB FBconfig will be passed to FireBaseClient
            fBaseClient = new FireSharp.FirebaseClient(FBaseconfig);

        }


        public async Task<JsonResult> UploadImage()
        {
            JsonResult result = new JsonResult(); //first created JasonResult() obj as in this we will send our Data
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet; //set its behaviour i.e on Get request call this method or send Data..

            try
            {
                var file = Request.Files[0]; //so Request.Files[0] will give the first file of the FormData we sent
                long uno = DateTime.Now.Ticks;
                string totalPosts = null;
                var fileName = $"{uno}{file.FileName.Substring(file.FileName.LastIndexOf("."))}";
                if (!string.IsNullOrWhiteSpace(file.FileName))
                {
                    string url = $"/ImageUploads/{uno}{file.FileName.Substring(file.FileName.LastIndexOf("."))}";
                    string path = Request.MapPath(url);


                    //// Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
                    //var stream = File.Open(path, FileMode.Open);

                    //// Constructr FirebaseStorage, path to where you want to upload the file and Put it there
                    //var task = new FirebaseStorage("your-bucket.appspot.com")
                    //    .Child("data")
                    //    .Child("random")
                    //    .Child("file.png")
                    //    .PutAsync(stream);

                    //// Track progress of the upload
                    //task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

                    //// await the task to wait until upload completes and get the download url
                    //var downloadUrl = await task;
                    

                    file.SaveAs(path);
                    //Add here images to Images Table directly using imageServiceController
                    FirebaseResponse responseSave = await fBaseClient.GetTaskAsync("Counter/node");
                    CounterModel cm = responseSave.ResultAs<CounterModel>();
                    totalPosts = ( Convert.ToInt32(cm.TotalPosts) + 1 ).ToString();

                    }
                    
                result.Data = new { Success = true, ImageURL = string.Format("/ImageUploads/{0}", fileName), TotalPosts = totalPosts };
            }
            catch (Exception ex)
            {
                      result.Data = new { Success = false, Message = ex.Message };
            }

            return result;
        }


        public async Task<JsonResult> CheckEmail(string email)
        {
            var verificationCode = Guid.NewGuid(); //generating a unique ID for record

            JsonResult result = new JsonResult(); //first created JasonResult() obj as in this we will send our Data
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet; //set its behaviour i.e on Get request call this method or send Data..

            string output = await SendEmail(email, Convert.ToString(verificationCode));

            if (output == "Email Sent") // so valid
            {
                result.Data = new { Success = true, Message = "Email Sent" , VerificationCode = verificationCode }; 
            }
            else
            {
                result.Data = new { Success = false, Message = "Email not Sent" };
            }



            return result;
        }


        public Task<string> SendEmail(string email, string verificationCode)
        {

            return Task.Factory.StartNew(() =>
            {
                MailMessage mail = new MailMessage();

                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("idesign0042@gmail.com");  //Sender Email say my 

                mail.To.Add(email);    //sending this email to or reciever email
                mail.Subject = "Sign Up Successful to Interior Design";
                mail.Body = "\n\n Welcome to Interior Designs \n Now You are ready to upload your first Interior\n Your Verification Code is "+ verificationCode;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("idesign0042@gmail.com", "interior@12345");
                SmtpServer.EnableSsl = true;

                try
                {
                    SmtpServer.Send(mail);
                    return "Email Sent";
                }
                catch (Exception e)
                {
                    return "Email invalid : " + e.Message;
                }

            });

        }

    }
}