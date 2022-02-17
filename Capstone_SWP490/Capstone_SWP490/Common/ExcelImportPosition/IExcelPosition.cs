using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Common.ExcelImportPosition
{
     interface IExcelPosition
    {
        int[,] GetPosition();
        int getStartAtCol();
        int getStartAtRow();
    }
}
