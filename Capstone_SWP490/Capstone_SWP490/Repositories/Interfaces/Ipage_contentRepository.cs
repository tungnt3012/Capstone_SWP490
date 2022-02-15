using Capstone_SWP490.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Repositories.Interfaces
{
   public interface Ipage_contentRepository : IGenericRepository<page_content>
    {
        List<page_content> GetPage_ContentByPageId(string stringId);
    }
}
