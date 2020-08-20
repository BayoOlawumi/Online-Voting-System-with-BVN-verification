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


namespace SeunEvote.Controllers.Admin
{
    public class ElectionDetailsController : Controller
    {
        private IVotingModelEntities db = new IVotingModelEntities();

        // GET: ElectionDetails
       // [SessionExpireFilter]
        public ActionResult Index()
        {
            return View(db.ElectionDetails.ToList());
        }

        // GET: ElectionDetails/Details/5
        [SessionExpireFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionDetail electionDetail = db.ElectionDetails.Find(id);
            if (electionDetail == null)
            {
                return HttpNotFound();
            }
            return View(electionDetail);
        }

        // GET: ElectionDetails/Create
     
        // POST: ElectionDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
       // [SessionExpireFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ElectionStartTime,ElectionStopTime")] ElectionDetail electionDetail)
        {
            if (ModelState.IsValid)
            {
                db.ElectionDetails.Add(electionDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(electionDetail);
        }

        // GET: ElectionDetails/Edit/5
        [SessionExpireFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionDetail electionDetail = db.ElectionDetails.Find(id);
            if (electionDetail == null)
            {
                return HttpNotFound();
            }
            return View(electionDetail);
        }

        // POST: ElectionDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionExpireFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ElectionStartTime,ElectionStopTime")] ElectionDetail electionDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(electionDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(electionDetail);
        }

        // GET: ElectionDetails/Delete/5
        [SessionExpireFilter]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ElectionDetail electionDetail = db.ElectionDetails.Find(id);
            if (electionDetail == null)
            {
                return HttpNotFound();
            }
            return View(electionDetail);
        }

        // POST: ElectionDetails/Delete/5
        [SessionExpireFilter]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ElectionDetail electionDetail = db.ElectionDetails.Find(id);
            db.ElectionDetails.Remove(electionDetail);
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
