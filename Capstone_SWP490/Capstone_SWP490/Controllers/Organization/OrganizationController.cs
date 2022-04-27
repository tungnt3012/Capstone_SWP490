using Capstone_SWP490.Constant.Const;
using Capstone_SWP490.Helper;
using Capstone_SWP490.Models.organization_ViewModel;
using Capstone_SWP490.Sercurity;
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
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        private readonly interfaces.Iteam_memberService _iteam_memberService = new services.teamMemberService();
        private static readonly ILog Log = LogManager.GetLogger(typeof(OrgnazationController));
        // GET: Orgnazation
        public ActionResult Overview()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult NewRegistCoach(organization_ListViewModel model, int? pageIndex)
        {
            try
            {
                if (pageIndex == null || pageIndex == 0)
                {
                    pageIndex = 1;
                }
                organization_ViewModel itemModel;
                List<organization_ViewModel> data = new List<organization_ViewModel>();
                List<app_user> coachUser = _iapp_UserService.findNewRegistCoach(model.keyword);
                foreach (var item in coachUser)
                {
                    itemModel = new organization_ViewModel();
                    itemModel.coach_id = item.user_id;
                    itemModel.full_name = item.full_name;
                    itemModel.email = item.email;
                    itemModel.status = item.active;
                    itemModel.coach_phone = _imemberService.GetMemberByUserId(item.user_id).phone_number;
                    school school = _ischoolService.findByCoachId(item.user_id).FirstOrDefault();
                    //school school = _ischoolService.findByNewRegistCoach(item.user_id);
                    if (school != null)
                    {
                        itemModel.school_name = school.school_name;
                        itemModel.institution_name = school.institution_name;
                        itemModel.school_phone = school.phone_number;
                        itemModel.school_address = school.address;
                    }
                    itemModel.is_duplicate_school = _ischoolService.checkDuplicate(itemModel.school_name, itemModel.institution_name);
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

        [AuthorizationAccept(Roles = "ORGANIZER")]
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
                List<app_user> coachUser = _iapp_UserService.findCoach(model.status, model.keyword);
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

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult RejectCoach(int coachId, string reason)
        {
            try
            {
                app_user coachUser = _iapp_UserService.getByUserId(coachId);
                school school = _ischoolService.findByNewRegistCoach(coachUser.user_id);
                member coachMember = _imemberService.GetMemberByUserId(coachUser.user_id);
                team coachTeam = _iteamService.findBySchoolId(school.school_id).FirstOrDefault();
                team_member teamMember = _iteam_memberService.getCoachTeamMember(coachTeam.team_id).FirstOrDefault();
                _iteam_memberService.delete(teamMember);
                _imemberService.deleteAsync(coachMember);
                _iteamService.delete(coachTeam);
                _ischoolService.deleteAsync(school);
                _iapp_UserService.delete(coachUser);
                new MailHelper().sendMailDisableCoach(coachUser, "rejeted", reason);
                return RedirectToAction(ACTION_CONST.Orgnazation.NEW_REGIST_COACH, ACTION_CONST.Orgnazation.CONTROLLER);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return RedirectToAction("", ACTION_CONST.Home.CONTROLLER);
            }
        }

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult EnableCoach(int user_id)
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

        [AuthorizationAccept(Roles = "ORGANIZER")]
        public ActionResult DisableCoach(int user_id, string reason)
        {
            try
            {
                app_user updated = _iapp_UserService.getByUserId(user_id);
                updated.active = false;
                _iapp_UserService.update(updated);
                new MailHelper().sendMailDisableCoach(updated, "deactived", reason);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return RedirectToAction(ACTION_CONST.Orgnazation.COACH_ACCOUNT, ACTION_CONST.Orgnazation.CONTROLLER);
        }
    }
}