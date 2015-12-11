using Buhta;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Компания
{
    public static class Const
    {
        public static Guid ЮрЛицо = Guid.Parse("42D05053-9281-40F6-951E-CB67A3D806ED");
        public static Guid ЮрЛицо_ИНН = Guid.Parse("56E8BC7A-550D-4BD6-B297-62FE2F9A0E8D");

        public static Guid ФизЛицо = Guid.Parse("74196FDF-5E95-4858-86F0-4A8665317943");
        public static Guid ФизЛицо_Фамилия = Guid.Parse("43284903-E9E5-40F5-9D70-74BB0FAF8014");
        public static Guid ФизЛицо_Имя = Guid.Parse("4F52E30F-7605-4375-B9A8-DC45933E7F86");
        public static Guid ФизЛицо_Отчество = Guid.Parse("F0FC6D92-94A4-4AB5-A7E1-A04088081A1F");

        public static Guid ДоговорРеализации = Guid.Parse("762B1329-1B6B-4507-8F4F-1788B4456068");
        public static Guid ДоговорРеализации_Клиент = Guid.Parse("A152F93B-AA4C-4223-99D0-881B66C3DF68");

        public static Guid Клиент = Guid.Parse("57929AE4-4153-492E-9C7B-0A7EEA560348");
    }


}
