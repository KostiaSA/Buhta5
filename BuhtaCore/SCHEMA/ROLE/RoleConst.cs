using DevExpress.XtraTreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{
    public static class RoleConst
    {
        public static Guid Таблица_Ключ = Guid.Parse("D30967FF-63E0-4C94-9334-9D57EA54D501");
        public static Guid Таблица_Колонка = Guid.Parse("D9B30302-509B-4F4B-8006-32CBAB4E055D");
        public static Guid Таблица = Guid.Parse("129419AE-32D3-456B-9532-5EE0C7A56DEE");

        public static Guid ВложеннаяТаблица = Guid.Parse("127E0D0A-7260-48BD-8C7B-13AF248B325C");
        public static Guid ВложеннаяТаблица_Мастер = Guid.Parse("A0ECA3C7-D9EE-4E39-88E9-AE229B7ACDE2");
        public static Guid ВложеннаяТаблица_БизнесОперация = Guid.Parse("311E63D5-3456-4F48-A615-4D054036AD78");

        public static Guid Регистр = Guid.Parse("EFA63E28-C713-4E63-B0A9-F7DBBF29EC9E");
        public static Guid Регистр_ДбКр = Guid.Parse("CBAC5FB3-DEA6-4A13-A3F9-3717C45CD193");
        public static Guid Регистр_Дата = Guid.Parse("5FA21AD1-44F0-410A-89DE-456FF40F902D");

        public static Guid Регистр_Измерение = Guid.Parse("5ED8DF51-23EF-4609-A870-7A5D175B5E62");
        public static Guid Регистр_Мера = Guid.Parse("6F6B1B9F-9E26-4D1F-AFEC-8960E3DF59DA");
        public static Guid Регистр_Реквизит = Guid.Parse("ADA7C6BD-3433-4755-ABFA-B26A1BAC840C");

        public static Guid Регистр_Мастер = Guid.Parse("34BFC596-C450-4DB5-9D37-90C3099DE555");
        public static Guid Регистр_Деталь = Guid.Parse("F7F04BB8-CEDF-43D3-81BB-323A1FB300BF");
        public static Guid Регистр_КонфигДеталь = Guid.Parse("717F22F9-78F4-46C0-9585-F9F20A4BF32E");
        public static Guid Регистр_КонфигБизнесОперация = Guid.Parse("8B1F5EA0-1621-4262-8C12-E5EC388D259E");
        public static Guid Регистр_КонфигПроводка = Guid.Parse("2AC7CF7C-9DAA-460F-A020-885C47DD29E4");

        public static Guid Справочник = Guid.Parse("55ECC74A-B9B1-432B-8743-517F8066958B");

        public static Guid Документ = Guid.Parse("0302A919-F292-4F01-AAEF-BDBF4E4CD0AA");
        public static Guid Документ_Дата = Guid.Parse("22EE024C-C2C9-457E-A06F-5674289AF1E3");
        public static Guid Документ_Сумма = Guid.Parse("1EF063F4-46A1-4212-994F-812B7FB071FF");
    }

}
