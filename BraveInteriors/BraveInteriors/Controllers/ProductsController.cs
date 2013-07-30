using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BraveInteriors.Controllers
{
    public class ProductsController : Controller
    {
        //
        // GET: /Products/

        public ActionResult Index(string category, string tag)
        {
            ViewBag.Category = category;
            ViewBag.Partial = string.Format("_{0}", tag);
            return View();
        }

    }
}
