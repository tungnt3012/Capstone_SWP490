using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Common.ExcelImportPosition
{
    public class SchoolImport : IExcelPosition
    {
        public static string sheetName { get; } = "Uni_Ins";

        public int startAtCol { get; } = 3;
        public int startAtRow { get; } = 1;
        public int[,] GetPosition()
        {
            int[,] positions = new int[10, 2];
            //name
            positions[0, 0] = 1;
            positions[0, 1] = 3;
            // Institution name
            positions[1, 0] = 2;
            positions[1, 1] = 3;

            // Rector name
            positions[2, 0] = 3;
            positions[2, 1] = 3;
            //phone
            positions[3, 0] = 4;
            positions[3, 1] = 3;
            //website
            positions[4, 0] = 5;
            positions[4, 1] = 3;

            //coach name
            positions[5, 0] = 6;
            positions[5, 1] = 3;

            //coach email
            positions[6, 0] = 7;
            positions[6, 1] = 3;

            //coach phone
            positions[7, 0] = 8;
            positions[7, 1] = 3;

            //vice coach name
            positions[8, 0] = 9;
            positions[8, 1] = 3;


            //vice coach email
            positions[9, 0] = 10;
            positions[9, 1] = 3;

            //vice coach phone
            positions[10, 0] = 11;
            positions[10, 1] = 3;

            return positions;
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