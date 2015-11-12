using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTablesSelectivelyFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            var list = new List<AccessTreeNode>();

            var tables = (from table in App.Schema.GetSampleObjects<SchemaTable>()
                          orderby table.DisplayName
                          select table).ToList();

            foreach (var table in tables)
            {
                var node = new AccessTreeTableNode() { ParentNode = this, Table = table };
                list.Add(node);
            }

            return list;
        }

        public override string GetDisplayName()
        {
            return "ВЫБОРОЧНО";
        }

        protected override byte[] GetLocalID()
        {
            return TablesSelectivelyAccessNodeID.ToByteArray();
        }

    }
}
