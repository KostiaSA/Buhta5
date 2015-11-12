using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeQueryNode : AccessTreeNode
    {
        public SchemaQuery Query;
        protected override List<AccessTreeNode> GetChildNodes()
        {
            return null;
        }

        protected override byte[] GetLocalID()
        {
            return Query.ID.ToByteArray();
        }

        public override string GetDisplayName()
        {
            return Query.DisplayName;
        }

    }
}
