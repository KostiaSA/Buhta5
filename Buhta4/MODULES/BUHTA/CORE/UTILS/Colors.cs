using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Buhta
{
    public static class TextColor
    {
        public static string KeyField = "textcolor-keyfield";  // оранжевый цвет ключевых полей (ID, Номер и т.п.)
        public static string DateTime = "textcolor-datetime";  // синий цвет полей с датой и временем
        public static string SchemaRole = "textcolor-schemarole";  // фиолетовый - роли

        public static string Add = "textcolor-add";  // зеленый цвет добавления
        public static string Edit = "textcolor-edit";  // синий цвет редактирования
        public static string Delete = "textcolor-delete";  // красный цвет удаления
    }
}