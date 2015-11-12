using CSScriptLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaObject_Script : Component
    {
        [Browsable(false)]
        public Object ScriptedObject;

        public virtual void Initialize()
        {
        }

    }

    public delegate void OnBeforeSaveRecord(object recordToSave);

    public class SchemaTable_Script : SchemaObject_Script
    {
      //  public event OnBeforeSaveRecord OnBeforeSaveRecord;

        public SchemaTable Table { get { return (SchemaTable)ScriptedObject; } }


        //public override void Initialize()
        //{
        //    //            base.Initialize();
        //}

    }
}
