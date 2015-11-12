using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTablesAllDeleteOwnedFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            return null;
        }

        public override string GetDisplayName()
        {
            return "Все УДАЛЕНИЕ СВОИХ";
        }

        protected override byte[] GetLocalID()
        {
            return TablesAllDeleteOwnedAccessNodeID.ToByteArray();
        }
        public override bool GetIsSupportAllowDenyPermission()
        {
            return true;
        }

    }
}
