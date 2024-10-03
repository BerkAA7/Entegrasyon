using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ExcelToPanorama.Interface;
namespace ExcelToPanorama.Helpers
{
    internal class DataGridHelpers 
    {

        public void ClearRowCellBackground<T>(T item, List<T> itemList, DataGrid dataGrid)
        {
            // Verilen nesnenin indexini bul
            int rowIndex = itemList.IndexOf(item); // itemList, List<T> tipinde olmalı

            if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
                return; // Geçersiz index kontrolü

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
                if (cell != null)
                {
                    var dataGridCell = GetDataGridCell(cell);
                    if (dataGridCell != null)
                    {
                        dataGridCell.Background = Brushes.White; // Varsayılan arka plan rengi
                    }
                }
            }
        }

        public DataGridCell GetDataGridCell(DataGrid dataGrid, DataRowView row)
        {
            var container = dataGrid.ItemContainerGenerator.ContainerFromItem(row) as DataGridRow;
            if (container != null)
            {
                var column = dataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Seç");
                if (column != null)
                {
                    var cellContent = column.GetCellContent(container);
                    return GetDataGridCell(cellContent);
                }
            }
            return null;
        }

        public static DataGridCell GetDataGridCell(FrameworkElement element)
        {
            while (element != null && !(element is DataGridCell))
            {
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
            return element as DataGridCell;
        }

        #region AKTARILAN HÜCRELERİ BOYAMA
        /*GENERİC*/
        public void HighlightInvalidCells<T>(T item, List<T> itemList, DataGrid dataGrid, System.Windows.Media.Color color)
        {
            // Verilen nesnenin indexini bul
            int rowIndex = itemList.IndexOf(item); // itemList, List<T> tipinde olmalı

            if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
                return; // Geçersiz index kontrolü

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
                if (cell != null)
                {
                    var dataGridCell = GetDataGridCell(cell);
                    if (dataGridCell != null)
                    {
                        dataGridCell.Background = new SolidColorBrush(color); // Geçersiz hücre arka plan rengi
                    }
                }
            }
        }


        //private void HighlightSuccessfulCells(IMusteri musteri, System.Windows.Media.Color color)
        //{
        //    // IMusteri nesnesinin indexini bul
        //    int rowIndex = musteriList.IndexOf(musteri); // Eğer musteriList bir List<IMusteri> ise

        //    if (rowIndex < 0 || rowIndex >= dataGrid.Items.Count)
        //        return; // Geçersiz index kontrolü

        //    for (int i = 0; i < dataGrid.Columns.Count; i++)
        //    {
        //        var cell = dataGrid.Columns[i].GetCellContent(dataGrid.Items[rowIndex]);
        //        if (cell != null)
        //        {
        //            var dataGridCell = GetDataGridCell(cell);
        //            if (dataGridCell != null)
        //            {
        //                dataGridCell.Background = new SolidColorBrush(color); // Başarılı hücre arka plan rengi
        //            }
        //        }
        //    }
        //}
        /*GENERİC*/
        public static void HighlightSuccessfulCells<T>(T item, DataGrid dataGrid, System.Windows.Media.Color color)
        {
            // T türündeki nesneyi dataGrid'deki öğelerle eşleştir
            foreach (var gridItem in dataGrid.Items)
            {
                if (gridItem.Equals(item))
                {
                    var row = dataGrid.ItemContainerGenerator.ContainerFromItem(gridItem) as DataGridRow;
                    if (row != null)
                    {
                        for (int i = 0; i < dataGrid.Columns.Count; i++)
                        {
                            var cell = dataGrid.Columns[i].GetCellContent(row);
                            if (cell != null)
                            {
                                var dataGridCell = GetDataGridCell(cell);
                                if (dataGridCell != null)
                                {
                                    dataGridCell.Background = new SolidColorBrush(color); // Başarılı hücre arka plan rengi
                                }
                            }
                        }
                    }
                    break;
                }
            }
        }
        #endregion
    }
}
