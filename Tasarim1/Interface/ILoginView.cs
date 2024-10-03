using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ExcelToPanorama.Interface
{
    public interface ILoginView
    {

       List<IMusteri> GetMusteriList();
        void MusteriAL(List<IMusteri> list);
    }
    
    public interface IDataGridHelpers
    {
        void ClearRowCellBackground<T>(T item, List<T> itemList, DataGrid dataGrid);
        DataGridCell GetDataGridCell(DataGrid dataGrid, DataRowView row);
        DataGridCell GetDataGridCell(FrameworkElement element);
    }

    public interface IExcelHelpers
    {
        void BtnExcelYukle<T>(Func<string, List<T>> readExcelFileFunc, Action<List<T>> alFunc);
        List<T> ReadExcelFile<T>(string filePath);
        string GetCellValue(IXLRow row, Dictionary<string, int> columnIndices, string columnName, int defaultIndex);
        string ConvertCustomersToXML(List<Tasarim1.CustomerIntegration> customers, string UserName, string panServisSifresi, string firmaKodu, string calismaYili, string dist);
    }
}
