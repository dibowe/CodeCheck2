using System;
using System.Windows;
using System.Windows.Input;
using MiX.Model;

namespace MiX
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        AppLogStruct logStruct = new AppLogStruct();
        MainWindow mainWind;
        App thisApp = (App)Application.Current;
        public Login()
        {
            InitializeComponent();
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            if(this.checkBox1.IsChecked==true)
            {
                this.textBlock1.Visibility = Visibility.Visible;
            }

            else
            {
                this.textBlock1.Visibility = Visibility.Collapsed;
            }

        }

        private void BTN_Login_Click(object sender, RoutedEventArgs e)
        {
            UserLogin();
        }

        private void KeyEnterEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UserLogin();
            }
        }

        //执行Login
        private void UserLogin()
        {
            string userName = CB_USERNAME.Text.Trim();
            string pwd = PB_PWD.Password;
            
            if(LoginVerify(userName,pwd))
            {
                //必须先设置当前用户和检查模式，再创建MainWindow
                //因为MainWindow的构造器中会引用这些变量

                SetCurrentUser(userName);
                SetCheckModel();
                mainWind = new MainWindow();
                this.Hide();
                mainWind.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("登录失败");
            }
        }

        private bool LoginVerify(string userName,string pwd)
        {
            string _selectText = "SELECT COUNT(*) FROM uinfo WHERE uname='"+userName+"' AND pwd='"+pwd+"'";
            string loginInfo = "用户名：" + userName + ";密码：" + pwd + ";";
            if (thisApp.DataProvider.GetRecordCounter(_selectText)==1)
            {
                loginInfo = loginInfo + "登录验证：成功！";
                logStruct.CName = thisApp.ClientEnv.ComputerName;
                logStruct.UName = userName;
                logStruct.ClientTime = DateTime.Now;
                logStruct.LogType = "登录验证";
                logStruct.Message = "登录验证：成功！";
                thisApp.WriteLogToDB(logStruct);
                thisApp.WriteLogToFile(logStruct);
                return true;
            }
            else
            {
                loginInfo = loginInfo + "登录验证：失败;";
                logStruct.CName = thisApp.ClientEnv.ComputerName;
                logStruct.UName = userName;
                logStruct.ClientTime = DateTime.Now;
                logStruct.LogType = "登录验证";
                logStruct.Message = "登录验证：失败！";
                thisApp.WriteLogToDB(logStruct);
                thisApp.WriteLogToFile(logStruct);
                return false;
            }
        }

        private void SetCurrentUser(string userName)
        {
            //将用户输入转为大写，再设置当前用户
            thisApp.CurrentUser = userName.ToUpper();
        }

        private void SetCheckModel()
        {
            if(checkBox1.IsChecked==true)
            {
                thisApp.CheckMode = 2;
            }
            else
            {
                thisApp.CheckMode = 1;
            }
        }

        //不论是否登录成功吗，都会在登录时将用户使用的登录信息写入日志
        //应包括用户名+密码及其验证结果，检查模式、登录时间

    }
}
