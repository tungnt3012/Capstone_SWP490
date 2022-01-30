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
    }
}