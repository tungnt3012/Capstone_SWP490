﻿using Capstone_SWP490.Models.app_userViewModel;
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

namespace Capstone_SWP490.Controllers
{
    public class PostController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PostController));
        private readonly IpostService _postService = new postService();


        // GET: Post
        public ActionResult Index(string status)
        {
            app_userViewModel logined = (app_userViewModel)Session["profile"];
            if (logined == null)
            {
                return RedirectToAction("", "Home");
            }
            List<post> listPost = _postService.getByAuthorId(logined.user_id, status);
            postList_ViewModel model = new postList_ViewModel();
            model.posts = listPost;
            model.status = status;
            return View(model);
        }

        public ActionResult Enable(int id)
        {
            try
            {
                post _post = _postService.getById(id);
                _post.enabled = true;
                _post.schedule_date = null;
                _post.update_date = DateTime.Now + "";
                _postService.update(_post);
                return RedirectToAction("", "Post");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return RedirectToAction("", "Home");

            }
        }

        public ActionResult Disable(int id)
        {
            try
            {
                post _post = _postService.getById(id);
                _post.enabled = false;
                _post.schedule_date = null;
                _post.update_date = DateTime.Now + "";
                _postService.update(_post);
                return RedirectToAction("", "Post");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return RedirectToAction("", "Home");

            }
        }
        public ActionResult Create()
        {
            post_ViewModel model = new post_ViewModel();
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(post_ViewModel model, string postToFanPage)
        {
            app_userViewModel logined = (app_userViewModel)Session["profile"];
            if (postToFanPage != null && postToFanPage.Equals("on"))
            {
                model.post.post_to += ",FANPAGE";
                DateTime now = DateTime.Now;
                if (model.post.schedule_date == null || DateTime.Parse(model.post.schedule_date) <= now)
                {
                    @ViewData["EDIT_ERROR"] = "In Case Of Schedule Post, Schedule Date Cannot be empty or earlier than Now";
                    return View("Edit", model);
                }
            }
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
            model.post.enabled = false;
            _postService.insert(model.post);
            return RedirectToAction("", "Post");
        }
        public ActionResult Edit(int id)
        {
            try
            {
                post_ViewModel model = new post_ViewModel();
                post post = _postService.getById(id);
                model.post = post;
                model.action = "Edit";
                model.featured = post.featured == null ? false : post.featured.Value;
                model.insert_date = post.insert_date;
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return RedirectToAction("", "Home");
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(post_ViewModel model, string postToFanPage, HttpPostedFileBase file, string action)
        {
            try
            {
                if (action != null && action.Equals("Schedule"))
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
                if (postToFanPage != null && postToFanPage.Equals("on"))
                {
                    model.post.post_to += ",FANPAGE";
                }
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
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/images/profile"), pic);
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

                }
                model.post.insert_date = DateTime.Now + "";
                model.post.update_date = DateTime.Now + "";
                model.post.post_by = logined.user_id;
                model.post.update_date = DateTime.Now + "";
                model.post.featured = model.featured;
                _postService.update(model.post);
                return RedirectToAction("", "Post");
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return RedirectToAction("", "Home");
        }

    }
}