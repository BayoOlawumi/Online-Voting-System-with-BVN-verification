using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SeunEvote.Models;
using System.Text;


namespace SeunEvote.Controllers
{
    public class ElectionsController : Controller
    {

      //  public void ()
        //{
        //
        //}
        //Results
        //[SessionExpireFilter]
        public ActionResult Result(int ? id)
        {
            if (id == 2)
            {
                ViewBag.Message = "Result has been declared";
            }
            else
            {
                ViewBag.Message = "Time not set to declare Result";
            }
            ViewBag.Elected = db.Elections.ToList();
            ViewBag.Contestants = db.Contestants.ToList();
            return View();
        }
        public DateTime CurrentDateTimeGenerator()
        {
            ViewBag.CurrentTime = DateTime.Now;
            return DateTime.Now;
        }

        public int TimeVerifier()
        {

            ElectionDetail setter = db.ElectionDetails.First();
            DateTime sett = setter.ElectionStopTime;
            int resp = DateTime.Compare(CurrentDateTimeGenerator(), sett);
            return resp;

        }
        [HttpGet]
        [SessionExpireFilter]
        public ActionResult Declare()
        {
            if ((TimeVerifier() == 0) || (TimeVerifier() > 0))
            {  
                return RedirectToAction("Result", new { id = 2 });
            }
            else
            {
                return RedirectToAction("Result");
            }

           
        }





        private IVotingModelEntities db = new IVotingModelEntities();

        // GET: Elections
        [SessionExpireFilter]
        public ActionResult Index(int? id,string report)
        {
            ViewBag.Reporter = report;
            return View(db.Elections.ToList());

        }

        [SessionExpireFilter]
       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Election election = db.Elections.Find(id);
            if (election == null)
            {
                return HttpNotFound();
            }
            return View(election);
        }
      
        
        // GET: Elections/Create
        [SessionExpireFilter]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Elections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Post,NoOfContestants")] Election election)
        {

            if (ModelState.IsValid)
            {
                
                string EditedPost = new string(election.Post.Where(Char.IsLetter).ToArray());
                
                //db.UpdateElections(EditedPost);
                election.Post = EditedPost;
                db.Elections.Add(election);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(election);
        }

        

       
        // GET: Elections/Delete/5
        [SessionExpireFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Election election = db.Elections.Find(id);
            var cona = db.Contestants.Where(a => a.ElectionId == id);
            StringBuilder Elec = new StringBuilder();
            foreach (var item in cona)
            {
                Elec.Append(item.Nickname + " ,");
            }
            ViewBag.ElecRemoved = Elec;
            if (election == null)
            {
                return HttpNotFound();
            }
            return View(election);
        }

        // POST: Elections/Delete/5
        [HttpPost, ActionName("Delete")]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Election election = db.Elections.Find(id);
            if (election.Contestants != null)
            {
                foreach(var child in election.Contestants.ToList())
                {
                    db.Contestants.Remove(child);
                }
               
                db.Elections.Remove(election);
                //db.DeleteVoterElection(election.Post);


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
