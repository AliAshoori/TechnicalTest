﻿using Microsoft.Extensions.Logging;
using TechnicalTest.Shared;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TechnicalTest.Server.Services
{
    public interface IReportValuesToExcelSheetMerger
    {
        Task MergeAsync(ReportMergePayload mergePayload);
    }

    public class ReportValuesToExcelSheetMerger : IReportValuesToExcelSheetMerger
    {
        private readonly ILogger<ReportValuesToExcelSheetMerger> _logger;
        private readonly IReportValuesToExcelSheetMergerValidator _validator;

        public ReportValuesToExcelSheetMerger(
            ILogger<ReportValuesToExcelSheetMerger> logger,
            IReportValuesToExcelSheetMergerValidator validator)
        {
            _logger = logger.NotNull();
            _validator = validator.NotNull();
        }

        public async Task MergeAsync(ReportMergePayload mergePayload)
        {
            _validator.Validate(mergePayload);

            try
            {
                _logger.LogInformation($"Merging the report value results into the excel sheet. ReportValues: {mergePayload.ReportValues.Count()}, Target Cells: {mergePayload.Cells.Count()}");

                IEnumerable<ReportValueCell> targetRows = mergePayload.Cells.GroupBy(t => t.Row).Where(r => r.Count() == 1).SelectMany(item => item.ToArray());
                IEnumerable<ReportValueCell> targetColumns = mergePayload.Cells.GroupBy(t => t.Column).Where(c => c.Count() == 1).SelectMany(item => item.ToArray());

                _logger.LogInformation($"Found {targetRows.Count()} rows with {targetColumns.Count()} columns");

                foreach (var item in mergePayload.ReportValues)
                {
                    var cell1 = targetRows.Single(tr => int.Parse(tr.Value) == item.Row);
                    var cell2 = targetColumns.Single(tc => int.Parse(tc.Value) == item.Column);

                    var cellWithBiggerRow = cell1.Row > cell2.Row ? cell1 : cell2;

                    var reportValueCell = new ReportValueCell();

                    if (cell1.Row > cell2.Row && cell1.Column > cell2.Column) // if cell1 with bigger row has also the bigger column
                    {
                        reportValueCell.Row = cell1.Row;
                        reportValueCell.Column = cell2.Column;
                    }
                    else
                    {
                        reportValueCell.Row = Math.Max(cell1.Row, cell2.Row);
                        reportValueCell.Column = Math.Max(cell1.Column, cell2.Column);
                    }

                    reportValueCell.Value = item.Value == 0 ? "0" : item.Value.ToString("#,###");

                    mergePayload.WorkSheet.Cells[reportValueCell.Row, reportValueCell.Column].Value = reportValueCell.Value;
                }

                _logger.LogInformation($"Now writing down the merge result into excel sheet");

                await mergePayload.Package.SaveAsAsync(new FileInfo("D:\\TestReport.xlsx"));
            }
            catch (Exception exception)
            {
                _logger.LogError($"{exception}");
                throw;
            }
        }
    }
}