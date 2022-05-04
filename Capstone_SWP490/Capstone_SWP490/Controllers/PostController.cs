using Capstone_SWP490.Models.app_userViewModel;
using Capstone_SWP490.Models.post_ViewModel;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.IO;
using System.Threading.Tasks;
using Capstone_SWP490.Sercurity;
using Capstone_SWP490.Models;

namespace Capstone_SWP490.Controllers
{
    public class PostController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PostController));
        private readonly IpostService _postService = new postService();

        [AuthorizationAccept(Roles = "ORGANIZER")]
        // GET: Post
        public ActionResult Index(string status)
        {
            app_userViewModel logined = (app_userViewModel)Session["profile"];
            if (logined == null)
            {
                return RedirectToAction("", "Home");
            }
            List<post_TopViewModel> listPost = _postService.getByAuthorId(status);
            postList_ViewModel model = new postList_ViewModel();
            model.posts = listPost;
            model.status = status;
            return View(model);
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult Enable(int id)
        {
            try
            {
                _postService.Enable(id);
                return RedirectToAction("", "Post");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return RedirectToAction("", "Home");

            }
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult Disable(int id)
        {
            try
            {

                _postService.Disable(id);
                return RedirectToAction("", "Post");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return RedirectToAction("", "Home");

            }
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult Create()
        {
            //post_ViewModel model = new post_ViewModel();
            //return View("Edit", model);
            return View();
        }
        [AuthorizationAccept(Roles = "ORGANIZER")]
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(postUpload_ViewModel model)
        {
            if (model.actionBtn != null && model.actionBtn.Equals("Schedule"))
            {
                DateTime now = DateTime.Now;
                if (model.schedule_date == null || DateTime.Parse(model.schedule_date) <= now)
                {
                    //@ViewData["CREATE_ERROR"] = "In Case Of Schedule Post, Schedule Date Cannot be empty or earlier than Now";
                    //return View();
                    return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "In Case Of Schedule Post, Schedule Date Cannot be empty or earlier than Now", Status = 0 });
                }
                model.schedule_date = Convert.ToDateTime(model.schedule_date).ToString();
                model.enabled = false;
            }
            else
            {
                model.schedule_date = "";
                model.enabled = true;
            }

            app_userViewModel logined = (app_userViewModel)Session["profile"];
            if (model.title == null || model.title.Equals(""))
            {
                //@ViewData["CREATE_ERROR"] = "Title cannot be empty";
                ////return View("Edit", model);
                //return View();
                return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "Title cannot be empty!!!", Status = 0 });
            }
            if (model.html_content == null || model.title.Equals(""))
            {
                //@ViewData["CREATE_ERROR"] = "Content cannot be empty";
                ////return View("Edit", model);
                //return View();
                return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "Content cannot be empty!!!", Status = 0 });
            }
            model.insert_date = DateTime.Now + "";
            model.update_date = DateTime.Now + "";
            model.post_by = logined.user_id;

            if (model.imageFile != null)
            {
                model.title_image = UploadedFile(model.imageFile);
            }
            else
            {
                model.title_image = "default.png";
            }

            //if (model.featured == true)
            //{
            //    model.post_to = "0";
            //}
            //else
            //{
            //    model.post_to = "NO";
            //}

            post p = new post
            {
                title = model.title,
                enabled = model.enabled,
                featured = model.featured,
                html_content = model.html_content,
                insert_date = model.insert_date,
                update_date = model.update_date,
                schedule_date = model.schedule_date,
                content = model.content,
                post_by = model.post_by,
                post_to = model.post_to,
                short_description = model.short_description,
                title_image = model.title_image
            };

            if (await _postService.insert(p) == null)
            {
                if (model.featured == true)
                {
                    var pinPost = await _postService.PinPost(model.post_id);
                    if (pinPost.Data == false)
                    {
                        return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "Create Failed", Status = 0 });
                    }
                }
            }

            return Json(new AjaxResponseViewModel<bool> { Data = true, Message = "Add Successfull", Status = 1 });
        }
        private string UploadedFile(HttpPostedFileBase file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                uniqueFileName = GetUniqueFileName(file.FileName);
                string _path = Path.Combine(Server.MapPath("~/image/post"), uniqueFileName);
                file.SaveAs(_path);
            }
            return uniqueFileName;
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_" + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }


        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult Edit(int id)
        {
            try
            {
                //post_ViewModel model = new post_ViewModel();
                //post post = _postService.getById(id);
                //model.post = post;
                //model.action = "Edit";
                //model.featured = post.post_to == null || post.post_to.Equals("NO") ? false : true;
                //model.insert_date = post.insert_date;
                var model = _postService.getPostById(id);
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return RedirectToAction("", "Home");
            }
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(postUpload_ViewModel model)
        {
            //try
            //{
            if (model.actionBtn != null && model.actionBtn.Equals("Schedule"))
            {
                DateTime now = DateTime.Now;
                if (model.schedule_date == null || DateTime.Parse(model.schedule_date) <= now)
                {
                    //@ViewData["EDIT_ERROR"] = "In Case Of Schedule Post, Schedule Date Cannot be empty or earlier than Now";
                    //return View(model);
                    return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "In Case Of Schedule Post, Schedule Date Cannot be empty or earlier than Now", Status = 0 });
                }
                model.enabled = false;
            }
            else
            {
                model.schedule_date = "";
                model.enabled = true;
            }
            app_userViewModel logined = (app_userViewModel)Session["profile"];
            if (model.title == null || model.title.Equals(""))
            {
                //@ViewData["EDIT_ERROR"] = "Title cannot be empty";
                //return View(model);
                return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "Title cannot be empty", Status = 0 });

            }
            if (model.html_content == null || model.title.Equals(""))
            {
                //@ViewData["EDIT_ERROR"] = "Content cannot be empty";
                //return View(model);
                return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "Content cannot be empty", Status = 0 });
            }
            if (model.imageFile != null)
            {
                model.title_image = UploadedFile(model.imageFile);
            }

            model.insert_date = DateTime.Now + "";
            model.update_date = DateTime.Now + "";
            model.post_by = logined.user_id;
            model.update_date = DateTime.Now + "";
            model.featured = model.featured;

            if (model.imageFile != null)
            {
                model.title_image = UploadedFile(model.imageFile);
            }

            post p = new post
            {
                post_id = model.post_id,
                title = model.title,
                enabled = model.enabled,
                featured = model.featured,
                html_content = model.html_content,
                insert_date = model.insert_date,
                update_date = model.update_date,
                schedule_date = model.schedule_date,
                content = model.content,
                post_by = model.post_by,
                post_to = model.post_to,
                short_description = model.short_description,
                title_image = model.title_image
            };

            if (await _postService.update(p) != -1)
            {
                if (model.featured == true)
                {
                    var pinPost = await _postService.PinPost(model.post_id);
                    if (pinPost.Data == false)
                    {
                        return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "Save Fail", Status = 0 });
                    }
                }
                else
                {
                    var pinPost = await _postService.UnPinPost(model.post_id);
                    if (pinPost.Data == false)
                    {
                        return Json(new AjaxResponseViewModel<bool> { Data = false, Message = "Create Failed", Status = 0 });
                    }
                }
            }
            return Json(new AjaxResponseViewModel<bool> { Data = true, Message = "Save Successfull", Status = 1 });
            //return RedirectToAction("", "Post");
            //}
            //catch (Exception e)
            //{
            //    Log.Error(e.Message);
            //}
            //return RedirectToAction("", "Home");
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public async Task<ActionResult> Remove(int post_id)
        {
            await _postService.Delete(post_id);
            return RedirectToAction("", "Post");
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public async Task<ActionResult> PinPost(int postId)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _postService.PinPost(postId);
            return Json(ajaxResponse);
            //return RedirectToAction("", "Post");
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public async Task<ActionResult> UnPinPost(int postId)
        {
            AjaxResponseViewModel<bool> ajaxResponse = await _postService.UnPinPost(postId);
            return Json(ajaxResponse);
            //return RedirectToAction("", "Post");
        }


        [AuthorizationAccept(Roles = "ORGANIZER")]
        public async Task<ActionResult> Unpin(int postId)
        {
            await _postService.Unpin(postId);
            return RedirectToAction("", "Post");
        }
    }
}