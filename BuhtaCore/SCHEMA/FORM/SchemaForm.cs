using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SchemaObject))]
    public class SchemaForm : SchemaObject
    {

        private SchemaFormControl rootControl;
        public SchemaFormControl RootControl
        {
            get { return rootControl; }
            set { rootControl = value; firePropertyChanged("RootColumn"); }
        }


        public override BaseEdit_Page GetEditForm_page()
        {
            return new BaseEdit_Page("SchemaFormDesigner_page() { EditedRecord = this }");
        }

        ////public void Render(Control parentControl)
        ////{
        ////    RootControl.Render(parentControl);
        ////}

        public override string GetTypeDisplay
        {
            get
            {
                return "Форма";
            }
        }

        public void OpenInMainFormTab(string tabText)
        {
            ////XtraTabPage tabPage;
            ////tabPage = new XtraTabPage();
            ////var pageContent = new Base_page();
            ////pageContent.Dock = DockStyle.Fill;

            ////pageContent.AfterClose += new Base_page.AfterCloseEventHandler(sender_page =>
            ////{
            ////    //tabPages.Remove(focusedElement.ID);
            ////    //App.MainTabControl.SelectedTabPage = this.Parent as XtraTabPage;
            ////    //treeList.RefreshDataSource();
            ////    //treeList.SetFocusedNode(editedNode);
            ////});

            ////tabPage.Controls.Add(pageContent);
            ////tabPage.Text = tabText;
            ////App.MainTabControl.TabPages.Add(tabPage);
            ////Render(pageContent);
            //////pageContent.LoadData();
            //////tabPages.Add(focusedElement.ID, tabPage);

            ////App.MainTabControl.SelectedTabPage = tabPage;
        }
    }
}
