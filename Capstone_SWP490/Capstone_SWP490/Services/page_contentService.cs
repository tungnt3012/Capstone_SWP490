using Capstone_SWP490.Models;
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
    public class page_contentService : Ipage_contentService
    {
        private readonly Ipage_contentRepository _ipage_contentRepository = new page_contentRepository();

        public async Task<AjaxResponseViewModel<page_content>> CreatePageContent(page_content page_ContentsIn)
        {
            var output = new AjaxResponseViewModel<page_content>
            {
                Status = 0,
                Data = null
            };
            if (!String.IsNullOrEmpty(page_ContentsIn.content) && !String.IsNullOrEmpty(page_ContentsIn.title))
            {
                var data = await _ipage_contentRepository.Create(page_ContentsIn);
                if (data != null)
                {
                    output.Data = data;
                    output.Message = "Success";
                    output.Status = 1;
                    return output;
                }
            }
            output.Message = "Fail";
            return output;
        }

        public async Task<AjaxResponseViewModel<bool>> DeleteSingleContent(page_content page_ContentsIn)
        {
            var output = new AjaxResponseViewModel<bool>
            {
                Status = 0,
                Data = false
            };
            var pc = _ipage_contentRepository.FindBy(x => x.content_id == page_ContentsIn.content_id).FirstOrDefault();
            if (pc != null)
            {
                if (await _ipage_contentRepository.Delete(pc) != -1)
                {
                    output.Data = true;
                    output.Message = "Success";
                    output.Status = 1;
                    return output;
                }
            }
            output.Message = "Fail";
            return output;
        }

        public async Task<AjaxResponseViewModel<bool>> PinPageContent(page_content page_ContentsIn)
        {
            var output = new AjaxResponseViewModel<bool>
            {
                Status = 0,
                Data = false
            };
            var findContent = _ipage_contentRepository.FindBy(x => x.content_id == page_ContentsIn.content_id).FirstOrDefault();
            if (findContent != null)
            {
                var lstPageContent = (from data in _ipage_contentRepository.FindBy(x => x.page_id.Equals(page_ContentsIn.page_id)).ToList()
                          orderby data.position ascending
                          select data).ToList();

                if (lstPageContent != null)
                {
                    int pos = 1;
                    foreach (var item in lstPageContent)
                    {
                        if (item.content_id != findContent.content_id)
                        {
                            item.position = pos;
                            if (await _ipage_contentRepository.Update(item, item.content_id) == -1)
                            {
                                output.Message = "Pin content FAIL";
                                return output;
                            }
                            pos++;
                        }   
                        if(item.content_id == findContent.content_id)
                        {
                            findContent.position = 0;
                            if (await _ipage_contentRepository.Update(findContent, findContent.content_id) == -1)
                            {
                                output.Message = "Pin content FAIL";
                                return output;
                            }
                        }
                    }
                    output.Message = "success";
                    output.Data = true;
                    output.Status = 1;
                    return output;
                }
            }
            output.Message = "Fail";
            return output;
        }

        public async Task<AjaxResponseViewModel<IEnumerable<page_content>>> UpdateListPageContent(List<page_content> page_ContentsIn)
        {
            var output = new AjaxResponseViewModel<IEnumerable<page_content>>();
            var data = new List<page_content>();
            foreach (var item in page_ContentsIn)
            {
                var singleContent = _ipage_contentRepository.FindBy(x => x.content_id == item.content_id).FirstOrDefault();
                if (singleContent == null)
                {
                    output.Data = null;
                    output.Message = "Content not exist";
                    output.Status = 0;
                    return output;
                }
                singleContent.content = item.content.Trim();
                singleContent.position = item.position;
                singleContent.title = item.title.Trim();
                string s = item.title.Trim();
                if (await _ipage_contentRepository.Update(singleContent, singleContent.content_id) == -1)
                {
                    output.Data = null;
                    output.Message = "Content have Title " + singleContent.title + " update fail";
                    output.Status = 0;
                    return output;
                }
                data.Add(singleContent);
            }
            return output;
        }

        public async Task<AjaxResponseViewModel<page_content>> UpdateSingleContent(page_content page_ContentsIn)
        {
            var output = new AjaxResponseViewModel<page_content>
            {
                Data = null,
                Status = 0
            };
            var pc = _ipage_contentRepository.FindBy(x => x.content_id == page_ContentsIn.content_id).FirstOrDefault();
            if (pc != null)
            {
                pc.content = page_ContentsIn.content.Trim();
                pc.position = page_ContentsIn.position;
                pc.title = page_ContentsIn.title.Trim();
                if (await _ipage_contentRepository.Update(pc, pc.content_id) != -1)
                {
                    output.Data = pc;
                    output.Message = "Success";
                    output.Status = 1;
                    return output;
                }
            }
            output.Message = "fail";
            return output;
        }
    }
}