﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buhta.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var model = new SchemaTableColumnEditModel(this, null);
            //model.Column = new SchemaTableColumn() { Name = "это жопа1" };
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application 1 description page.";

            TestClass1.Test1();
            TestClass1.Test2();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Chat()
        {
            return View();
        }



        SchemaTable OrgTable;

        public ActionResult EditTable()
        {
            return View();
        }

        private void X_OnChange(xInput sender, string newValue)
        {
            throw new NotImplementedException();
        }
    }
}