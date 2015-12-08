using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public class ValidateError
    {
        public string ErrorObjectName;
        public string ErrorMessage;

    }

    public class ValidateErrorList
    {
        List<ValidateError> errorList = new List<ValidateError>();
        public void AddError(string errorObjectName, string errorMessage)
        {
            errorList.Add(new ValidateError() { ErrorObjectName = errorObjectName, ErrorMessage = errorMessage });
        }

        public bool IsEmpty
        {
            get
            {
                return errorList.Count == 0;
            }
        }

        public MvcHtmlString ToHtmlString()
        {
            if (IsEmpty) return MvcHtmlString.Empty;

            var sb = new StringBuilder();

            foreach (var error in errorList)
            {
                if (string.IsNullOrWhiteSpace(error.ErrorObjectName))
                    sb.Append(@"<span>" + error.ErrorMessage.AsHtml() + "</span><br>");
                else
                    sb.Append(@"<span>""" + error.ErrorObjectName.AsHtml() + @""": " + error.ErrorMessage.AsHtml() + "</span><br>");
            }
            sb.RemoveLastChar(4);
            return new MvcHtmlString(sb.ToString());

        }

        public MvcHtmlString ToHtmlStringOnlyMessages()
        {
            if (IsEmpty) return MvcHtmlString.Empty;

            var sb = new StringBuilder();

            foreach (var error in errorList)
            {
                sb.Append("<span>" + error.ErrorMessage.AsHtml() + "</span><br>");
            }
            sb.RemoveLastChar(4);
            return new MvcHtmlString(sb.ToString());

        }

    }

}


