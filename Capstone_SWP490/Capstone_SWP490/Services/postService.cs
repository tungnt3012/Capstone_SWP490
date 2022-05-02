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

        public List<post_TopViewModel> getByAuthorId(string status)
        {
            try
            {
                if (status != null)
                {
                    if (status.Equals("posted"))
                    {
                        var lstPost = _ipostRepository.FindBy(x => x.enabled == true).ToList();
                        var lOut = new List<post_TopViewModel>();
                        if (lstPost.Count > 0)
                        {
                            lstPost.OrderBy(x => x.update_date);
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
                        var lstPost = _ipostRepository.FindBy(x => x.enabled == false && (x.schedule_date != null && !x.schedule_date.Equals(""))).ToList();
                        var lOut = new List<post_TopViewModel>();
                        lstPost.OrderBy(x => x.insert_date);
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
                List<post> postsNotPin = _ipostRepository.FindBy(x => x.post_to.Equals("NO")).OrderByDescending(x => x.update_date).ToList();
                List<post> postsHasPin = _ipostRepository.FindBy(x => !x.post_to.Equals("NO")).OrderBy(x => x.post_to).ToList();
                if (postsNotPin.Count > 0)
                {
                    foreach (var x in postsHasPin)
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
                        lstOut.Add(p);
                    }
                }

                foreach (var x in postsNotPin)
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
                    lstOut.Add(p);

                }

                return lstOut;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return new List<post_TopViewModel>();
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
            try
            {
                //pin this creatation, no schedule
                if (!post.post_to.Equals("NO") && post.enabled == true)
                {
                    var lastPin = _ipostRepository.FindBy(x => !x.post_to.Equals("2")).FirstOrDefault();
                    if (lastPin != null)
                    {
                        lastPin.post_to = "NO";
                        await update(lastPin);
                    }
                    var pinned = _ipostRepository.FindBy(x => !x.post_to.Equals("NO")).OrderBy(x => x.post_to).ToList();
                    int i = 0;
                    foreach (var item in pinned)
                    {
                        item.post_to = ++i + "";
                        await update(item);
                    }
                }
                return await _ipostRepository.Create(post);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return null;
        }

        public async Task<int> update(post post)
        {
            return await _ipostRepository.Update(post, post.post_id);
        }

        public List<post_TopViewModel> GetTop5Posts()
        {
            var postsNotPin = _ipostRepository.FindBy(x => x.enabled == true && x.post_to.Equals("NO")).OrderByDescending(x => x.update_date);
            var postsHasPin = _ipostRepository.FindBy(x => x.enabled == true && !x.post_to.Equals("NO")).OrderBy(x => x.post_to);
            var lstPostOut = new List<post_TopViewModel>();
            if (postsNotPin.Count() > 0)
            {
                foreach (var x in postsHasPin)
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

            if (lstPostOut.Count < 5)
            {
                int i = 5 - lstPostOut.Count;
                foreach (var x in postsNotPin)
                {
                    if (i == 0)
                    {
                        break;
                    }
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
                    i--;
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
            try
            {
                var pin0 = _ipostRepository.FindBy(x => x.post_to.Equals("0")).FirstOrDefault();
                var pin1 = _ipostRepository.FindBy(x => x.post_to.Equals("1")).FirstOrDefault();
                var pin2 = _ipostRepository.FindBy(x => x.post_to.Equals("2")).FirstOrDefault();
                if (pin0 != null)
                {
                    pin0.post_to = "1";
                    await update(pin0);
                }
                if (pin1 != null)
                {
                    pin1.post_to = "2";
                    await update(pin1);
                }
                if (pin2 != null)
                {
                    pin2.post_to = "NO";
                    await update(pin2);
                }

                var postTemp = _ipostRepository.FindBy(x => x.post_id == postId).FirstOrDefault();
                postTemp.enabled = true;
                postTemp.post_to = "0";
                postTemp.update_date = DateTime.Now + "";
                await update(postTemp);
                return new AjaxResponseViewModel<bool> { Data = true, Message = "Success", Status = 1 };
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new AjaxResponseViewModel<bool> { Data = false, Message = "Failed", Status = 0 };
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

        public async Task<int> Disable(int id)
        {
            post _post = _ipostRepository.FindBy(x => x.post_id == id).FirstOrDefault();
            bool isPin = _post.post_to != null || !_post.post_to.Equals("NO");
            _post.enabled = false;
            _post.schedule_date = null;
            _post.update_date = DateTime.Now + "";
            _post.post_to = "NO";
            await update(_post);
            if (isPin)
            {
                var pinned = _ipostRepository.FindBy(x => !x.post_to.Equals("NO")).OrderBy(x => x.post_to).ToList();
                int i = 0;
                foreach (var item in pinned)
                {
                    item.post_to = i++ + "";
                    await update(item);
                }
            }
            return 1;
        }
        public async Task<int> Enable(int id)
        {
            post _post = _ipostRepository.FindBy(x => x.post_id == id).FirstOrDefault();
            if (_post != null)
            {
                _post.enabled = true;
                _post.schedule_date = null;
                _post.update_date = DateTime.Now + "";
                return await update(_post);
            }
            return -1;
        }

        public async Task<int> UpdatePost(post post)
        {
            //pin this creatation, no schedule
            if (!post.post_to.Equals("NO") && post.enabled == true)
            {
                List<post> pinned = _ipostRepository.FindBy(x => !x.post_to.Equals("NO")).OrderBy(x => x.post_to).ToList();
                for (int i = 0; i < pinned.Count() - 1; i++)
                {
                    post pinnedNew = pinned.ElementAt(i).post_to = i + 1 + "";
                    await update(pinnedNew);
                }
                await update(pinned.ElementAt(pinned.Count() - 1).post_to = "NO");
            }
            return await update(post);
        }

        public async Task<int> Unpin(int id)
        {
            post _post = _ipostRepository.FindBy(x => x.post_id == id).FirstOrDefault();
            if (_post != null)
            {
                _post.enabled = true;
                _post.schedule_date = null;
                _post.update_date = DateTime.Now + "";
                _post.post_to = "NO";
                await update(_post);
                List<post> pinned = _ipostRepository.FindBy(x => !x.post_to.Equals("NO")).OrderBy(x => x.post_to).ToList();
                int i = 0;
                foreach (var item in pinned)
                {
                    item.post_to = i++ + "";
                    await update(item);
                }
            }
            return -1;
        }

        public async Task<int> UpdateScheduler()
        {
            List<post> posts = getToScheduler();
            int i = 0;
            foreach (var item in posts)
            {
                long now = DateTime.Now.Millisecond;
                long scheduleTime2 = DateTime.Parse(item.schedule_date.ToString()).Millisecond;
                if ((scheduleTime2 + 5 * 60000) >= now)
                {
                    if (item.featured == true)
                    {
                        item.post_to = "0";
                    }
                    item.update_date = DateTime.Now + "";
                    item.enabled = true;
                    item.schedule_date = null;
                    await update(item);
                    i++;
                }
            }
            return i;

        }
    }
}