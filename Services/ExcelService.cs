using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using TBGL.Common;

namespace TBGL.Services;

public sealed class ExcelService : IExcelService
{
    public const string ACCOUNTING_CELL_FORMAT = """_(""$""* #,##0.00_);_(""$""* \(#,##0.00\);_(""$""* ""-""??_);_(@_)""";
    
    private static readonly IReadOnlyDictionary<string, XLColor> Colors;
    
    public TrialBalanceLoadResult? TrialBalanceReport { get; private set; }
    
    public GeneralLedgerLoadResult? GeneralLedgerReport { get; private set; }

    public async Task<TrialBalanceLoadResult> LoadTrialBalanceWorkbookAsync(Uri workbookPath)
    {
        var stream = await GetWorkbookStreamAsync(workbookPath);
        var workbook = new XLWorkbook(stream);
        return TrialBalanceReport = new TrialBalanceLoadResult(workbookPath.LocalPath, workbook.Worksheet(1));
    }

    public async Task<GeneralLedgerLoadResult> LoadGeneralLedgerWorkbookAsync(Uri workbookPath)
    {
        var stream = await GetWorkbookStreamAsync(workbookPath);
        var workbook = new XLWorkbook(stream);
        return GeneralLedgerReport = new GeneralLedgerLoadResult(workbookPath.LocalPath, workbook.Worksheet(1));
    }

