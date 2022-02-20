using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
   public interface Ipage_contentService
    {
        Task<bool> UpdateListPageContent(List<page_content> page_Contents);
        Task<bool> UpdateSingleContent(page_content page_Contents);
        Task<bool> DeleteSingleContent(page_content page_Contents);
        Task<page_content> CreatePageContent(page_content page_Contents);
    }
}
