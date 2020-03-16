using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TrialManager.Model;

namespace TrialManager.Services
{
    public class PrintService : IPrintService
    {
        public async Task<bool> Print(IEnumerable<TrialistDrawEntry> drawEntries, string title)
        {
            return await Task.Run(() =>
            {
                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                {
                    FlowDocument fd = new FlowDocument()
                    {
                        PageHeight = pd.PrintableAreaHeight,
                        PageWidth = pd.PrintableAreaWidth,
                        TextAlignment = TextAlignment.Center,
                        ColumnWidth = 500,
                        FontSize = 13,
                    };
                    fd.BringIntoView();

                    // Add the document title
                    Paragraph titleParagragh = new Paragraph(new Run(title))
                    {
                        FontSize = 16
                    };
                    fd.Blocks.Add(titleParagragh);

                    Table table = new Table
                    {
                        CellSpacing = 0,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1, 1, 0, 0)
                    };

                    TableRowGroup tableRowGroup = new TableRowGroup
                    {
                        Background = Brushes.DarkGray,
                        Foreground = Brushes.White,
                        FontWeight = FontWeights.Bold
                    };
                    TableRow tableRow = new TableRow();

                    // Create the header row
                    tableRow.Cells.Add(GetHeaderTableCell("Run Number"));
                    tableRow.Cells.Add(GetHeaderTableCell("Name"));
                    tableRow.Cells.Add(GetHeaderTableCell("Status"));
                    tableRow.Cells.Add(GetHeaderTableCell("Dog Name"));
                    tableRow.Cells.Add(GetHeaderTableCell("Dog Status"));

                    tableRowGroup.Rows.Add(tableRow);
                    table.RowGroups.Add(tableRowGroup);

                    // Add the individual entries to the table
                    tableRowGroup = new TableRowGroup();
                    foreach (TrialistDrawEntry entry in drawEntries)
                    {
                        tableRow = new TableRow();

                        TableCell runNumberCell = GetTableCell(entry.RunNumber.ToString());
                        runNumberCell.FontWeight = FontWeights.Bold;
                        tableRow.Cells.Add(runNumberCell);
                        tableRow.Cells.Add(GetTableCell(entry.TrialistName));
                        tableRow.Cells.Add(GetTableCell(entry.TrialistStatus));
                        tableRow.Cells.Add(GetTableCell(entry.CompetingDogName));
                        tableRow.Cells.Add(GetTableCell(entry.CompetingDogStatus));

                        tableRowGroup.Rows.Add(tableRow);
                    }
                    table.RowGroups.Add(tableRowGroup);

                    fd.Blocks.Add(table);
                    pd.PrintDocument(((IDocumentPaginatorSource)fd).DocumentPaginator, title);
                    return true;
                }
                return false;
            }).ConfigureAwait(false);
        }

        private TableCell GetTableCell(string cellData)
        {
            return new TableCell(new Paragraph(new Run(cellData)))
            {
                ColumnSpan = 4,
                Padding = new Thickness(4),
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0, 0, 1, 1)
            };
        }

        private TableCell GetHeaderTableCell(string cellData)
        {
            return new TableCell(new Paragraph(new Run(cellData)))
            {
                ColumnSpan = 4,
                Padding = new Thickness(4),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
        }
    }
}
