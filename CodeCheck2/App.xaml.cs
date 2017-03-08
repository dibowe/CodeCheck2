using System.Windows;
using MiX.Model;
using System.Diagnostics;
using System;
using System.Configuration;
using System.Data;

namespace MiX
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //字段
        private BarcodeDataProviderClass dp;
        private AppLogStruct logStruct = new AppLogStruct();
        public DataTable appLog;

        //属性
        public  BarcodeDataProviderClass DataProvider { get { return dp; }}
        public int CheckMode { get; set; }
        public string CurrentUser { get; set; }
        public ClientEnvironment ClientEnv
        {
            get { return GetClientEnvironment(); }
        }
        public DataTable AppLog { get { return appLog; } }


        public App()
        {
            CheckSingleThread();
            CheckMode = 0;
            CurrentUser = null;
            dp = BarcodeDataProviderClass.Create();
            appLog = dp.RefreshBarCodeTable("select * from AppLog where SN >", "AppLog");
        }

        //
        private ClientEnvironment GetClientEnvironment()
        {
            ClientEnvironment env = new ClientEnvironment();
            env.ComputerName=Environment.MachineName;
            env.ClientTime = DateTime.Now;
            env.CurrentDirectory = Environment.CurrentDirectory;
            env.UName = CurrentUser;

            return env;
        }

        //检查是否有同名进程在运行
        private static void CheckSingleThread()
        {
            string name = Process.GetCurrentProcess().ProcessName;
            int id = Process.GetCurrentProcess().Id;
            Process[] prc = Process.GetProcesses();
            foreach (Process pr in prc)
            {
                if ((name == pr.ProcessName) && (pr.Id != id))
                {
                    MessageBox.Show("该系统正在运行，请不要重复启动!");
                    System.Environment.Exit(0);
                }
            }

        }

        //处理所有未处理的异常
        private void AppFailure(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logStruct.CName = ClientEnv.ComputerName;
            logStruct.UName = ClientEnv.UName;
            logStruct.ClientTime = ClientEnv.ClientTime;
            logStruct.LogType = "应用程序异常";
            logStruct.Message = e.Exception.ToString();
            try
            {
                WriteLogToFile(logStruct);
            }
            catch
            {
                
            }   
            finally
            {
                WriteLogToDB(logStruct);
            }         
            
        }

        public void WriteLogToFile(AppLogStruct logStruct)
        {
            //using (StreamWriter errorLog = new StreamWriter("C:\\MiX.txt",true))
            //{
            //    errorLog.WriteLine(logStruct.ClientTime.ToString());
            //    errorLog.WriteLine(logStruct.CName);
            //    errorLog.WriteLine(logStruct.UName);
            //    errorLog.WriteLine(logStruct.LogType);
            //    errorLog.WriteLine(logStruct.Message);
            //    errorLog.WriteLine("==================================");
            //}
        }

        public void WriteLogToDB(AppLogStruct logStruct)
        {
            AppLogStruct log = logStruct;
            string sql = "INSERT INTO AppLog(CName,UName,LogType,ClientTime,DBTime,Message)" +
                                    "VALUES ('"+log.CName+ "','"+log.UName+ "','"+log.LogType+ "','"
                                    +log.ClientTime.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),'"+log.Message+ "')";
            if(dp==null)
            {
                dp = BarcodeDataProviderClass.Create();
            }
            //MessageBox.Show(sql);
            dp.InsertBySql(sql);
        }

        public void WriteLog()
        {

        }

        public struct ClientEnvironment
        {
            public string UName;
            public string ComputerName;
            public DateTime ClientTime;
            public string CurrentDirectory;
        }

        //从MSDN复制来的
        //https://msdn.microsoft.com/zh-cn/library/ms254494(v=vs.110).aspx
        public static ConfigurationSection DencryptConfig(string exeConfigName , string sectionName)
        {
            
            // Takes the executable file name without the
            // .config extension.
            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.
                Configuration config = ConfigurationManager.
                    OpenExeConfiguration(exeConfigName);

                if(config==null)
                {
                    MessageBox.Show("获取配置文件失败！");
                    return null;
                }

                ConfigurationSection section = config.GetSection(sectionName);                


                if (section.SectionInformation.IsProtected)
                {
                    // Remove encryption.
                    section.SectionInformation.UnprotectSection();
                }

                return section;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static void EncryptConfig(string exeConfigName)
        {
            // Takes the executable file name without the
            // .config extension.
            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.
                Configuration config = ConfigurationManager.
                    OpenExeConfiguration(exeConfigName);

                ConnectionStringsSection section =
                    config.GetSection("connectionStrings")
                    as ConnectionStringsSection;

                if (section.SectionInformation.IsProtected)
                {
                    // Remove encryption.
                    //section.SectionInformation.UnprotectSection();

                    return;
                }
                else
                {
                    // Encrypt the section.
                    section.SectionInformation.ProtectSection(
                        "DataProtectionConfigurationProvider");
                }
                // Save the current configuration.
                config.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }

}
