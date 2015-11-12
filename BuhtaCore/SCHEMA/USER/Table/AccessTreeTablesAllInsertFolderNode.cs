using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTablesAllInsertFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            return null;
        }

        public override string GetDisplayName()
        {
            return "Все ДОБАВЛЕНИЕ";
        }

        protected override byte[] GetLocalID()
        {
            return TablesAllInsertAccessNodeID.ToByteArray();
        }

        public override bool GetIsSupportAllowDenyPermission()
        {
            return true;
        }

    }
}
