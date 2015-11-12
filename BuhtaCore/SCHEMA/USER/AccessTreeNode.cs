using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class AccessTreeNode
    {
        public static Guid RootAccessNodeID = Guid.Parse("E0AB12CA-0260-4DFB-835E-60C44D097382");

        public static Guid TablesFolderAccessNodeID = Guid.Parse("3579BAFD-24F4-45D5-B0A8-E27358DDE92A");
        public static Guid TablesAllReadAccessNodeID = Guid.Parse("42C62A9D-8842-40AA-8EE9-ACB17012847D");
        public static Guid TablesAllInsertAccessNodeID = Guid.Parse("B118C87B-F5CE-4489-89B6-B95B9C7B2B4F");
        public static Guid TablesAllUpdateAccessNodeID = Guid.Parse("C784E43C-4F3B-44CF-A588-E3D6E1E1551F");
        public static Guid TablesAllDeleteAccessNodeID = Guid.Parse("73B51C2B-23B7-4A9E-A109-6FE14DABD4B3");
        public static Guid TablesAllUpdateOwnedAccessNodeID = Guid.Parse("14980779-5179-43F7-896F-B302E17CF075");
        public static Guid TablesAllDeleteOwnedAccessNodeID = Guid.Parse("DA88519B-9421-40A8-BF18-2C3F48B4C513");
        public static Guid TablesSelectivelyAccessNodeID = Guid.Parse("A0350341-01BC-4E72-A2EB-BBCE1F0FB63D");

        public static Guid QueriesFolderAccessNodeID = Guid.Parse("5AF4321B-B790-4313-B529-7C5A8C4035BB");

        public AccessTreeNode ParentNode;

        List<AccessTreeNode> nodes;
        public List<AccessTreeNode> Nodes
        {
            get
            {
                if (nodes == null)
                    nodes = GetChildNodes();
                return nodes;
            }
        }

        protected virtual List<AccessTreeNode> GetChildNodes()
        {
            return new List<AccessTreeNode>();
        }

        public virtual string GetDisplayName()
        {
            return "GetDisplayName()";
        }

        public static byte[] GetSha256(Guid id)
        {
            SHA256 sha256 = SHA256Managed.Create();
            return sha256.ComputeHash(id.ToByteArray());
        }

        public static byte[] GetSha256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
        }

        public static byte[] GetSha256(byte[] x, byte[] y)
        {
            var z = new byte[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);
            SHA256 sha256 = SHA256Managed.Create();
            return sha256.ComputeHash(z);
        }

        public static byte[] GetSha256(byte[] array)
        {
            SHA256 sha256 = SHA256Managed.Create();
            return sha256.ComputeHash(array);
        }

        byte[] cached_ID;
        public byte[] ID
        {
            get
            {
                if (cached_ID == null)
                {
                    if (ParentNode == null)
                        cached_ID = GetSha256(GetLocalID());
                    else
                        cached_ID = GetSha256(ParentNode.ID, GetLocalID());
                }
                return cached_ID;
            }
        }

        protected virtual byte[] GetLocalID()
        {
            throw new Exception("internal error FD3EB7B6");
        }

        public bool GetIsSupportPermissions()
        {
            return 
                GetIsSupportAllowDenyPermission() ||
                GetIsSupportReadPermission() ||
                GetIsSupportInsertPermission() ||
                GetIsSupportUpdatePermission() ||
                GetIsSupportDeletePermission() ||
                GetIsSupportOwnedUpdatePermission() ||
                GetIsSupportOwnedDeletePermission();
        }

        public virtual bool GetIsSupportAllowDenyPermission()
        {
            return false;
        }

        public virtual bool GetIsSupportReadPermission()
        {
            return false;
        }

        public virtual bool GetIsSupportInsertPermission()
        {
            return false;
        }

        public virtual bool GetIsSupportUpdatePermission()
        {
            return false;
        }

        public virtual bool GetIsSupportDeletePermission()
        {
            return false;
        }

        public virtual bool GetIsSupportOwnedUpdatePermission()
        {
            return false;
        }

        public virtual bool GetIsSupportOwnedDeletePermission()
        {
            return false;
        }

    }
}
