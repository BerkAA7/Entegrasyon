using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_LoginForm.View;

namespace ExcelToPanorama.Class
{
    public class ExcelDepolama
    {
        LoginView loginView;
        IMusteri _musteri;
        public ExcelDepolama(IMusteri musteri)
        {
            _musteri = musteri;
        }
        //public string Oku()
        //{
        //    loginView.Durum
        //}
    }
}
