using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiX.Model
{
    class PageInfo
    {
        string pageName;
        Uri pageUri;
        bool isEnable;
        bool isSelected;

        public string PageName
        {
            get { return pageName; }
        }

        public Uri PageUri
        {
            get { return pageUri; }
        }

        public bool IsEnable
        {
            get { return isEnable; }
        }

        public bool IsSelected
        {
            get { return isSelected; }
        }

        public PageInfo(string argPageName,Uri argPageUri,bool argIsEnable)
        {
            this.pageName = argPageName;
            this.pageUri = argPageUri;
            this.isEnable = argIsEnable;
        }
    }
}
