using Capstone_SWP490.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
   public interface Ipage_contentService
    {
        Task<AjaxResponseViewModel<IEnumerable<page_content>>> UpdateListPageContent(List<page_content> page_ContentsIn);
        Task<AjaxResponseViewModel<page_content>> UpdateSingleContent(page_content page_ContentsIn);
        Task<AjaxResponseViewModel<bool>> DeleteSingleContent(page_content page_ContentsIn);
        Task<AjaxResponseViewModel<page_content>> CreatePageContent(page_content page_ContentsIn);
        Task<AjaxResponseViewModel<bool>> PinPageContent(page_content page_ContentsIn);
        List<page_content> GetMenuContents(string user_role);
    }
}
