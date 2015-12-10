using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public static class TextColor
    {
        public static string KeyField = nameof(TextColor).ToLower() + "-" + nameof(KeyField).ToLower();  // оранжевый цвет ключевых полей (ID, Номер и т.п.)
        public static string DateTime = nameof(TextColor).ToLower() + "-" + nameof(DateTime).ToLower();  // синий цвет полей с датой и временем
    }
}