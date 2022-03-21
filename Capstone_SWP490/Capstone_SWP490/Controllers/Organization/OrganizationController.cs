using Capstone_SWP490.Constant.Const;
using Capstone_SWP490.Helper;
using Capstone_SWP490.Models.organization_ViewModel;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using interfaces = Capstone_SWP490.Services.Interfaces;
using services = Capstone_SWP490.Services;

namespace Capstone_SWP490.Controllers.Orgnazition
{
    public class OrgnazationController : Controller
    {
        private static readonly int PAGE_SIZE = 10;
        private readonly Iapp_userService _iapp_UserService = new app_userService();
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private static readonly ILog Log = LogManager.GetLogger(typeof(OrgnazationController));
        // GET: Orgnazation
        public ActionResult CoachAccount(organization_ListViewModel model, int? pageIndex)
        {
            try
            {
                if (pageIndex == null || pageIndex == 0)
                {
                    pageIndex = 1;
                }
                if (model.status == null || model.Equals(""))
                {
                    model.status = "All";
                }
                organization_ViewModel itemModel;
                List<organization_ViewModel> data = new List<organization_ViewModel>();
                List<app_user> coachUser = _iapp_UserService.findCoach(model.status,model.keyword);
                foreach (var item in coachUser)
                {
                    itemModel = new organization_ViewModel();
                    itemModel.coach_id = item.user_id;
                    itemModel.full_name = item.full_name;
                    itemModel.email = item.email;
                    itemModel.status = item.active;
                    school school = _ischoolService.findByCoachId(item.user_id).FirstOrDefault();
                    if (school != null)
                    {
                        itemModel.school_name = school.school_name;
                        itemModel.institution_name = school.institution_name;
                        itemModel.school_phone = school.phone_number;
                        itemModel.school_address = school.address;
                    }
                    data.Add(itemModel);
                }
                model.total_data = data.Count;
                int totalPage = (model.total_data % PAGE_SIZE != 0 ? (model.total_data / PAGE_SIZE + 1) : model.total_data / PAGE_SIZE);
                model.total_page = totalPage;
                model.pageIndex = (int)pageIndex;
                int index = ((int)pageIndex - 1) * PAGE_SIZE;
                int size = data.Count - index;
                try
                {
                    model.data = data.Skip(index)
                    .Take(PAGE_SIZE).ToList();
                }
                catch
                {
                    model.data = data.Skip(index)
                     .Take(PAGE_SIZE).ToList();
                }
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return RedirectToAction("", ACTION_CONST.Home.CONTROLLER);
            }
        }

        public ActionResult enable(int user_id)
        {
            try
            {
                app_user updated = _iapp_UserService.getByUserId(user_id);
                updated.active = true;
                _iapp_UserService.update(updated);
                new MailHelper().sendMailAfterConfirm(updated);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return RedirectToAction(ACTION_CONST.Orgnazation.COACH_ACCOUNT, ACTION_CONST.Orgnazation.CONTROLLER);
        }

        public ActionResult disable(int user_id)
        {
            try
            {
                app_user updated = _iapp_UserService.getByUserId(user_id);
                updated.active = false;
                _iapp_UserService.update(updated);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return RedirectToAction(ACTION_CONST.Orgnazation.COACH_ACCOUNT, ACTION_CONST.Orgnazation.CONTROLLER);
        }
    }
}