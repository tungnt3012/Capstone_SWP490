using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Common.ExcelImportPosition
{
    public class TeamImport : IExcelPosition
    {
        public static string sheetName { get; } = "Team";

        public int startAtCol { get; } = 3;
        public int startAtRow { get; } = 4;
        public int[,] GetPosition()
        {
            throw new NotImplementedException();
        }

        public int getStartAtCol()
        {
            return startAtCol;
        }

        public int getStartAtRow()
        {
            return startAtRow;
        }
    }
}