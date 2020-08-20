using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeunEvote.Models;
using System.IO;
using System.Web.Helpers;

namespace SeunEvote.Controllers.Public
{



    public class TesterController : Controller
    {
        // GET: Home
        private IVotingModelEntities db = new IVotingModelEntities();
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
        [NonAction]
        [System.Web.Services.WebMethod]
        public DateTime CurrentDateTimeGenerator()
        {
            ViewBag.CurrentTime = DateTime.Now;
            return DateTime.Now;
        }

        public ActionResult Index(string report, string encoded, string goal, string logger)
        {
            if (TimeVerifier() > 0)
            {
                return RedirectToAction("ElectionResult","Home", new { id = 2 });
            }
            else
            {
                if (logger == "voter")
                {
                    var deduced = db.Voteds.Where(m => m.Encoded == encoded);
                    if (deduced != null)
                    {
                        foreach (var item in deduced)
                        {
                            if (encoded == item.Encoded)
                            {
                                if ((goal == "vote") && (logger == "voter"))
                                {
                                    Session["UserEncoded"] = encoded;
                                    return RedirectToAction("Index", "Home", new { user = "logger" });
                                }

                            }

                            else
                            {

                                return RedirectToAction("NotRegistered", "Home");
                            }
                        }
                    }
                }
                else if (logger == "admin")
                {
                    var deduced2 = db.Adminees.ToList();

                    if (deduced2 != null)
                    {
                        //foreach (var item in deduced2)
                        //{
                        //    if (encoded == item.Encoded)
                        //    {
                        if (goal == "enroll")
                        {
                            Session["ToBeEnrolled"]=encoded;
                                    return RedirectToAction("Index", "Home", new { user = "admin2enroll" });

                        }
                        else if (goal == "management")
                                {
                                    return RedirectToAction("Index", "Home", new { user = "admin" });
                                }
                        //    }

                        //}
                    }
                }
                }
               
                
                ViewBag.Report = report;
                return RedirectToAction("NotRegistered","Home");
            }

        }
    }
