using System;
using ClosedXML.Excel;

namespace TBGL.Common;

public sealed class TrialBalanceLoadResult(string path, IXLWorksheet worksheet) : XLWorksheetLoadResult(path, worksheet);