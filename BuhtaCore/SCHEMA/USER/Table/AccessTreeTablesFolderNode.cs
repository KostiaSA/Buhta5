using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTablesFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            var list = new List<AccessTreeNode>();

            list.Add(new AccessTreeTablesAllReadFolderNode() { ParentNode = this });
            list.Add(new AccessTreeTablesAllInsertFolderNode() { ParentNode = this });
            list.Add(new AccessTreeTablesAllUpdateFolderNode() { ParentNode = this });
            list.Add(new AccessTreeTablesAllDeleteFolderNode() { ParentNode = this });
            list.Add(new AccessTreeTablesAllUpdateOwnedFolderNode() { ParentNode = this });
            list.Add(new AccessTreeTablesAllDeleteOwnedFolderNode() { ParentNode = this });
            list.Add(new AccessTreeTablesSelectivelyFolderNode() { ParentNode = this });

            return list;
        }

        public override string GetDisplayName()
        {
            return "Таблицы";
        }

        protected override byte[] GetLocalID()
        {
            return TablesFolderAccessNodeID.ToByteArray();
        }

    }
}
