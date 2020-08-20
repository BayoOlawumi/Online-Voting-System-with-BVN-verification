using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SeunEvote.Models;
using System.IO;

namespace SeunEvote.Controllers
{
    public class VotersDetsController : Controller
    {
        private IVotingModelEntities db = new IVotingModelEntities();

        // GET: VotersDetails
        [SessionExpireFilter]
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return View(db.VotersDets.ToList());
            }
            else if(id!=null)
            {
                ViewBag.Password = db.Voteds.Find(id).Password;
                ViewBag.Surname = db.Voteds.Find(id).Surname;
                return View(db.VotersDets.ToList());
            }
            else{
                return View(db.VotersDets.ToList());
            }
        }

        // GET: VotersDetails/Details/5
        [SessionExpireFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VotersDet votersDetail = db.VotersDets.Find(id);
            if (votersDetail == null)
            {
                return HttpNotFound();
            }
            return View(votersDetail);
        }

        // GET: VotersDetails/Create
        [SessionExpireFilter]
      public ActionResult Create()
        {
            //ViewBag.Encoded = Session["ToBeEnrolled"];
            //if (ViewBag.Encoded == null)
            //{
            //    return RedirectToAction("index", "elections",new { report = "..." + Session["AdminName"].ToString()});
            ////}
            return View();
        }
      //  [SessionExpireFilter]
        public string FolderCreator(string surname)
        {
            System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/Voters/" + surname));
            string destination = string.Format("~/Content/Voters/{0}/",surname);
            return destination;
        }

        // POST: VotersDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Create( VotersDet votersDetail, HttpPostedFileBase passport, HttpPostedFileBase certificateoforigin)
        {
                if (ModelState.IsValid)
                {
                
                    string des= FolderCreator(votersDetail.Surname);

                    string filenamewithoutpassport = Path.GetFileNameWithoutExtension(passport.FileName);
                    string fileextpassport = Path.GetExtension(passport.FileName);

                    

                    string filenamewithoutCOO = Path.GetFileNameWithoutExtension(certificateoforigin.FileName);
                    string fileextCOO = Path.GetExtension(certificateoforigin.FileName);

                    string filenamepassport = filenamewithoutpassport + DateTime.Now.ToString("yymmssfff") + fileextpassport;
                    string physicalPathPassport = Server.MapPath(des + filenamepassport);

                    string filenameCOO = filenamewithoutCOO + DateTime.Now.ToString("yymmssfff") + fileextCOO;
                    string physicalPathCOO = Server.MapPath(des + filenameCOO);

                  

                     passport.SaveAs(physicalPathPassport);
                    certificateoforigin.SaveAs(physicalPathCOO);
                     

                    votersDetail.Passport = filenamepassport;
                    votersDetail.CertificateOfOrigin = filenameCOO;
                    
                     Session["ToBeEnrolled"] = null;

                    db.VotersDets.Add(votersDetail);
                     db.SaveChanges();
                     VotersConfirm(votersDetail);

                     Voted newLogger = new Voted();
                    ViewBag.Password = newLogger.Password = System.Web.Security.Membership.GeneratePassword(12, 1);
                    ViewBag.Surname = newLogger.Surname = votersDetail.Surname;
                    
                    
                    newLogger.HasVoted = false;
                    newLogger.Encoded = "40";
                    newLogger.BVN = votersDetail.BVN;
                    newLogger.PHONE_NUMBER = votersDetail.PhoneNumber;
                    db.Voteds.Add(newLogger);
                    db.SaveChanges();
                    return RedirectToAction("Index" , new { id = newLogger.Id });
            }


            return View(votersDetail);
        }
        [SessionExpireFilter]
        [HttpGet]
        public ActionResult VotersConfirm(VotersDet voy)
        {
            
            
            return RedirectToAction("Details", new { id = voy.VoterId });
       
        }


        // GET: VotersDetails/Edit/5
        [SessionExpireFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VotersDet votersDetail = db.VotersDets.Find(id);
            if (votersDetail == null)
            {
                return HttpNotFound();
            }
            return View(votersDetail);
        }

        // POST: VotersDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Surname,Middlename,Lastname,Age,StateOfOrigin,LocalGovt,PhoneNumber,NOKName,NOKPhoneNo")] VotersDet votersDetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(votersDetail).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "unable to save changes");
            }
            return RedirectToAction("Index");
        }

        // GET: VotersDetails/Delete/5
        [SessionExpireFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VotersDet votersDetail = db.VotersDets.Find(id);
            if (votersDetail == null)
            {
                return HttpNotFound();
            }
            return View(votersDetail);
        }

        // POST: VotersDetails/Delete/5
        [SessionExpireFilter]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VotersDet votersDetail = db.VotersDets.Find(id);
            db.VotersDets.Remove(votersDetail);
            var RVoter = db.Voteds.Where(m => m.Id.Equals(votersDetail.VoterId));
            db.Voteds.RemoveRange(RVoter);
            db.SaveChanges();
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
