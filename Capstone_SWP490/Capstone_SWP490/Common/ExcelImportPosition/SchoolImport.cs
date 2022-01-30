using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Common.ExcelImportPosition
{
    public class SchoolImport : IExcelPosition
    {
    public int sheetPosition { get;} = 1;

        public int[,] GetPosition()
        {
                int[,] positions = new int[8, 2];
                //name
                positions[0, 0] = 2;
                positions[0, 1] = 3;
                //address
                positions[1, 0] = 3;
                positions[1, 1] = 3;
                //leader name
                positions[2, 0] = 7;
                positions[2, 1] = 3;
                //leader phone
                positions[3, 0] = 7;
                positions[3, 1] = 6;
                //leader email
                positions[4, 0] = 7;
                positions[4, 1] = 9;
                //vice leader name
                positions[5, 0] = 8;
                positions[5, 1] = 3;
                //vice leader phone
                positions[6, 0] = 8;
                positions[6, 1] = 6;
                //vice leaeder mail
                positions[7, 0] = 8;
                positions[7, 1] = 9;

            return positions;
        }

        public int getStartAtCol()
        {
            throw new NotImplementedException();
        }

        public int getStartAtRow()
        {
            throw new NotImplementedException();
        }
    }
}