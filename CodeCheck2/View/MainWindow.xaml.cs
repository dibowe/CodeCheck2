using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MiX.Model;
using System.Data;
using System.Windows.Controls;

namespace MiX
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        App thisApp = (App)Application.Current;
        string currentUser;
        int checkModel;
        bool isAddLock = false;
        Barcoder barcoder;

        DataTable barcodeTable, barcodeRepeatTable;




        public MainWindow()
        {
            InitializeComponent();
            InitInfoBoard();
            this.TB_BarcodeInput.Focus();
            this.SetTip(9);          
            this.COMB_PatternsListInit();



        }

        //设置“匹配的机种”ComBox的数据源
        //并设置列表中的第一项为默认选项
        private void COMB_PatternsListInit()
        {
            barcoder = Barcoder.Create(TextBox.TextProperty,this.TB_BarcodeInput);
            this.COMB_PatternsList.ItemsSource = barcoder.patternsList;
            this.COMB_PatternsList.DisplayMemberPath = "Name";
            this.COMB_PatternsList.SelectedValuePath = "Text";
            this.COMB_PatternsList.SelectedIndex = 0; //将列表中的第一项设为默认选项
        }
        private void InitInfoBoard()
        {
            currentUser = thisApp.CurrentUser;
            checkModel = thisApp.CheckMode;
            this.TB_CurrentUser.Text = currentUser;

            if(checkModel==2)
            {
                this.TB_CheckModel.Text = "产线重检";
            }
            else
            {
                this.TB_CheckModel.Text = "打印初检";
            }
        }
        
        //条码格式和数量检查函数
        private int Check()
        {
            string currentBarcode= this.TB_BarcodeInput.Text.Trim();
            string _insertBarcodeSql = "INSERT INTO BarcodeLib (Barcode,InputTime,InputName) VALUES('"
                                    + currentBarcode + "',GetDate(),'" + currentUser + "')";


            int _checkResult = 0;
            int _checkFormatResult = 0;
            int _checkNumsResult = 0;
            int _isSuccessSaveToDB = 0;

            

            //执行添加之前先检查是否已解除锁定，只能单击解除锁定按钮解除。
            if (this.isAddLock)
            {
                MessageBox.Show("请先解除锁定！");
                return 1;
            }

            //如果条码格式验证通过，进行条码唯一性检查
            //如果格式不正确，则停止重复性检查
            if (barcoder.CheckFormat(currentBarcode,COMB_PatternsList.SelectedIndex))
            {
                _checkNumsResult = Check_BarCodeNums(currentBarcode);

                if(_checkNumsResult==0)
                {
                    _isSuccessSaveToDB = thisApp.DataProvider.InsertBySql(_insertBarcodeSql);
                }
                else//唯一性检查没通过，将Inser sql中的BarcodeLib改为BarCodeRepeat
                {
                    FilterListItems(currentBarcode);
                    _insertBarcodeSql = "INSERT INTO BarcodeREpeat (Barcode,InputTime,InputName) VALUES('"
                                    + currentBarcode + "',GetDate(),'" + currentUser + "')";
                    _isSuccessSaveToDB = thisApp.DataProvider.InsertBySql(_insertBarcodeSql);
                }
                
            }
            else
            {
                _checkFormatResult = 1;
            }

            _checkResult = _checkFormatResult + _checkNumsResult+_isSuccessSaveToDB;
            SetTip(_checkResult);

            return _checkResult;

        }

        private int Check_BarCodeNums(string currentBarcode)
        {
            int _checkResult = 0;
            string _selectSqlOnlyBarcode = "SELECT COUNT(*) FROM barcodelib WHERE barcode='"+currentBarcode+"'";
            string _selectSqlWithUserName = "SELECT COUNT(*) FROM barcodelib WHERE barcode='" + currentBarcode + "' AND InputName ='"+currentUser+"'";

            if(checkModel==1)
            {
                _checkResult=thisApp.DataProvider.GetRecordCounter(_selectSqlOnlyBarcode);
            }
            else
            {
                _checkResult = thisApp.DataProvider.GetRecordCounter(_selectSqlWithUserName);
            }

            if (_checkResult >= 1)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        private void LoadDataToDataGrid()
        {
            barcodeTable = thisApp.DataProvider.RefreshBarCodeTable("SELECT * FROM BarcodeLib WHERE SN >", "BarcodeLib");
            barcodeRepeatTable = thisApp.DataProvider.RefreshBarCodeTable("SELECT * FROM BarcodeRepeat WHERE SN >", "BarcodeRepeat");
            
            if(this.LIST_Barcode.ItemsSource==null)
            {
                this.LIST_Barcode.ItemsSource = barcodeTable.DefaultView;               
            }
            if(this.LIST_BarcodeRepeat.ItemsSource == null)
            {
                this.LIST_BarcodeRepeat.ItemsSource = barcodeRepeatTable.DefaultView;
            }

        }

        private void FilterListItems(string barcode)
        {
            if(barcodeTable==null)
            {
                MessageBox.Show("请先连接数据库！");
                return;
            }
            if (barcode == "")
            {
                barcodeTable.DefaultView.RowFilter = null;
                return;
            }
            string _filterStr = "BarCode='" + barcode + "'";
            if(this.LIST_Barcode.ItemsSource==null)
            {
                MessageBox.Show("请先连接数据库！");
                return;
            }

            barcodeTable.DefaultView.RowFilter = _filterStr;

        }

        private void SelectListItems(string barcode)
        {
            //this.LIST_Barcode.SelectedItem
        }

        private void LIST_BarcodeRepeat_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //SelectionChanged事件，当选择一个条目后，鼠标点击其他控件也会发生
            //MSDN:只要选择更改就会发生此事件。不仅可以通过用户交互，而且可以通过绑定方式来更改选择和其他设置值。
            //当重新设置LIST_BarcodeRepeat的数据源ItemSource时，SelectItem就为空了
            //ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);

            DataRowView _selectItem = ((sender as DataGrid).SelectedItem as DataRowView);

            if(_selectItem!=null)
            {
                string _barcode = _selectItem["BarCode"].ToString().Trim();
                FilterListItems(_barcode);
            }

        }

        private void BTN_Conn_CLK(object sender, RoutedEventArgs e)
        {
            LoadDataToDataGrid();
            this.TB_BarcodeInput.Focus();
        }

        //窗体响应回车键，功能同单击添加按钮
        private void KeyEnterEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Check();
            }
        }

        private void BTN_Add_CLK(object sender, RoutedEventArgs e)
        {
            Check();
        }

        //解除锁定
        private void BTN_Unlock_CLK(object sender, RoutedEventArgs e)
        {
            this.isAddLock = false;
            this.SetTip(9);
        }

        private void BTN_Restore_Click(object sender, RoutedEventArgs e)
        {
            FilterListItems("");
        }

        private void BTN_Reload_CLK(object sender, RoutedEventArgs e)
        {

            LoadDataToDataGrid();
            this.TB_BarcodeInput.Focus();
        }

        //在MainWindow初始化时，检查一些值
        //例如App.CheckMode、App.CurrentUser
        private void Window_Initialized(object sender, System.EventArgs e)
        {
            //if(thisApp.CheckMode==0 || thisApp.CurrentUser==null)
            //{
            //    MessageBox.Show("主窗口初始化失败：可能是由于未没有执行登录！");
            //    this.Hide();
            //    this.Close();
            //}

        }

        private void SetTip(int id)
        {
            switch (id)
            {
                case 0:
                    RTB_Tip.FontSize = 72;
                    RTB_Tip.Foreground = Brushes.Green;
                    RTB_Tip.Text = "OK";
                    this.TB_BarcodeInput.Focus();
                    this.TB_BarcodeInput.Clear();
                    LoadDataToDataGrid();
                    FilterListItems("");
                    break;
                case 1:
                    RTB_Tip.FontSize = 72;
                    RTB_Tip.Foreground = Brushes.Red;
                    RTB_Tip.Text = "格式不正确";
                    this.isAddLock = true;
                    break;
                case 2:
                    RTB_Tip.FontSize = 72;
                    RTB_Tip.Foreground = Brushes.Red;
                    RTB_Tip.Text = "发现重复";
                    this.isAddLock = true;
                    LoadDataToDataGrid();
                    break;
                case 3:
                    RTB_Tip.FontSize = 72;
                    RTB_Tip.Foreground = Brushes.Red;
                    RTB_Tip.Text = "请先连接数据库";
                    break;

                case 4:
                    RTB_Tip.FontSize = 72;
                    RTB_Tip.Foreground = Brushes.Red;
                    RTB_Tip.Text = "保存到数据库发现异常";
                    break;
                case 6:
                    RTB_Tip.FontSize = 72;
                    RTB_Tip.Foreground = Brushes.Red;
                    RTB_Tip.Text = "保存到数据库发现异常";
                    break;

                case 9:
                    RTB_Tip.FontSize = 72;
                    RTB_Tip.Foreground = Brushes.SkyBlue;
                    RTB_Tip.Text = "等待输入……";
                    this.TB_BarcodeInput.Focus();
                    this.TB_BarcodeInput.Clear();
                    break;
                default:
                    RTB_Tip.FontSize = 72;
                    RTB_Tip.Foreground = Brushes.Red;
                    RTB_Tip.Text = "未知错误";
                    break;
            }
        }


    }
}
