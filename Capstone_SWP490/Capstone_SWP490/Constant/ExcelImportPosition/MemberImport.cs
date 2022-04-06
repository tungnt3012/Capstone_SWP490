using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Common.ExcelImportPosition
{
    public class MemberImport : IExcelPosition
    {
        public static string sheetName { get; } = "Team Member";
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