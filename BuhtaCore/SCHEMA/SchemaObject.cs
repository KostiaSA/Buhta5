using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Buhta
{
    public class SchemaObject : ObservableObject
    {
        //public Observable<string> LastName { get { return getVar<string>("LastName"); } }

        public Guid ID { get; set; }

        public string Name { get; set; }


        public string Description { get; set; }


        public int Position { get; set; }


        public virtual void PrepareNew()
        {
            ID = Guid.NewGuid();
        }

        public virtual void firePropertyChanged(string propertyName)
        {
            //cached_ParentObject = null;
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public virtual void Validate(StringBuilder error)
        {
            if (ID == Guid.Empty)
                error.AppendLine("Пустое поле 'ID'.");
            if (string.IsNullOrWhiteSpace(Name))
                error.AppendLine("Не заполнено поле  'Имя'.");
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
            // File.WriteAllText(@"c:\$\" + ID.ToString() + "-" + Name.ToString().TranslateToCorrectFileName() + ".txt", json_text);
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

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual string GetSchemaDesignerDisplayName()
        {
            return Name;
        }


        public virtual string GetSchemaDesignerDescription()
        {
            return Description;
        }

    }

}


