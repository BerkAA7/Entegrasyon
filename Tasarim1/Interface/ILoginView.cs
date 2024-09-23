using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToPanorama.Interface
{
    public interface ILoginView
    {
        List<IMusteri> ReadExcelFile(string filePath);
        List<IMusteri> GetMusteriList();
        void MusteriAL(List<IMusteri> list);
    }
}
