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
    public class ContestantsController : Controller
    {
        private IVotingModelEntities db = new IVotingModelEntities();
        [SessionExpireFilter]
        [NonAction]
        private bool checker(Contestant contestant)
        {

            Election election = new Election();
           // contestant.Party = db.Parties.Find(int.Parse(contestant.Party)).PartyName;
            election = db.Elections.Find(contestant.ElectionId);
            int ExpectedContestants = election.NoOfContestants;
            int AvailableContestantsCount = db.Contestants.Where(m => m.ElectionId == contestant.ElectionId).Count();
            if (ExpectedContestants > AvailableContestantsCount)
            {
                
                return true;
            }
            else
            {
                
                return false;

            }
            
        }
        // GET: Contestants
        [SessionExpireFilter]
        public ActionResult Index()
        {
            var contestants = db.Contestants.Include(c => c.Election);
            
            return View(contestants.ToList());
        }

        // GET: Contestants/Details/5
        [SessionExpireFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contestant contestant = db.Contestants.Find(id);
            if (contestant == null)
            {
                return HttpNotFound();
            }
            return View(contestant);
        }

        // GET: Contestants/Create
        [SessionExpireFilter]
        public ActionResult Create()
        {
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Post");
            ViewBag.Party = new SelectList(db.Parties, "PartyName","PartyName");
            List<SelectListItem> PartyReader = new List<SelectListItem>();
            foreach (var item in db.Parties)
            {
                SelectListItem s = new SelectListItem();
                s.Value = item.PartyName;
                s.Text = item.PartyName;
                PartyReader.Add(s);
            }
            ViewBag.Party = PartyReader;
            return View();
        }
            

         [SessionExpireFilter]
        public bool PartyChecker(Contestant ConInstance)
        {
            var Con = db.Contestants.Where(m => m.ElectionId.Equals(ConInstance.ElectionId));
            foreach (var item in Con)
            {
                var Scan = item.Party;
                if (Scan == ConInstance.Party)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            
          
            return true;
        }
        // POST: Contestants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nickname,Party,PartyId,ElectionId,ProfilePicturePath")] Contestant contestant,HttpPostedFileBase profilefile)
        {
            
            bool rep = checker(contestant);
            if (ModelState.IsValid && profilefile != null && profilefile.ContentLength > 0)
            {
              
                if ((rep == true)  /*&&(!PartyRep)*/)
                { 
                        string filename = Path.GetFileName(profilefile.FileName);
                        string physicalPath = Server.MapPath("~/Content/Contestants/" + filename);
                        profilefile.SaveAs(physicalPath);
                        contestant.ProfilePicturePath = filename;
                        contestant.Count = 0;
                        var pat = db.Parties.Where(a => a.PartyName.Equals(contestant.Party));
                        foreach (var item in pat)
                        {
                            contestant.PartyPartyId = item.PartyId;
                        }

                        db.Entry(contestant).State = EntityState.Modified;
                        db.Contestants.Add(contestant);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else if ((rep == false) /*&&(!PartyRep)*/)
                      {
                        ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Post", contestant.ElectionId);
                        ViewBag.Party = new SelectList(db.Parties, "PartyName", "PartyName");
                        ViewBag.Report = "Contestants Limit Exceeded";
                        return View();

                    }
                    /*else if (PartyRep)
                    {
                        ViewBag.Report = contestant.Party + " already has a contestant";
                        return View(contestant);
                    }*/

                }

            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Post", contestant.ElectionId);
                return View(contestant);
           
        }

        // GET: Contestants/Edit/5
        [SessionExpireFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contestant contestant = db.Contestants.Find(id);
            if (contestant == null)
            {
                return HttpNotFound();
            }
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Post", contestant.ElectionId);
            return View(contestant);
        }

        // POST: Contestants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nickname,Party,ElectionId,ProfilePicturePath")] Contestant contestant)
        {
            bool rep = checker(contestant);
            if (ModelState.IsValid && rep==true)
            {
                db.Entry(contestant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ElectionId = new SelectList(db.Elections, "Id", "Post", contestant.ElectionId);
            return View(contestant);
        }

        // GET: Contestants/Delete/5
        [SessionExpireFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contestant contestant = db.Contestants.Find(id);
            if (contestant == null)
            {
                return HttpNotFound();
            }
            return View(contestant);
        }

        // POST: Contestants/Delete/5
        [HttpPost, ActionName("Delete")]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contestant contestant = db.Contestants.Find(id);
            db.Contestants.Remove(contestant);
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
