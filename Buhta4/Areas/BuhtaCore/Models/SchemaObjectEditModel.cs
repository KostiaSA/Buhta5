using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaObjectEditModel<T>: BaseModel where T : SchemaObject
    {
        public bool NeedSave;
        public T EditedObject { get; set; }

        public virtual string EditedObjectName { get { return EditedObject.Name; } }

        public bool SaveButtonDisabled { get { return NeedSave; } }

        public void SaveButtonClick(string chromeWindowId, dynamic args)
        {
            //EditedObject.Save;
        }

    }
}
