using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeQueriesFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            var list = new List<AccessTreeNode>();

            var queries = (from table in App.Schema.GetSampleObjects<SchemaQuery>()
                          orderby table.DisplayName
                          select table).ToList();

            foreach (var query in queries)
            {
                var node = new AccessTreeQueryNode() { ParentNode = this, Query = query };
                list.Add(node);

            }

            return list;
        }

        public override string GetDisplayName()
        {
            return "Запросы";
        }

        protected override byte[] GetLocalID()
        {
            return QueriesFolderAccessNodeID.ToByteArray();
        }

    }
}
