using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeunEvote.Models;
using System.IO;
using System.Web.Helpers;

namespace SSeunEvote.Controllers.Public
{
   
    
    public class SessionExpireFilterAttribute : ActionFilterAttribute { 
    
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["VotersId"] == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }

    public class HomeController : Controller
    {
        // GET: Home
        private IVotingModelEntities db = new IVotingModelEntities();
      
        public ActionResult Index(string report,string user)
        {
            if (TimeVerifier() > 0)
            {
                return RedirectToAction("ElectionResult", new { id = 2 });
            }
            else if((user=="logger") ||(user=="admin2enroll") ||(user=="admin"))
            {
                Session["job"] = user;
                ViewBag.Report = report;
                return View();
            }
           
            else if (Session["VoterId"] != null)
            {
                Voted Vot = db.Voteds.Find(Session["VoterId"]);
                if (Vot.HasVoted == true)
                {
                    ViewBag.Report = "You have voted!";
                    return View();
                }
                else
                {
                   return RedirectToAction("NotRegistered");
                }
            }
            else 
            {
                ViewBag.Report = report;
                return View();
                //return RedirectToAction("NotRegistered");
            }
           
        }
        [HttpGet]
        public ActionResult NotRegistered()
        {
            return View();
        }
        public ActionResult HomePage()
        {
            return View();
        }
        public ActionResult Admineer()
        {
            return View();
        }
        [NonAction]
        [System.Web.Services.WebMethod]
        public DateTime CurrentDateTimeGenerator()
        {
            ViewBag.CurrentTime = DateTime.Now;
            return DateTime.Now;
        }
       
        public ActionResult Mychart(int id)
        {
            List<string> dt = new List<string>();
            List<int> dl = new List<int>();
            var Ele = db.Elections.Find(id);
            var con = db.Contestants.Where(m => m.ElectionId.Equals(id));
     
                foreach (var ite in con)
                {
                    dt.Add(ite.Nickname);
                    dl.Add(ite.Count);
                }

                string[] Posi = dt.ToArray();
                int[] Pol = dl.ToArray();
                new Chart(width: 500, height: 200).AddSeries
                (chartType: "column", xValue: Posi, yValues: Pol).Write
                ("png").AddTitle(Ele.Post).SetXAxis("Contestants").SetYAxis("No of Votes");
           


            return null;
        }
        [HttpGet]
        public ActionResult ElectionResult(int id)
        {
            if (id == 2)
            {
                ViewBag.Elected = db.Elections.ToList();
             
                ViewBag.Contestants = db.Contestants.ToList();
                return View();
            }
            return RedirectToAction("index", new { report = "Welcome!" });
        }
        [NonAction]
        public int TimeVerifier()
        {
            if (db.ElectionDetails.FirstOrDefault() == null)
            {
                return 0;
            }
            else
            {
                ElectionDetail setter = db.ElectionDetails.First();
                DateTime sett = setter.ElectionStopTime;
                int resp = DateTime.Compare(CurrentDateTimeGenerator(), sett);
                return resp;
            }
           
        }
        public string FolderCreator(string surname)
        {
            System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/voting/" + surname));
            string destination = string.Format("~/Content/voting/{0}/", surname);
            return destination;
        }
        [HttpPost]
     
