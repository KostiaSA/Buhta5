using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeTableNode : AccessTreeNode
    {
        public SchemaTable Table;
        protected override List<AccessTreeNode> GetChildNodes()
        {
            var list = new List<AccessTreeNode>();

            //var tables = (from table in App.Schema.GetSampleObjects<SchemaTable>()
            //              orderby table.DisplayName
            //              select table).ToList();

            //foreach (var table in tables)
            //{
            //    var node=new AccessTreeTablesFolderNode
            //}

            //list.Add(new AccessTreeTablesFolderNode());
            return list;
        }

        protected override byte[] GetLocalID()
        {
            return Table.ID.ToByteArray();
        }

        public override string GetDisplayName()
        {
            return Table.DisplayName;
        }

        public override bool GetIsSupportAllowDenyPermission()
        {
            return true;
        }

        public override bool GetIsSupportReadPermission()
        {
            return true;
        }

        public override bool GetIsSupportInsertPermission()
        {
            return true;
        }

        public override bool GetIsSupportUpdatePermission()
        {
            return true;
        }

        public override bool GetIsSupportDeletePermission()
        {
            return true;
        }

        public override bool GetIsSupportOwnedUpdatePermission()
        {
            return true;
        }

        public override bool GetIsSupportOwnedDeletePermission()
        {
            return true;
        }

    }
}
