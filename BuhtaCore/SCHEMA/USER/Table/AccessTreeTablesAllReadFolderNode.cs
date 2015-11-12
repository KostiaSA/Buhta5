using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTablesAllReadFolderNode : AccessTreeNode
    {
        protected override List<AccessTreeNode> GetChildNodes()
        {
            return null;
        }

        public override string GetDisplayName()
        {
            return "Все ЧТЕНИЕ";
        }

        protected override byte[] GetLocalID()
        {
            return TablesAllReadAccessNodeID.ToByteArray();
        }

        public override bool GetIsSupportAllowDenyPermission()
        {
            return true;
        }

    }
}