    public void GenerateWorkpaper(TemplateModel template, Uri path)
    {
        Debug.Assert(TrialBalanceReport is not null);

        try
        {
            using var workbook = new XLWorkbook();

            var trialBalanceWorksheet = workbook.AddWorksheet("Trial Balance Report");

            trialBalanceWorksheet.Cell(1, 1).SetValue("Property:");
            trialBalanceWorksheet.Cell(1, 2).SetValue(TrialBalanceReport.Property.ToString());

            trialBalanceWorksheet.Cell("A2").SetValue("Reporting Book:");
            trialBalanceWorksheet.Cell("B2").SetValue(TrialBalanceReport.ReportingBook);

            trialBalanceWorksheet.Cell("A3").SetValue("Start Date:");
            trialBalanceWorksheet.Cell("B3").SetValue(TrialBalanceReport.StartDate.ToString("d"));

            trialBalanceWorksheet.Cell("A4").SetValue("End Date:");
            trialBalanceWorksheet.Cell("B4").SetValue(TrialBalanceReport.EndDate.ToString("d"));

            trialBalanceWorksheet.Cell("A5").SetValue("Account Number");
            trialBalanceWorksheet.Cell("B5").SetValue("Account Name");
            trialBalanceWorksheet.Cell("C5").SetValue($"Opening balance on {TrialBalanceReport.StartDate:d}");
            trialBalanceWorksheet.Cell("D5").SetValue("Debit");
            trialBalanceWorksheet.Cell("E5").SetValue("Credit");
            trialBalanceWorksheet.Cell("F5").SetValue($"Closing balance on {TrialBalanceReport.EndDate:d}");

            var trialBalanceRow = 5;
            foreach (var account in TrialBalanceReport.Accounts)
            {
                trialBalanceRow++;
                trialBalanceWorksheet.Cell($"A{trialBalanceRow}").SetValue($"{account.Metadata.Category}-{account.Metadata.SubCategory}");
                trialBalanceWorksheet.Cell($"B{trialBalanceRow}").SetValue(account.Metadata.Name);
                trialBalanceWorksheet.Cell($"C{trialBalanceRow}").SetValue(account.OpeningBalance).Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
                trialBalanceWorksheet.Cell($"D{trialBalanceRow}").SetValue(account.Debit).Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
                trialBalanceWorksheet.Cell($"E{trialBalanceRow}").SetValue(account.Credit).Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
                trialBalanceWorksheet.Cell($"F{trialBalanceRow}").SetValue(account.ClosingBalance).Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
            }

            var range = trialBalanceWorksheet.Range($"A5:F{trialBalanceRow}");
            var table = range.CreateTable("Accounts").SetShowHeaderRow(true).SetShowTotalsRow(true).SetShowAutoFilter(false);
            table.TotalsRow().Cell(1).SetValue("Totals:");
            table.Fields.Skip(1).First().TotalsRowFunction = XLTotalsRowFunction.None;
            foreach (var field in table.Fields.Skip(2))
            {
                field.TotalsRowFunction = XLTotalsRowFunction.Sum;
                field.TotalsCell.Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
            }

            trialBalanceWorksheet.Columns().AdjustToContents();

            const int monthOffset = 2;
            foreach (var group in template.Groups)
            {
                var color = Colors.GetValueOrDefault(group.Color ?? string.Empty) ?? XLColor.NoColor;
                var sheet = workbook.AddWorksheet(group.Name).SetTabColor(color);

                var accounts = group.Filter(GeneralLedgerReport!.TransactionHistories).ToList();

                var groupRow = 1;
                var year = TrialBalanceReport.StartDate.Year;
                foreach (var account in accounts)
                {
                    var groupRowStart = groupRow;

                    sheet.Cell(groupRow, 01).SetValue(account.Metadata.ToString());
                    sheet.Cell(groupRow, 02).SetValue("Balance Forward");
                    sheet.Cell(groupRow, 03).SetValue($"January {year}");
                    sheet.Cell(groupRow, 04).SetValue($"February {year}");
                    sheet.Cell(groupRow, 05).SetValue($"March {year}");
                    sheet.Cell(groupRow, 06).SetValue($"April {year}");
                    sheet.Cell(groupRow, 07).SetValue($"May {year}");
                    sheet.Cell(groupRow, 08).SetValue($"June {year}");
                    sheet.Cell(groupRow, 09).SetValue($"July {year}");
                    sheet.Cell(groupRow, 10).SetValue($"August {year}");
                    sheet.Cell(groupRow, 11).SetValue($"September {year}");
                    sheet.Cell(groupRow, 12).SetValue($"October {year}");
                    sheet.Cell(groupRow, 13).SetValue($"November {year}");
                    sheet.Cell(groupRow, 14).SetValue($"December {year}");

                    groupRow++;
                    sheet.Cell(groupRow, 2).SetValue(account.StartingBalance).Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
                    
                    foreach (var month in account.EnumerateTransactions(false).GroupBy(x => x.PostedDate!.Value.Month))
                    {
                        foreach (var transaction in month)
                        {
                            groupRow++;
                            sheet.Cell(groupRow, 1).SetValue(transaction.Memo).Style.NumberFormat
                                .SetNumberFormatId((int)XLPredefinedFormat.Number.Text);
                            // debit is +, credit is -
                            var amount = transaction.Debit ?? -transaction.Credit;
                            sheet.Cell(groupRow, monthOffset + month.Key).SetValue(amount).Style.NumberFormat
                                .SetFormat(ACCOUNTING_CELL_FORMAT);
                        }
                    }
                    
                    var groupRange = sheet.Range(groupRowStart, 1, groupRow, 14);
                    var groupTable = groupRange.CreateTable().SetShowHeaderRow(true).SetShowTotalsRow(true).SetShowAutoFilter(false);

                    groupTable.TotalsRow().Cell(1).SetValue("Totals:");

                    var groupTableFields = groupTable.Fields.ToList();

                    for (var i = 1; i < groupTableFields.Count; i++)
                    {
                        var field = groupTableFields[i];
                            
                        field.TotalsRowFormulaR1C1 = i == 1
                            ? $"R{groupRowStart + 1}C"
                            : $"=RC[-1]+SUM(R{groupRowStart + 1}C:R{groupRow}C)";
                        field.TotalsCell.Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
                    }

                    groupRow += 3;
                    sheet.Cell(groupRow, 13).SetValue("G/L Balance:");
                    sheet.Cell(groupRow, 14).SetFormulaR1C1("=R[-2]C").Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);

                    groupRow++;
                    sheet.Cell(groupRow, 13).SetValue("TB Balance:");
                    sheet.Cell(groupRow, 14).SetFormulaR1C1($"=VLOOKUP(\"{account.Metadata.GetNumber()}\", 'Trial Balance Report'!R6C1:R{trialBalanceRow}C6, 6, FALSE)").Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
                    
                    groupRow++;
                    sheet.Cell(groupRow, 13).SetValue("Difference:");
                    sheet.Cell(groupRow, 14).SetFormulaR1C1("=ABS(R[-1]C-R[-2]C)").Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);

                    groupRow += 3;
                }

                sheet.Columns().AdjustToContents();
            }

            workbook.SaveAs(path.LocalPath);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private static async Task<MemoryStream> GetWorkbookStreamAsync(Uri workbookPath)
    {
        await using var file = File.OpenRead(workbookPath.LocalPath);
        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    static ExcelService()
    {
        var dict = new Dictionary<string, XLColor>();
        foreach (var property in typeof(XLColor).GetProperties(BindingFlags.Static | BindingFlags.Public)
                     .Where(x => x.PropertyType == typeof(XLColor)))
        {
            dict[property.Name.ToLower()] = (XLColor) property.GetValue(null)!;
        }

        Colors = dict;
    }
}