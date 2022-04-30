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
            post_ViewModel model = new post_ViewModel();
            return View("Edit", model);
        }
        [AuthorizationAccept(Roles = "ORGANIZER")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(post_ViewModel model, string postToFanPage, HttpPostedFileBase file, string actionBtn)
        {
            if (actionBtn != null && actionBtn.Equals("Schedule"))
            {
                DateTime now = DateTime.Now;
                if (model.post.schedule_date == null || DateTime.Parse(model.post.schedule_date) <= now)
                {
                    @ViewData["EDIT_ERROR"] = "In Case Of Schedule Post, Schedule Date Cannot be empty or earlier than Now";
                    return View("Edit", model);
                }
                model.post.enabled = false;
            }
            else
            {
                model.post.schedule_date = "";
                model.post.enabled = true;
            }

            app_userViewModel logined = (app_userViewModel)Session["profile"];
            if (model.post.title == null || model.post.title.Equals(""))
            {
                @ViewData["EDIT_ERROR"] = "Title cannot be empty";
                return View("Edit", model);
            }
            if (model.post.html_content == null || model.post.title.Equals(""))
            {
                @ViewData["EDIT_ERROR"] = "Content cannot be empty";
                return View("Edit", model);
            }
            model.post.insert_date = DateTime.Now + "";
            model.post.update_date = DateTime.Now + "";
            model.post.post_by = logined.user_id;
            if (model.featured == true)
            {
                model.post.post_to = "0";
            }
            else
            {
                model.post.post_to = "NO";
            }
            _postService.insert(model.post);
            return RedirectToAction("Index", "Post");
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult Edit(int id)
        {
            try
            {
                post_ViewModel model = new post_ViewModel();
                post post = _postService.getById(id);
                model.post = post;
                model.action = "Edit";
                model.featured = post.post_to == null || post.post_to.Equals("NO") ? false : true;
                model.insert_date = post.insert_date;
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
        public ActionResult Edit(post_ViewModel model, string postToFanPage, HttpPostedFileBase file, string actionBtn)
        {
            try
            {
                if (actionBtn != null && actionBtn.Equals("Schedule"))
                {
                    DateTime now = DateTime.Now;
                    if (model.post.schedule_date == null || DateTime.Parse(model.post.schedule_date) <= now)
                    {
                        @ViewData["EDIT_ERROR"] = "In Case Of Schedule Post, Schedule Date Cannot be empty or earlier than Now";
                        return View(model);
                    }
                    model.post.enabled = false;
                }
                else
                {
                    model.post.schedule_date = "";
                    model.post.enabled = true;
                }
                app_userViewModel logined = (app_userViewModel)Session["profile"];
                if (model.post.title == null || model.post.title.Equals(""))
                {
                    @ViewData["EDIT_ERROR"] = "Title cannot be empty";
                    return View(model);
                }
                if (model.post.html_content == null || model.post.title.Equals(""))
                {
                    @ViewData["EDIT_ERROR"] = "Content cannot be empty";
                    return View(model);
                }
                if (file != null)
                {
                    string pic = DateTime.Now.ToFileTime() + Path.GetExtension(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/image/post"), pic);
                    // file is uploaded
                    file.SaveAs(path);

                    // save the image path path to the database or you can send image 
                    // directly to database
                    // in-case if you want to store byte[] ie. for DB
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                    model.post.title_image = pic;
                }
                model.post.insert_date = DateTime.Now + "";
                model.post.update_date = DateTime.Now + "";
                model.post.post_by = logined.user_id;
                model.post.update_date = DateTime.Now + "";
                if (model.featured == true)
                {
                    model.post.post_to = "0";
                }
                else
                {
                    model.post.post_to = "NO";
                }
                _postService.update(model.post);
                return RedirectToAction("", "Post");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return RedirectToAction("", "Home");
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
        public async Task<ActionResult> Unpin(int postId)
        {
            await _postService.Unpin(postId);
            return RedirectToAction("", "Post");
        }
    }
}