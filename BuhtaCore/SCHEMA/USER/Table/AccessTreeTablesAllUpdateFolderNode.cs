using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTablesAllUpdateFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            return null;
        }

        public override string GetDisplayName()
        {
            return "Все ИЗМЕНЕНИЕ";
        }

        protected override byte[] GetLocalID()
        {
            return TablesAllUpdateAccessNodeID.ToByteArray();
        }

        public override bool GetIsSupportAllowDenyPermission()
        {
            return true;
        }

    }
}
