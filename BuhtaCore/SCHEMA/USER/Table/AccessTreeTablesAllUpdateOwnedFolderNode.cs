using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTablesAllUpdateOwnedFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            return null;
        }

        public override string GetDisplayName()
        {
            return "Все ИМЕНЕНИЕ СВОИХ";
        }

        protected override byte[] GetLocalID()
        {
            return TablesAllUpdateOwnedAccessNodeID.ToByteArray();
        }

        public override bool GetIsSupportAllowDenyPermission()
        {
            return true;
        }


    }
}
