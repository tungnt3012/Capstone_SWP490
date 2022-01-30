using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Common.ExcelImportPosition
{
    public class TeamImport : IExcelPosition
    {
        public int sheetPosition { get; } = 2;

        public int startAtCol { get; } = 3;
        public int startAtRow { get; } = 3;
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