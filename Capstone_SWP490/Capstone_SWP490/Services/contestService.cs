using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
namespace Capstone_SWP490.Services
{
    public class contestService : IcontestService
    {
        private readonly IcontestRepository _icontestRepository = new contestRepository();

        public contest getByCode(string code)
        {
            return _icontestRepository.getByCode(code);
        }

        public contest getByCodeOrName(string code, string name)
        {
            if (code == null || name == null)
            {
                throw new Exception("Contest code and name cannot null");
            }
            return _icontestRepository.FindBy(x => x.code.Equals(code) || x.contest_name.Equals(name)).FirstOrDefault();
        }

        public contest getById(int? id)
        {
            return _icontestRepository.FindBy(x => x.contest_id == id).FirstOrDefault();
        }

        public List<contest> getIndividualContest()
        {
            return _icontestRepository.FindBy(x => x.max_contestant == 0).ToList();
        }
    }
}