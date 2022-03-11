using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
   public interface IcontestService
    {
        contest getByCode(string code);
        contest getByCodeOrName(string code, string name);
        contest getById(int? id);
        List<contest> getIndividualContest();
    }
}
