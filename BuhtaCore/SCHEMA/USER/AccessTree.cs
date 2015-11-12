using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{


    public class AccessTree : AccessTreeNode
    {

        protected override List<AccessTreeNode> GetChildNodes()
        {
            var list = new List<AccessTreeNode>();

            list.Add(new AccessTreeTablesFolderNode() { ParentNode = this });
            list.Add(new AccessTreeQueriesFolderNode() { ParentNode = this });

            return list;
        }

        protected override byte[] GetLocalID()
        {
            return RootAccessNodeID.ToByteArray();
        }

        public override string GetDisplayName()
        {
            return "Root";
        }


    }
}
