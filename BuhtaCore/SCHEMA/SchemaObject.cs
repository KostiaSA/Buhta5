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

namespace Buhta
{
    public interface IEditControl
    {
    }

    public class BaseEdit_Page
    {
        public BaseEdit_Page(string s) { }
    }
    public class Bitmap
    {
        public Bitmap(string s) { }
    }

    public class Color
    {
        public Color(string s) { }
        public static Color Black;
    }

    public interface ISchemaDesignerElement
    {
        string GetSchemaDesignerDisplayName();
        Bitmap GetSchemaDesignerImage();
        Color GetSchemaDesinerColor();
        string GetSchemaDesignerDescription();
        DateTime? GetSchemaDesignerChangeDate();
        string GetSchemaDesignerChangeUser();
    }

    public interface ISchemaTreeListNode
    {
        Guid ID { get; set; }
        Guid? ParentObjectID { get; set; }
        string DisplayName { get; }
    }

    //  [BsonKnownTypes(typeof(SchemaFolder))]
    public class SchemaObject : ISupportInitialize, INotifyPropertyChanged, ISchemaTreeListNode, ISchemaDesignerElement
    {
        public Guid id;
        public Guid ID
        {
            get { return id; }
            set { id = value; firePropertyChanged("ID"); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private Guid? parentObjectID;
        public Guid? ParentObjectID
        {
            get { return parentObjectID; }
            set { parentObjectID = value; firePropertyChanged("ParentObjectID"); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; firePropertyChanged("Description"); }
        }

        private DateTime createDate;
        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; firePropertyChanged("CreateDate"); }
        }


        private Guid? createUserID;
        public Guid? CreateUserID
        {
            get { return createUserID; }
            set { createUserID = value; firePropertyChanged("CreateUserID"); }
        }

        private DateTime? changeDate;
        public DateTime? ChangeDate
        {
            get { return changeDate; }
            set { changeDate = value; firePropertyChanged("ChangeDate"); }
        }

        private Guid? changeUserID;
        public Guid? ChangeUserID
        {
            get { return changeUserID; }
            set { changeUserID = value; firePropertyChanged("ChangeUserID"); }
        }

        private DateTime? lockDateTime;
        public DateTime? LockDateTime
        {
            get { return lockDateTime; }
            set { lockDateTime = value; firePropertyChanged("LockDateTime"); }
        }

        private Guid? lockedByUserID;
        public Guid? LockedByUserID
        {
            get { return lockedByUserID; }
            set { lockedByUserID = value; firePropertyChanged("LockedByUserID"); }
        }

        private int position;
        public int Position
        {
            get { return position; }
            set { position = value; firePropertyChanged("Position"); }
        }

        private string scriptCode;
        public string ScriptCode
        {
            get { return scriptCode; }
            set { scriptCode = value; firePropertyChanged("ScriptCode"); }
        }

        public virtual void PrepareNew()
        {
            ID = Guid.NewGuid();
        }

        public virtual void firePropertyChanged(string propertyName)
        {
            //            columnsByName.Clear();
            cached_ParentObject = null;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        ////public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        ////{
        ////    if (info.Column.FieldName == "Name")
        ////        info.CellData = (info.Node as SchemaObject).Name;
        ////    else
        ////        if (info.Column.FieldName == "DisplayName")
        ////            info.CellData = (info.Node as SchemaObject).DisplayName;
        ////        else
        ////            if (info.Column.FieldName == "Position")
        ////                info.CellData = (info.Node as SchemaObject).Position;
        ////            else
        ////                info.CellData = null;
        ////}

        ////public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        ////{
        ////    info.Children = App.Schema.GetSampleChildObjects(ID);

        ////}

        ////public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        ////{
        ////    throw new NotImplementedException();
        ////}


        public virtual BaseEdit_Page GetEditForm_page()
        {
            throw new NotImplementedException();
        }

        public virtual Bitmap GetImage()
        {
            return new Bitmap("global::Buhta.Properties.Resources.save_disk_blue_16");
        }


        public virtual void Validate(StringBuilder error)
        {
            if (ID == Guid.Empty)
                error.AppendLine("Пустое поле 'ID'.");
            if (string.IsNullOrWhiteSpace(Name))
                error.AppendLine("Не заполнено поле 'Имя'.");
        }

        //public void Save()
        //{
        //    App.Schema.SchemaObjectsCollection.Save<SchemaObject>(this);

        //}

        public string GetJsonText()
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore
            };
            string json_text = JsonConvert.SerializeObject(this, Formatting.Indented, jsonSettings);

#if DEBUG
            File.WriteAllText(@"c:\$\" + ID.ToString() + "-" + Name.ToString().TranslateToCorrectFileName() + ".txt", json_text);
#endif
            return json_text;
        }

