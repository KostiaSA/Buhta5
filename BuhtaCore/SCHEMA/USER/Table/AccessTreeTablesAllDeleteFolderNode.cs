using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTablesAllDeleteFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            return null;
        }

        public override string GetDisplayName()
        {
            return "Все УДАЛЕНИЕ";
        }

        protected override byte[] GetLocalID()
        {
            return TablesAllDeleteAccessNodeID.ToByteArray();
        }

        public override bool GetIsSupportAllowDenyPermission()
        {
            return true;
        }

    }
}
