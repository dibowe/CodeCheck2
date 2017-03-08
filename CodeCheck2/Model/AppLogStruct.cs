using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiX.Model
{
    enum SqlCommandType { Select, Insert, Delete, Update };
    public struct AppLogStruct
    {
        public string CName;
        public string UName;
        public string LogType;
        public DateTime ClientTime;
        public DateTime DBTime;
        public string Message;
    }


}
