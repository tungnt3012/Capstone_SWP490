using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class page_contentService  : Ipage_contentService
    {
        private readonly Ipage_contentRepository _ipage_contentRepository = new page_contentRepository();

        public Task<page_content> CreatePageContent(page_content page_ContentsIn)
        {
            if (page_ContentsIn != null)
            {
                return _ipage_contentRepository.Create(page_ContentsIn);
            }
            return null;
        }

        public async Task<bool> DeleteSingleContent(page_content page_ContentsIn)
        {
            if (page_ContentsIn != null)
            {
                var pc = _ipage_contentRepository.FindBy(x => x.content_id == page_ContentsIn.content_id).FirstOrDefault();
                if (pc != null)
                {
                    if (await _ipage_contentRepository.Delete(pc) != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> UpdateListPageContent(List<page_content> page_ContentsIn)
        {
            if (page_ContentsIn != null)
            {
                foreach (var item in page_ContentsIn)
                {
                    var singleContent = _ipage_contentRepository.FindBy(x => x.content_id == item.content_id).FirstOrDefault();
                    if (singleContent == null)
                    {
                        return false;
                    }
                    singleContent.content = item.content.Trim();
                    singleContent.position = item.position;
                    singleContent.title = item.title.Trim();
                    string s = item.title.Trim();
                    if (await _ipage_contentRepository.Update(singleContent, singleContent.content_id) == -1)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateSingleContent(page_content page_ContentsIn)
        {
            if (page_ContentsIn != null)
            {
                var pc = _ipage_contentRepository.FindBy(x => x.content_id == page_ContentsIn.content_id).FirstOrDefault();
                if (pc != null)
                {
                    pc.content = page_ContentsIn.content.Trim();
                    pc.position = page_ContentsIn.position;
                    pc.title = page_ContentsIn.title.Trim();
                    if (await _ipage_contentRepository.Update(pc, pc.content_id) != -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}