using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    [JsonObject(IsReference = true)]
    public class SchemaTableSystemColumn:SchemaTableColumn
    {

    }

}
