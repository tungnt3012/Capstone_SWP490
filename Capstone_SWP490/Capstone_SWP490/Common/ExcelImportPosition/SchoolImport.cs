using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Common.ExcelImportPosition
{
    public class SchoolImport : IExcelPosition
    {
    public int sheetPosition { get;} = 2;

        public int[,] GetPosition()
        {
                int[,] positions = new int[8, 2];
                //name
                positions[0, 0] = 3;
                positions[0, 1] = 2;
                //address
                positions[1, 0] = 3;
                positions[1, 1] = 3;
                //leader name
                positions[2, 0] = 3;
                positions[2, 1] = 7;
                //leader phone
                positions[3, 0] = 5;
                positions[3, 1] = 7;
                //leader email
                positions[4, 0] = 7;
                positions[4, 1] = 7;
                //vice leader name
                positions[5, 0] = 3;
                positions[5, 1] = 8;
                //vice leader phone
                positions[6, 0] = 5;
                positions[6, 1] = 8;
                //vice leaeder mail
                positions[7, 0] = 7;
                positions[7, 1] = 8;

                return positions;
        }
    }
}