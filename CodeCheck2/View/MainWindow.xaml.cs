using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MiX.Model;
using MiX.View;
using System.Collections.ObjectModel;

namespace MiX.View
{
    /// <summary>
    /// Window.xaml 的交互逻辑
    /// 创建一个集合对象，元素是Page类型，这个集合对象具有以下功能：
    ///     1、包含Addpage对象的方法。
    ///     2、包含移除page对象的方法。
    ///     3、在主界面初始化的时候，加载用户有权限的页面。
    ///     4、将Menu项绑定到这个对象，没有加载的页面，也没有对应的Menu菜单。
    /// 自定义TabControl，使其具有：
    ///     1、每个TabItem都包含一个Frame控件。
    ///     2、每个TabItem都包含1个关闭按钮。
    ///     3、将其ItemSource绑定到模型。
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<PageInfo> PageCollection;//TabCtrl.ItemSource的数据源
        public MainWindow()
        {
            InitializeComponent();
            
            
            PageCollection = new ObservableCollection<PageInfo>();
            this.MainTabCtrl.ItemsSource = PageCollection;
            AddPage("StartPage");
            this.MainMenu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(this.OnMenuClick));

        }

        private void OnMenuClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem=e.OriginalSource as MenuItem;

            if(menuItem!=null)
            {
                PageSwitch(menuItem.Header.ToString());
            }
            e.Handled = true;
        }

            

        private void PageSwitch(string menuName)
        {
            //MessageBox.Show("MenuName is :" + menuName);
            string pageName="";
            switch(menuName)
            {
                case "加班时":
                    break;
                case "条码检查":
                    pageName = "PageCodeCheck";
                    break;
                case "起始页":
                    pageName = "StartPage";
                    break;
                default:
                    break;
            }

            AddPage(pageName);

        }

        private bool CheckAuthorisation(string menuName)
        {
            return true;
        }
        private void AddPage(string pageName)
        {
            if(pageName=="")
            {
                return;
            }

            //控制页的数量
            if(PageCollection.Count>=7)
            {
                return;
            }

            foreach(PageInfo p in PageCollection)
            {
                if(p.PageName==pageName)
                {
                    //将绑定这个page的TabItem设为选中
                    return;
                }
            }

            Uri pageUri = new Uri("Pages/"+pageName + ".xaml",UriKind.Relative);
            PageInfo newPage = new PageInfo(pageName,pageUri,true);

            this.PageCollection.Add(newPage);
        }
    }
}
