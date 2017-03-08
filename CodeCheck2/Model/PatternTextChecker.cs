using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace MiX.Model
{
    public class BarcodePattern
    {
        public string Name { get; set; }
        public string Text { get; set; }
    }


    /*    1、Barcoder作为二维码的抽象，绑定到二维码输入文本框，其属性CurrentValue表示当前输入的二维码。
          2、使用单例模式，构造函数私有
          3、创建实例时将Barcoder.CurrentValue属性绑定到某个文本属性
          4、实现属性变更自动通知功能

          
    */
    public class Barcoder:INotifyPropertyChanged
    {
        public List<BarcodePattern> patternsList;
        private static Barcoder barcoder;
        public event PropertyChangedEventHandler PropertyChanged;

        private string currentValue;

        public List<BarcodePattern> PatternsList { get; }
        public string CurrentValue
        {
            get { return currentValue; }
            set
            {
                currentValue = value;
                if(this.PropertyChanged !=null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentBarcode"));
                }

            }
        }

        public string ErrorTip { get; set; }//格式化的错误提示

        //构造函数私有
        private Barcoder(DependencyProperty targetProperty, DependencyObject target)
        {
            //设置绑定
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("CurrentValue");
            binding.Mode = BindingMode.OneWayToSource;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(target, targetProperty, binding);

            //初始化PatternsList集合
            InitPatternList();
        }

        //接受一个TextProperty属性作为参数，将其与自己的属性绑定
        public static Barcoder Create(DependencyProperty targetProperty,DependencyObject target )
        {
            if(barcoder==null)
            {
                barcoder = new Barcoder(targetProperty,target);
            }

            return barcoder;
        }

        //初始化各个机种的PatternText的列表对象，将其作为“机种选择”ComBox的数据源
        //并设置列表中的第一项为默认选项
        private void InitPatternList()
        {

            patternsList = new List<BarcodePattern>()
            {
                new BarcodePattern() { Name = "Lonestar",      Text = @"^PK23000K1V0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "Gordon",        Text = @"^PK23000K2V0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "L450",          Text = @"^PK23000KAV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "L460",          Text = @"^PK23000NDV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "G41",           Text = @"^PK23000N8V0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "G51",           Text = @"^PK23000N9V0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "Y41",           Text = @"^PK23000N4V0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "Y41",           Text = @"^PK23000N5V0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "NANO110",       Text = @"^PK23000NUV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "E47X-L",        Text = @"^PK23000NXV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "E47X-R",        Text = @"^PK23000NZV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "NANO110/17",    Text = @"^PK23000NYV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "E57X",          Text = @"^PK23000NWV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "NANO",          Text = @"^PK23000NCV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "CG410",         Text = @"^PK23000NLV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "LANCER14",      Text = @"^PK23000JY00vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "LANCER15",      Text = @"^PK23000JZ00vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "LANCER17",      Text = @"^PK23000K8V0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "CG411",         Text = @"^PK23000NRV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "CG511",         Text = @"^PK23000NSV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "SUPER X-L",     Text = @"^PK23000NKV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "SUPER X-R",     Text = @"^PK23000NJV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "DG421",         Text = @"^PK23000PQV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "DG521",         Text = @"^PK23000PRV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "DG721",         Text = @"^PK23000PUV0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}"},
                new BarcodePattern() { Name = "CT470",         Text = @"^PK23000P3V0vco2[0-9A-Z]{2}[0-9]{1}[0-9A-V]{2}101[0-9A-F]{4}" }
             };
        }

        public bool CheckFormat(string currentBarcode,int inxPatternsList)
        {
            string input = currentBarcode;
            string pattern = this.patternsList[inxPatternsList].Text;

            bool isSuccess = false;
            if (pattern==null || pattern=="")
            {
                return isSuccess;
            }


            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
            {
                isSuccess = true;
            }


            return isSuccess;
        }

    }

}