        public ActionResult Login([Bind(Include = "Id,Surname,Password,BVN,PHONE_NUMBER")] Voted voted)
        {

            if (ModelState.IsValid)
            {


                var Voter = db.Voteds.Where(a => a.Password.Equals(voted.Password) 
                && a.Surname.Equals(voted.Surname) 
                && a.PHONE_NUMBER.Equals(voted.PHONE_NUMBER)
                && a.BVN.Equals(voted.BVN)).FirstOrDefault();
                var Admine = db.Adminees.Where(b => b.Surname.Equals(voted.Surname) && b.Password.Equals(voted.Password)).FirstOrDefault();
                if (Voter != null)
                {
                    if (Voter.HasVoted == true)
                    {
                        ViewBag.port = "You have already voted";
                        return RedirectToAction("Index", new { report = ViewBag.port });
                    }
                    else if (TimeVerifier() > 0)
                    {
                        return RedirectToAction("ElectionResult", new { id = 2 });
                    }
                    else if (DateTime.Now < db.ElectionDetails.First().ElectionStartTime)
                    {
                        ViewBag.port = "Time not set for election!";
                        return RedirectToAction("Index", new { report = ViewBag.port });
                    }
                    //else if (Voter.Encoded != Session["UserEncoded"].ToString())
                    //{
                    //    ViewBag.port = "";
                    //    return RedirectToAction("Index", new { report = ViewBag.port });
                    //}
                    else
                    {       /*;*/

                        Session["VotersId"] = Voter.Id;
                        Session["VotersName"] = Voter.Surname.ToString();
                        Session["Role"] = "Voter";

                        return RedirectToAction("VoteNow", new { VotersId = Voter.Id });
                    }


                }
                
                else
                {
                    ViewBag.port = "Invalid Login Details ";
                    return RedirectToAction("Index", new { report = ViewBag.port });
                }

            }
            return RedirectToAction("Ind");
        }
        public void Print()
        {

           
        }
        public ActionResult AdminLogger([Bind(Include = "Id,Surname,Password")]Adminee Admin)
        {

            if (ModelState.IsValid)
            {
                var Admine = db.Adminees.Where(b => b.Surname.Equals(Admin.Surname) && b.Password.Equals(Admin.Password)).FirstOrDefault();
               if (Admine != null)
                {
                    if ((Session["ToBeEnrolled"] != null) && (Session["job"].ToString() == "admin2enroll"))
                    {
                        Session["AdminId"] = Admine.Id.ToString();
                        Session["AdminName"] = Admine.Surname.ToString();
                        Session["Role"] = "Administrator";

                        return RedirectToAction("create", "VotersDets", new { enrollID = Session["ToBeEnrolled"] });

                    }
                    Session["AdminId"] = Admine.Id.ToString();
                    Session["AdminName"] = Admine.Surname.ToString();
                    Session["Role"] = "Administrator";
                    return RedirectToAction("Index", "elections", new { id = Admine.Id });

                }
                else
                {
                    ViewBag.port = "Invalid Login Details Mr Admin";
                    return RedirectToAction("Index", new { report = ViewBag.port });
                }

            }
            return RedirectToAction("Ind");
        }

        [HttpGet]
        [SessionExpireFilter]
        public ActionResult VoteNow(int ? id)
        {
            if (id == null)
            {
                List<int> Result= new List<int>();
                TempData["Report"] = Result;
                ViewBag.Electi = db.Elections.FirstOrDefault();
                var changer=db.Elections.Where(m => m.Done==true || m.Done==null);
                foreach (var item in changer)
                {
                    item.Done = false;
                }
                db.SaveChanges();
                return View();
            }
           
            return View();

        }
        [HttpGet]
        [SessionExpireFilter]
        public ActionResult Voted()
        {
            List<object> None = new List<object>();
            ViewBag.Reporter = TempData["Report"];
            foreach (var item in ViewBag.Reporter)
            {
                Contestant SelectedCon = db.Contestants.Find(item);
                None.Add(SelectedCon);
            }
            ViewBag.Reportee = None;
            TempData["Reported"] = ViewBag.Reporter;
            return View();
        }
        [HttpPost]
        [SessionExpireFilter]
        public ActionResult Submission()
        {
            ViewBag.Reported = TempData["Reported"];
        
            foreach (var item in ViewBag.Reported)
            {
                Contestant SelectedCon = db.Contestants.Find((int)item);
                SelectedCon.Count++;
              
            }
            int Dealer = Convert.ToInt16(Session["VotersId"]);
            Voted votee= db.Voteds.Find(Dealer);
            votee.HasVoted= true;
            db.SaveChanges();
            return RedirectToAction("Index", new { report = "You have successfully voted"});
        }

        [HttpGet]
        [SessionExpireFilter]
        public ActionResult Voting(int id)
        {

            if (db.Elections.All(m => m.Done == true))
            {
                return RedirectToAction("Voted");
            }
            else
            {
                Election raty2 = db.Elections.Where(m => m.Done == false).FirstOrDefault();
                ViewBag.Electi = raty2;
                return View();
            }
       
        }
        [SessionExpireFilter]
        [HttpPost]
   
        [ValidateAntiForgeryToken]
        public ActionResult save(FormCollection form,int DoneId)
        {
            
            Election raty = db.Elections.Find(DoneId);
            int ConSelected = Convert.ToInt32(form[raty.Post]);
            Contestant SelectedCon = db.Contestants.Find(ConSelected);
            List<int> lst = (List<int>)TempData["Report"];
            lst.Add(ConSelected);
            //db.Contestants.Where(m => m.Id.Equals(ConSelected)).FirstOrDefault().Count++;
            raty.Done = true;
            db.SaveChanges();
            return RedirectToAction("Voting", new {id= DoneId });
           
        }
    }
}