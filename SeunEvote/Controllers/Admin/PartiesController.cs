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
using System.Text;

namespace SeunEvote.Controllers
{
    public class PartiesController : Controller
    {
        private IVotingModelEntities db = new IVotingModelEntities();

        // GET: Parties
        [SessionExpireFilter]
        public ActionResult Index()
        {
            return View(db.Parties.ToList());
        }

        // GET: Parties/Details/5


        // GET: Parties/Create
        [SessionExpireFilter]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Parties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PartyName,LogoPath")] Party party, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                string filenamewithout = Path.GetFileNameWithoutExtension(file.FileName);
                string fileext = Path.GetExtension(file.FileName);
                string filename = filenamewithout + DateTime.Now.ToString("yymmssfff") + fileext;
                string physicalPath = Server.MapPath("~/Content/Parties/"+ filename);
                file.SaveAs(physicalPath);
                party.LogoPath = filename;
                db.Parties.Add(party);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(party);
        }

        // GET: Parties/Edit/5
        [SessionExpireFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Party party = db.Parties.Find(id);
            if (party == null)
            {
                return HttpNotFound();
            }
            return View(party);
        }

        // POST: Parties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [SessionExpireFilter]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PartyId,PartyName,LogoPath")] Party party,HttpPostedFileBase fileupdate)
        {
            if (ModelState.IsValid)
            {
                string filenamewithout = Path.GetFileNameWithoutExtension(fileupdate.FileName);
                string fileext = Path.GetExtension(fileupdate.FileName);
                string filename = filenamewithout+DateTime.Now.ToString("yymmssfff") + fileext;
                string physicalPath = Server.MapPath("~/Content/Parties/" + filename);
                fileupdate.SaveAs(physicalPath);
                party.LogoPath = filename;
                //db.UpdateParty(party.PartyId, party.PartyName, party.LogoPath);
                db.Contestants.Where(m => m.PartyPartyId.Equals(party.PartyId)).FirstOrDefault().Party=party.PartyName;
  
                db.Entry(party).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(party);
        }

        // GET: Parties/Delete/5
        [SessionExpireFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Party party = db.Parties.Find(id);
            var cona = db.Contestants.Where(a => a.PartyPartyId == id);
            StringBuilder Cont = new StringBuilder();
            foreach (var item in cona)
            {
                Cont.Append(item.Nickname + " ,");
            }
            ViewBag.ContRemoved = Cont;
            if (party == null)
            {
                return HttpNotFound();
            }
            return View(party);
        }

        // POST: Parties/Delete/5
        [SessionExpireFilter]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Party party = db.Parties.Find(id);
            var cona = db.Contestants.Where(a => a.PartyPartyId==id);
            db.Parties.Remove(party);
            db.Contestants.RemoveRange(cona);
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
