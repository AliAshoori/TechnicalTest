﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechnicalTest.Server;
using TechnicalTest.Server.Services;
using TechnicalTest.Shared;

namespace TechnicalTest.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ReportValueCellsCalculatorTests
    {
        MemoryStream _memoryStream;
        ExcelPackage _excelPackage;

        [TestCleanup]
        public void Cleanup()
        {
            if (_memoryStream != null)
                _memoryStream.Dispose();

            if (_excelPackage != null)
                _excelPackage.Dispose();
        }

        [TestMethod]
        public async Task Calculate_HappyScenario_ReturnsReportValueCells()
        {
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculator>>();
            var mockValidator = new Mock<IReportValueCellsCalculatorValidator>();

            var calculator = new ReportValueCellsCalculator(mockLogger.Object, mockValidator.Object);

            var expected = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Value = "010", Row  = 10, Column = 5
                },
                new ReportValueCell
                {
                    Value = "011", Row  = 10, Column = 6
                },
                new ReportValueCell
                {
                    Value = "012", Row  = 10, Column = 7
                },
                new ReportValueCell
                {
                    Value = "022", Row  = 10, Column = 8
                },
                new ReportValueCell
                {
                    Value = "025", Row  = 10, Column = 9
                },
                new ReportValueCell
                {
                    Value = "031", Row  = 10, Column = 10
                },
                new ReportValueCell
                {
                    Value = "040", Row  = 10, Column = 11
                },
                new ReportValueCell
                {
                    Value = "010", Row  = 11, Column = 2
                },
                new ReportValueCell
                {
                    Value = "020", Row  = 12, Column = 2
                }
            };

            var actual = Enumerable.Empty<ReportValueCell>();

            var file = await File.ReadAllBytesAsync("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "F 20.04");

            // Act
            actual = calculator.Calculate(worksheet);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task Calculate_WithRowIndexesOnTheRight_ReturnsReportValueCells()
        {
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculator>>();
            var mockValidator = new Mock<IReportValueCellsCalculatorValidator>();

            var calculator = new ReportValueCellsCalculator(mockLogger.Object, mockValidator.Object);

            var expected = new List<ReportValueCell>
            {
                new ReportValueCell
                {
                    Value = "010", Row  = 10, Column = 5
                },
                new ReportValueCell
                {
                    Value = "011", Row  = 10, Column = 6
                },
                new ReportValueCell
                {
                    Value = "012", Row  = 10, Column = 7
                },
                new ReportValueCell
                {
                    Value = "022", Row  = 10, Column = 8
                },
                new ReportValueCell
                {
                    Value = "025", Row  = 10, Column = 9
                },
                new ReportValueCell
                {
                    Value = "031", Row  = 10, Column = 10
                },
                new ReportValueCell
                {
                    Value = "040", Row  = 10, Column = 11
                },
                new ReportValueCell
                {
                    Value = "010", Row  = 11, Column = 12
                },
                new ReportValueCell
                {
                    Value = "020", Row  = 12, Column = 12
                }
            };

            var actual = Enumerable.Empty<ReportValueCell>();

            var file = await File.ReadAllBytesAsync("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "WithRowsIndexOnTheRight");

            // Act
            actual = calculator.Calculate(worksheet);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public async Task Calculate_WithNoUserDefinedIndexCellsFound_ThrowsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculator>>();
            var mockValidator = new Mock<IReportValueCellsCalculatorValidator>();

            var calculator = new ReportValueCellsCalculator(mockLogger.Object, mockValidator.Object);

            var expected = Enumerable.Empty<ReportValueCell>();
            var actual = Enumerable.Empty<ReportValueCell>();

            var file = await File.ReadAllBytesAsync("Data/ExcelReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            _memoryStream = new MemoryStream(file);
            _excelPackage = new ExcelPackage(_memoryStream);
            var worksheet = _excelPackage.Workbook?.Worksheets.FirstOrDefault(w => w.Name == "WithNoUserDefinedIndex");

            // Act
            actual = calculator.Calculate(worksheet);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [TestMethod]
        public void ReportValueCellsCalculator_WithNullLogger_ThrowsArgumentNullException()
        {
            // Arrange
            ILogger<ReportValueCellsCalculator> logger = null;
            var mockValidator = new Mock<IReportValueCellsCalculatorValidator>();

            // Act
            Action Init = () => new ReportValueCellsCalculator(logger, mockValidator.Object);

            // Assert
            Init.Should().ThrowExactly<ArgumentNullException>(nameof(logger));
        }

        [TestMethod]
        public void ReportValueCellsCalculator_WithNullValidator_ThrowsArgumentNullException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReportValueCellsCalculator>>();
            IReportValueCellsCalculatorValidator validator = null;

            // Act
            Action Init = () => new ReportValueCellsCalculator(mockLogger.Object, validator);

            // Assert
            Init.Should().ThrowExactly<ArgumentNullException>(nameof(validator));
        }
    }
}