        [JsonIgnore]
        public virtual string GetTypeDisplay
        {
            get
            {
                return GetType().Name;
            }
        }

        [JsonIgnore]
        [Browsable(false)]
        public virtual string DisplayName
        {
            get
            {
                return GetModulePrefix() + Name;
            }
        }


        public string GetModulePrefix()
        {
            var module = GetModule();
            if (module == null)
                return "";
            else
                return module.Prefix + ".";
        }

        public SchemaModule GetModule()
        {
            if (this is SchemaModule)
                return null;
            else
                if (ParentObjectID == null)
                return null;
            else
            {
                var parent = GetParentObject();
                if (parent is SchemaModule)
                    return parent as SchemaModule;
                else
                    return parent.GetModule();
            }
        }

        SchemaObject cached_ParentObject;
        public SchemaObject GetParentObject()
        {
            if (ParentObjectID == null)
                return null;
            else
                if (cached_ParentObject == null)
            {
                cached_ParentObject = App.Schema.GetSampleObject<SchemaObject>((Guid)ParentObjectID);
            }
            return cached_ParentObject;
        }


        void ISupportInitialize.BeginInit()
        {
            //throw new NotImplementedException();
        }


        public virtual void EndInit()
        {
            ////            if (this is SchemaTable && String.IsNullOrWhiteSpace(ScriptCode))

            ////                ScriptCode = @"
            ////using System;
            ////using System.Collections.Generic;
            ////using System.IO;
            ////using System.Linq;
            ////using System.Text;
            ////using System.Threading.Tasks;
            ////using System.Windows.Forms;
            ////using Buhta;

            ////namespace BuhtaExt
            ////{

            ////    public class myscript:SchemaTable_Script
            ////    {

            ////        public override void Initialize()
            ////        {
            //////            base.Initialize();
            ////              //throw new Exception(""пиздец""+Table.Name);

            ////        }

            ////    }
            ////}

            ////";
            ////            if (App.RegisteredScripts.Keys.Contains(ID))
            ////            {
            ////                dynamic script = Activator.CreateInstance(App.RegisteredScripts[ID]);
            ////                script.ScriptedObject = this;
            ////                script.Initialize();
            ////            }
            ////            else
            ////                if (!String.IsNullOrWhiteSpace(ScriptCode))
            ////                {
            ////                    try
            ////                    {
            ////                        ScriptEngine = new AsmHelper(CSScript.LoadCode(ScriptCode, null, true));
            ////                        dynamic script = ScriptEngine.CreateObject("*");
            ////                        script.ScriptedObject = this;
            ////                        script.Initialize();
            ////                        //ScriptEngine.Invoke("*." + delegateName, new object[] { GetContext(), this, param });
            ////                    }
            ////                    catch (CompilerException e)
            ////                    {
            ////                        throw new Exception(e.GetFullMessage());
            ////                    }
            ////                }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual string GetSchemaDesignerDisplayName()
        {
            return Name;
        }

        public virtual Bitmap GetSchemaDesignerImage()
        {
            return GetImage();
        }

        public virtual Color GetSchemaDesinerColor()
        {
            return Color.Black;
        }

        public virtual string GetSchemaDesignerDescription()
        {
            return Description;
        }


        public virtual DateTime? GetSchemaDesignerChangeDate()
        {
            if (ChangeDate == null)
                return CreateDate;
            else
                return ChangeDate;
        }

        public virtual string GetSchemaDesignerChangeUser()
        {
            if (ChangeUserID == null)
                return App.Schema.GetObjectName(CreateUserID);
            else
                return App.Schema.GetObjectName(ChangeUserID);
        }
    }

}


