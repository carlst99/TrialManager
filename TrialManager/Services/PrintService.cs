using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using TrialManager.Model;

namespace TrialManager.Services
{
    public class PrintService : IPrintService
    {
        public void Print(IEnumerable<TrialistDrawEntry> drawEntries, string title, DataGrid dg)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                FlowDocument fd = new FlowDocument()
                {
                    PageHeight = pd.PrintableAreaHeight,
                    PageWidth = pd.PrintableAreaWidth,
                    TextAlignment = TextAlignment.Center,
                    ColumnWidth = 500
                };
                fd.BringIntoView();

                // Add the document title
                Paragraph titleParagragh = new Paragraph(new Run(title))
                {
                    FontSize = 16,
                    FontFamily = dg.FontFamily
                };
                fd.Blocks.Add(titleParagragh);

                Table table = new Table
                {
                    CellSpacing = 0,
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1, 1, 0, 0),
                    FontStyle = dg.FontStyle,
                    FontFamily = dg.FontFamily,
                    FontSize = 13
                };

                TableRowGroup tableRowGroup = new TableRowGroup();
                TableRow tableRow = new TableRow();

                List<string> headerList = dg.Columns.Select(e => e.Header.ToString()).ToList();
                List<string> bindList = new List<string>();

                // Create the header row
                for (int i = 0; i < headerList.Count; i++)
                {
                    tableRow.Cells.Add(new TableCell(new Paragraph(new Run(headerList[i])))
                    {
                        ColumnSpan = 4,
                        Padding = new Thickness(4),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Background = Brushes.DarkGray,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold
                    });
                    Binding binding = (dg.Columns[i] as DataGridBoundColumn)?.Binding as Binding;
                    bindList.Add(binding.Path.Path);
                }
                tableRowGroup.Rows.Add(tableRow);
                table.RowGroups.Add(tableRowGroup);

                // Add the individual entries to the table
                tableRowGroup = new TableRowGroup();
                foreach (TrialistDrawEntry entry in drawEntries)
                {
                    tableRow = new TableRow();

                    tableRow.Cells.Add(GetTableCell(entry.RunNumber.ToString()));
                    tableRow.Cells.Add(GetTableCell(entry.TrialistName));
                    tableRow.Cells.Add(GetTableCell(entry.TrialistStatus));
                    tableRow.Cells.Add(GetTableCell(entry.CompetingDogName));
                    tableRow.Cells.Add(GetTableCell(entry.CompetingDogStatus));

                    tableRowGroup.Rows.Add(tableRow);
                }
                table.RowGroups.Add(tableRowGroup);

                //for (int i = 0; i < dg.Items.Count; i++)
                //{
                //    dynamic row;

                //    if (string.Equals(dg.ItemsSource.ToString(), "system.data.linqdataview", StringComparison.OrdinalIgnoreCase))
                //        row = (DataRowView)dg.Items.GetItemAt(i);
                //    else
                //        row = (TrialistDrawEntry)dg.Items.GetItemAt(i);

                //    tableRowGroup = new TableRowGroup();
                //    tableRow = new TableRow();

                //    for (int j = 0; j < row.Row.ItemArray.Count(); j++)
                //    {
                //        if (string.Equals(dg.ItemsSource.ToString(), "system.data.linqdataview", StringComparison.OrdinalIgnoreCase))
                //            tableRow.Cells.Add(new TableCell(new Paragraph(new Run(row.Row.ItemArray[j].ToString()))));
                //        else
                //            tableRow.Cells.Add(new TableCell(new Paragraph(new Run(row.GetType().GetProperty(bindList[j]).GetValue(row, null)))));

                //        tableRow.Cells[j].ColumnSpan = 4;
                //        tableRow.Cells[j].Padding = new Thickness(4);
                //        tableRow.Cells[j].BorderBrush = Brushes.DarkGray;
                //        tableRow.Cells[j].BorderThickness = new Thickness(0, 0, 1, 1);
                //    }

                //    tableRowGroup.Rows.Add(tableRow);
                //    table.RowGroups.Add(tableRowGroup);
                //}

                fd.Blocks.Add(table);
                pd.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, title);
            }
        }

        private TableCell GetTableCell(string cellData)
        {
            return new TableCell(new Paragraph(new Run(cellData)))
            {
                ColumnSpan = 4,
                Padding = new Thickness(4),
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0, 0, 1, 1),
                FontWeight = FontWeights.Bold
            };
        }
    }
}
