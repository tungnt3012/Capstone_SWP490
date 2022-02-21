using Capstone_SWP490.Common.Const;
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
        public ActionResult CoachAccount(string status, int? pageIndex)
        {
            try
            {
                if (pageIndex == null || pageIndex == 0)
                {
                    pageIndex = 1;
                }
                if (status == null || status.Equals(""))
                {
                    status = "all";
                }
                organization_ViewModel itemModel;
                organization_ListViewModel model = new organization_ListViewModel();
                List<organization_ViewModel> data = new List<organization_ViewModel>();
                List<app_user> coachUser = _iapp_UserService.findCoach(status);
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
                model.status = status;
                model.total_data = data.Count;
                model.total_page = model.total_data / PAGE_SIZE;
                model.pageIndex = (int)pageIndex;
                try
                {
                    model.data = data.GetRange(((int)pageIndex - 1) * PAGE_SIZE, PAGE_SIZE);
                }
                catch
                {
                    model.data = data.GetRange(((int)pageIndex - 1) * PAGE_SIZE, data.Count);
                }
                return View(model);
            }
            catch(Exception e)
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