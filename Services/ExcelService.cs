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
using TBGL.Extensions;

namespace TBGL.Services;

public sealed class ExcelService : IExcelService
{
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

    public void GenerateWorkpaper(PropertyMetadata property, TemplateModel template, Uri path)
    {
        Debug.Assert(TrialBalanceReport is not null);

        try
        {
            using var workbook = new XLWorkbook();

            var trialBalanceWorksheet = workbook.AddWorksheet("Trial Balance Report");

            trialBalanceWorksheet.FillValues(1..2, 1..4,
                "Property:", TrialBalanceReport.Property.ToString(),
                "Reporting Book:", TrialBalanceReport.ReportingBook,
                "Start Date:", TrialBalanceReport.StartDate.ToString("d"),
                "End Date:", TrialBalanceReport.EndDate.ToString("d"));

            trialBalanceWorksheet.FillValues(5, 1..6,
                "Account Number", "Account Name", $"Opening balance on {TrialBalanceReport.StartDate:d}", "Debit",
                "Credit", $"Closing balance on {TrialBalanceReport.EndDate:d}");

            var trialBalanceRow = 5;
            foreach (var account in TrialBalanceReport.Accounts)
            {
                trialBalanceRow++;

                trialBalanceWorksheet.FillValues(trialBalanceRow, 1..6,
                    $"{account.Metadata.Category}-{account.Metadata.SubCategory}",
                    account.Metadata.Name,
                    account.OpeningBalance,
                    account.Debit,
                    account.Credit,
                    account.ClosingBalance);

                trialBalanceWorksheet.Range(trialBalanceRow, 3..6).SetCurrencyFormat();
            }

            var range = trialBalanceWorksheet.Range(5, 1, trialBalanceRow, 6);
            var table = range.CreateTable("Accounts").SetShowHeaderRow(true).SetShowTotalsRow(true).SetShowAutoFilter(false);
            table.TotalsRow().Cell(1).SetValue("Totals:");
            table.Fields.Skip(1).First().TotalsRowFunction = XLTotalsRowFunction.None;
            foreach (var field in table.Fields.Skip(2))
            {
                field.TotalsRowFunction = XLTotalsRowFunction.Sum;
                field.TotalsCell.SetCurrencyFormat();
            }

            trialBalanceWorksheet.Columns().AdjustToContents();

            const int monthOffset = 2;
            foreach (var group in template.Groups)
            {
                var color = Colors.GetValueOrDefault(group.Color?.ToLower() ?? string.Empty) ?? XLColor.NoColor;
                var sheet = workbook.AddWorksheet(group.GetName(property)).SetTabColor(color);

                var accounts = group.Filter(property, GeneralLedgerReport!.TransactionHistories).ToList();

                var groupRow = 1;
                var year = TrialBalanceReport.StartDate.Year;
                foreach (var account in accounts)
                {
                    var groupRowStart = groupRow;

                    sheet.FillValues(groupRow, 1..14,
                        account.Metadata.ToString(), "Balance Forward",
                        $"January {year}", $"February {year}", $"March {year}",
                        $"April {year}", $"May {year}", $"June {year}",
                        $"July {year}", $"August {year}", $"September {year}",
                        $"October {year}", $"November {year}", $"December {year}");

                    groupRow++;
                    sheet.Cell(groupRow, 2).SetCurrencyValue(account.StartingBalance);
                    
                    foreach (var month in account.EnumerateTransactions(false).GroupBy(x => x.PostedDate!.Value.Month))
                    {
                        foreach (var transaction in month)
                        {
                            groupRow++;
                            sheet.Cell(groupRow, 1).SetTextValue(transaction.Memo);
                            // debit is +, credit is -
                            var amount = transaction.Debit ?? -transaction.Credit;
                            sheet.Cell(groupRow, monthOffset + month.Key).SetCurrencyValue(amount);
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
                        field.TotalsCell.SetCurrencyFormat();
                    }

                    groupRow += 3;
                    sheet.Cell(groupRow, 13).SetValue("G/L Balance:");
                    sheet.Cell(groupRow, 14).SetFormulaR1C1("=R[-2]C").SetCurrencyFormat();

                    groupRow++;
                    sheet.Cell(groupRow, 13).SetValue("TB Balance:");
                    sheet.Cell(groupRow, 14).SetFormulaR1C1(
                        Formula.VLookup(account.Metadata.GetNumber(), trialBalanceWorksheet.Range(6, 1, trialBalanceRow, 6), 6))
                        .SetCurrencyFormat();
                    groupRow++;
                    sheet.Cell(groupRow, 13).SetValue("Difference:");
                    sheet.Cell(groupRow, 14).SetFormulaR1C1("=ABS(R[-1]C-R[-2]C)").SetCurrencyFormat();

                    groupRow += 3;
                }

                sheet.Columns().AdjustToContents();
                sheet.Column(1).Width = 50;
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