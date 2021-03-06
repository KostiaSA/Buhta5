﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public static partial class HtmlHelperExtensions
    {
        public static MvcHtmlString bsTreeToolbarButton(this HtmlHelper helper, Action<bsTreeToolbarButton> settings)
        {
            var Settings = new bsTreeToolbarButton(helper.ViewData.Model as BaseModel);
            settings(Settings);

            (helper.ViewData.Model as BaseModel).Helper = helper;
            //var script = new StringBuilder();
            //var html = new StringBuilder();

            return new MvcHtmlString(Settings.GetHtml());
            //return new MvcHtmlString(Settings.EmitScriptAndHtml(script, html));
        }

    }

    public enum bsTreeToolbarButtonRole { None, Add, Edit, Delete, ExpandAll, CollapseAll, SelectAll, UnselectAll }

    public class bsTreeToolbarButton : bsButton
    {
        public bsTreeToolbarButton(BaseModel model) : base(model) { }

        public bsTree Tree;

        public bsTreeToolbarButtonRole Role = bsTreeToolbarButtonRole.None;

        public override void EmitScriptAndHtml(StringBuilder script, StringBuilder html)
        {
            AddStyle("margin-left", "5px");
         //   AddStyle("margin-bottom", "10px");

            if (Role == bsTreeToolbarButtonRole.ExpandAll)
                Bind_OnClick(Tree.JavaScript_ExpandAll);

            if (Role == bsTreeToolbarButtonRole.CollapseAll)
                Bind_OnClick(Tree.JavaScript_CollapseAll);

            base.EmitScriptAndHtml(script, html);
        }
    }


}
