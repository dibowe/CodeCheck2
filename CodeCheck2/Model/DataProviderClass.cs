using System;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Configuration;

namespace MiX.Model
{

    //在每个执行sql的方法内，检查传入的sql命令文本是否合格
    //使用正则表达式
    public class DataProviderClass
    {
        DataSet someSet;
        SqlConnection sqlConnection;
        SqlCommand SelectCmd,InsertCmd;
        SqlDataAdapter sqlAdapter;
        SqlConnectionStringBuilder connStrBuld;

        protected DataProviderClass()
        {
            connStrBuld = new SqlConnectionStringBuilder();
            someSet = new DataSet();
        }


        //数据库连接初始化
        public bool InitConnection()
        {
            //如果连接对象为空，创建连接并打开
            if (sqlConnection == null)
            {
                try
                {
                    //ConnectionStringsSection connSection = App.DencryptConfig("MiX.exe.Config", "connectionStrings") as ConnectionStringsSection;
                    //MessageBox.Show(connSection.ConnectionStrings[1]);
                    connStrBuld.DataSource =        ConfigurationManager.ConnectionStrings["ConnStr_Server"].ConnectionString;
                    connStrBuld.InitialCatalog =    ConfigurationManager.ConnectionStrings["ConnStr_DataBase"].ConnectionString;
                    connStrBuld.UserID =            ConfigurationManager.ConnectionStrings["ConnStr_UserID"].ConnectionString;
                    connStrBuld.Password =          ConfigurationManager.ConnectionStrings["ConnStr_Pwd"].ConnectionString; ;
                    connStrBuld.IntegratedSecurity = false;
                    connStrBuld.PersistSecurityInfo = false;

                    sqlConnection = new SqlConnection(connStrBuld.ConnectionString);
                    sqlConnection.Open();
                }
                catch(Exception e)
                {
                    //写入日志到文件，从此处看，需要分别设置写入日志到文件、写入日志到数据库
                    //在此处调用写入日志到数据库会造成循环调用
                    MessageBox.Show("数据库连接初始化失败："+e.Message);
                    return false;
                }
            }

            return true;
        }

        //使用正则表表达式检查命令文本是否合格
        private bool CheckSql(string sqlText)
        {
            //验证不通过抛出异常
            return true;
        }
        private SqlCommand InitSqlCommand(string sqlText,SqlCommand sqlCmd)
        {
            if(!InitConnection())
            {
                return null;
            }
            //检查命令文本是否合格
            if (CheckSql(sqlText))
            {
                if (sqlCmd == null)
                {
                    sqlCmd = new SqlCommand();
                }
                sqlCmd.CommandText = sqlText;
                sqlCmd.Connection = sqlConnection;

                return sqlCmd;
            }
            else
            {
                return null;
            }
        }

        public int InsertBySql(string _insertSql)
        {
            this.InsertCmd=InitSqlCommand(_insertSql,this.InsertCmd);

            try
            {
                InsertCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 4;
            }

            return 0;
        }

        public DataTable CreateDataTable(string selectSql, string tableName)
        {

            if (someSet.Tables[tableName] == null)
            {
                DataTable newTable = new DataTable(tableName);
                someSet.Tables.Add(newTable);
            }

            return someSet.Tables[tableName];
        }

        public DataTable RefreshBarCodeTable (string selectSql, string tableName)
        {
            DataTable theTable = CreateDataTable(selectSql, tableName);
            int rowNums = theTable.Rows.Count;

            if(rowNums==0)
            {
                selectSql = selectSql + "0";
            }
            else
            {
                DataRow lastRow = theTable.Rows[rowNums - 1];
                int lastRowSN = (int)lastRow["SN"];
                selectSql = selectSql + lastRowSN.ToString();
            }


            if (this.sqlAdapter == null)
            {
                this.sqlAdapter = new SqlDataAdapter();
            }
            //如果初始化命令对象成功，在InitSqlCommand内部会先检查sql文本的合规性。
            this.sqlAdapter.SelectCommand = InitSqlCommand(selectSql, this.SelectCmd);
            if (this.sqlAdapter.SelectCommand != null)
            {
                this.sqlAdapter.Fill(theTable);

                return theTable;
            }
            else
            {
                return null;
            }

        }
        public int GetRecordCounter(string selectSql)
        {
            int recordNums = 0;
            this.SelectCmd = InitSqlCommand(selectSql, SelectCmd);
            if (this.SelectCmd==null)
            {
                //抛出异常
            }

            recordNums = (int)this.SelectCmd.ExecuteScalar();

            return recordNums;
        }

        public int CloseConnection()
        {
            int _closedState = 9;
            this.sqlConnection.Close();
            switch (this.sqlConnection.State)
            {
                case ConnectionState.Open:
                    this.sqlConnection.Close();
                    _closedState = 0;
                    break;
                case ConnectionState.Broken:
                    _closedState = 0;
                    break;
                case ConnectionState.Closed:
                    _closedState = 0;
                    break;
                case ConnectionState.Connecting:
                    _closedState = 3;
                    break;
                case ConnectionState.Executing:
                    _closedState = 4;
                    break;
                case ConnectionState.Fetching:
                    _closedState = 5;
                    break;
                default:
                    _closedState = 9;
                    break;
            }

            return _closedState;

        }


    }
    public class BarcodeDataProviderClass:DataProviderClass
    {
        static BarcodeDataProviderClass bdp;

        private BarcodeDataProviderClass()
        {

        }

        public static BarcodeDataProviderClass Create()
        {
            if(bdp==null)
            {
                bdp = new BarcodeDataProviderClass();
            }

            return bdp;
        }
    }

}
