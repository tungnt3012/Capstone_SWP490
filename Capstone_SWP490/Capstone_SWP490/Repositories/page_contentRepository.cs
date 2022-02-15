using Capstone_SWP490.DAO;
using Capstone_SWP490.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Repositories
{
    public class page_contentRepository : GenericRepository<page_content>, Ipage_contentRepository
    {
        public List<page_content> GetPage_ContentByPageId(string stringId)
        {
            //var lstPage_Content = FindBy(x => x.page_id.Equals(stringId)).ToList();
            var rs = (from data in FindBy(x => x.page_id.Equals(stringId)).ToList()
                      orderby data.position ascending
                     select data).ToList();
            return rs;
        }
    }
}