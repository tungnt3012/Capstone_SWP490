using Capstone_SWP490.Models;
using Capstone_SWP490.Models.post_ViewModel;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class postService : IpostService
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(postService));
        private readonly IpostRepository _ipostRepository = new postRepository();

        public List<post_TopViewModel> getByAuthorId(int authorId, string status)
        {
            try
            {
                if (status != null)
                {
                    if (status.Equals("posted"))
                    {
                        var lstPost = _ipostRepository.FindBy(x => x.post_by == authorId && x.enabled == true).ToList();
                        var lOut = new List<post_TopViewModel>();
                        if (lstPost.Count > 0)
                        {
                            foreach (var item in lstPost)
                            {
                                lOut.Add(new post_TopViewModel
                                {
                                    post_id = item.post_id,
                                    content = item.content,
                                    enabled = item.enabled,
                                    featured = item.featured,
                                    html_content = item.html_content,
                                    insert_date = item.insert_date,
                                    post_by = item.post_by,
                                    post_to = item.post_to,
                                    schedule_date = item.schedule_date,
                                    short_description = item.short_description,
                                    title = item.title,
                                    title_image = item.title_image,
                                    update_date = item.update_date,
                                });
                            }
                        }
                        return lOut;
                    }
                    else if (status.Equals("scheduling"))
                    {
                        var lstPost = _ipostRepository.FindBy(x => x.post_by == authorId && x.enabled == false && (x.schedule_date != null && !x.schedule_date.Equals(""))).ToList();
                        var lOut = new List<post_TopViewModel>();
                        if (lstPost.Count > 0)
                        {
                            foreach (var item in lstPost)
                            {
                                lOut.Add(new post_TopViewModel
                                {
                                    post_id = item.post_id,
                                    content = item.content,
                                    enabled = item.enabled,
                                    featured = item.featured,
                                    html_content = item.html_content,
                                    insert_date = item.insert_date,
                                    post_by = item.post_by,
                                    post_to = item.post_to,
                                    schedule_date = item.schedule_date,
                                    short_description = item.short_description,
                                    title = item.title,
                                    title_image = item.title_image,
                                    update_date = item.update_date,
                                });
                            }
                        }
                        return lOut;
                    }
                }

                var lstOut = new List<post_TopViewModel>();
                var lstNoPin = (from x in _ipostRepository.FindBy(x => x.post_by == authorId && x.post_to.Equals("No"))
                                orderby x.update_date descending
                                select x).ToList();

                var lstPin = (from x in _ipostRepository.FindBy(x => x.post_by == authorId && !x.post_to.Equals("No"))
                              orderby Convert.ToInt32(x.post_to) ascending
                              select x).ToList();

                if (lstPin.Count > 0)
                {
                    foreach (var item in lstPin)
                    {
                        lstOut.Add(new post_TopViewModel
                        {
                            post_id = item.post_id,
                            content = item.content,
                            enabled = item.enabled,
                            featured = item.featured,
                            html_content = item.html_content,
                            insert_date = item.insert_date,
                            post_by = item.post_by,
                            post_to = item.post_to,
                            schedule_date = item.schedule_date,
                            short_description = item.short_description,
                            title = item.title,
                            title_image = item.title_image,
                            update_date = item.update_date,
                            isPin = 1
                        });
                    }
                }

                if (lstNoPin.Count > 0)
                {
                    foreach (var item in lstNoPin)
                    {
                        lstOut.Add(new post_TopViewModel
                        {
                            post_id = item.post_id,
                            content = item.content,
                            enabled = item.enabled,
                            featured = item.featured,
                            html_content = item.html_content,
                            insert_date = item.insert_date,
                            post_by = item.post_by,
                            post_to = item.post_to,
                            schedule_date = item.schedule_date,
                            short_description = item.short_description,
                            title = item.title,
                            title_image = item.title_image,
                            update_date = item.update_date,
                            isPin = 0
                        }); 
                    }
                }

                return lstOut;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw e;
            }
        }

        public post getById(int id)
        {
            try
            {
                return _ipostRepository.FindBy(x => x.post_id == id).FirstOrDefault();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw e;
            }
        }

        public List<post> getToScheduler()
        {
            try
            {
                DateTime now2 = DateTime.Now;
                return _ipostRepository.FindBy(x => x.enabled == false && x.schedule_date != null).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw e;
            }
        }

        public async Task<post> insert(post post)
        {
            post.post_to = "NO";
            return await _ipostRepository.Create(post);
        }

        public async Task<int> update(post post)
        {
            return await _ipostRepository.Update(post, post.post_id);
        }

        public List<post_TopViewModel> GetTop5Posts()
        {
            var postsNotPin = _ipostRepository.FindBy(x => x.featured == true && x.enabled == true && x.post_to.Equals("No"));
            var postsHasPin = _ipostRepository.FindBy(x => x.featured == true && x.enabled == true && !x.post_to.Equals("No"));
            var lstPostOut = new List<post_TopViewModel>();
            var lstPNoTemp = (from x in postsNotPin
                              orderby x.update_date ascending
                              select x).ToList();


            var lstPPinTemp = (from x in postsHasPin
                               orderby Convert.ToInt32(x.post_to) ascending
                               select x).Take(3).ToList();

            if (lstPPinTemp.Count > 0)
            {
                foreach (var x in lstPPinTemp)
                {
                    var p = new post_TopViewModel
                    {
                        post_id = x.post_id,
                        content = x.content,
                        enabled = x.enabled,
                        featured = x.featured,
                        html_content = x.html_content,
                        insert_date = x.insert_date,
                        post_by = x.post_by,
                        post_to = x.post_to,
                        schedule_date = x.schedule_date,
                        short_description = x.short_description,
                        title = x.title,
                        update_date = x.update_date,
                        title_image = x.title_image,
                        isPin = 1
                    };
                    lstPostOut.Add(p);
                }
            }

            if (lstPNoTemp.Count > 0)
            {
                foreach (var x in lstPNoTemp)
                {
                    var p = new post_TopViewModel
                    {
                        post_id = x.post_id,
                        content = x.content,
                        enabled = x.enabled,
                        featured = x.featured,
                        html_content = x.html_content,
                        insert_date = x.insert_date,
                        post_by = x.post_by,
                        post_to = x.post_to,
                        schedule_date = x.schedule_date,
                        short_description = x.short_description,
                        title = x.title,
                        update_date = x.update_date,
                        title_image = x.title_image,
                        isPin = 0
                    };
                    lstPostOut.Add(p);
                }
            }

            return lstPostOut;
        }

        public async Task<int> Delete(int postId)
        {
            post post = _ipostRepository.FindBy(x => x.post_id == postId).FirstOrDefault();
            if (post != null)
                return await _ipostRepository.Delete(post);

            return -1;
        }
        public async Task<AjaxResponseViewModel<bool>> PinPost(int postId)
        {
            var postTemp = _ipostRepository.FindBy(x => x.post_id == postId).FirstOrDefault();
            var listPostPin = (from x in _ipostRepository.FindBy(x => x.post_to != null)
                               orderby Convert.ToInt32(x.post_to) ascending
                               select x).ToList();
            
            if (!String.IsNullOrWhiteSpace(postTemp.post_to))
            {
                if (listPostPin.Count > 0)
                {
                    int pos = 1;
                    foreach (var item in listPostPin)
                    {
                        if (item.post_id == postTemp.post_id)
                        {
                            postTemp.post_to = 0 + "";
                            if (await _ipostRepository.Update(postTemp, postTemp.post_id) == -1)
                            {
                                return new AjaxResponseViewModel<bool> { Data = false, Message = "Failed", Status = 0 };
                            }
                        }
                        else
                        {
                            item.post_to = pos + "";
                            if (await _ipostRepository.Update(item, item.post_id) == -1)
                            {
                                return new AjaxResponseViewModel<bool> { Data = false, Message = "Failed", Status = 0 };
                            }
                            pos++;
                        }
                    }
                }
                else
                {
                    postTemp.post_to = 0 + "";
                    if (await _ipostRepository.Update(postTemp, postTemp.post_id) == -1)
                    {
                        return new AjaxResponseViewModel<bool> { Data = false, Message = "Failed", Status = 0 };
                    }
                }

                if (listPostPin.Count > 3)
                {
                    for (int i = 2; i < listPostPin.Count; i++)
                    {
                        var p = listPostPin[i];
                        p.post_to = "NO";
                        if (await _ipostRepository.Update(p, p.post_id) == -1)
                        {
                            return new AjaxResponseViewModel<bool> { Data = false, Message = "Failed", Status = 0 };
                        }
                    }
                }
                return new AjaxResponseViewModel<bool> { Data = true, Message = "Success", Status = 1 };
            }
            else
            {
                int pos = 1;
                postTemp.post_to = 0 + "";
                if (await _ipostRepository.Update(postTemp, postTemp.post_id) == -1)
                {
                    return new AjaxResponseViewModel<bool> { Data = false, Message = "Failed", Status = 0 };
                }
                foreach (var item in listPostPin)
                {
                    item.post_to = pos + "";
                    if (await _ipostRepository.Update(item, item.post_id) == -1)
                    {
                        return new AjaxResponseViewModel<bool> { Data = false, Message = "Failed", Status = 0 };
                    }
                    pos++;
                }

                if (listPostPin.Count > 3)
                {
                    for (int i = 2; i < listPostPin.Count; i++)
                    {
                        var p = listPostPin[i];
                        p.post_to = "NO";
                        if (await _ipostRepository.Update(p, p.post_id) == -1)
                        {
                            return new AjaxResponseViewModel<bool> { Data = false, Message = "Failed", Status = 0 };
                        }
                    }
                }
                return new AjaxResponseViewModel<bool> { Data = true, Message = "Success", Status = 1 };
            }
        }

        public List<post_TopViewModel> GetTopAllPosts()
        {
            var posts = _ipostRepository.FindBy(x => x.featured == true);
            var lstPostOut = new List<post_TopViewModel>();
            var lstPTemp = (from x in posts
                            orderby x.schedule_date descending
                            select x).ToList();
            foreach (var x in lstPTemp)
            {
                var p = new post_TopViewModel
                {
                    post_id = x.post_id,
                    content = x.content,
                    enabled = x.enabled,
                    featured = x.featured,
                    html_content = x.html_content,
                    insert_date = x.insert_date,
                    post_by = x.post_by,
                    post_to = x.post_to,
                    schedule_date = x.schedule_date,
                    short_description = x.short_description,
                    title = x.title,
                    update_date = x.update_date,
                    title_image = x.title_image
                };
                lstPostOut.Add(p);
            }
            return lstPostOut;
        }
    }
}