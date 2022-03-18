﻿using Capstone_SWP490.Models;
using Capstone_SWP490.Models.contestViewModel;
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
        List<contestViewModel> GetContests();
        Task<contestViewModel> UpdateContest(contestViewModel contestIn);
        Task<contestViewModel> CreateContest(contestViewModel contestIn);
        Task<bool> DeleteContest(int id);
        IEnumerable<contestViewModel> GetAllContestAvailale();
        contestViewModel GetContestById(int id);
        AjaxResponseViewModel<List<contestViewModel>> FilterContest(string keyFilter);
    }
}
