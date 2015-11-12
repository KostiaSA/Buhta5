using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buhta
{
    public class TestClass1
    {
        static SchemaObject obj;
        public static void Test1()
        {
            obj = new SchemaObject();
            //obj.Subscribe(args =>
            //{
            //    var xx = args.LocationName+args.Value;
            //});

        }

        public static void Test2()
        {
            obj.Name = "Жопа";
        }
    }
}
