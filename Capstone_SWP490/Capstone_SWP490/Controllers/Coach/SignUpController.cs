using Capstone_SWP490.Helper;
using Capstone_SWP490.Models.coachViewModel;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using interfaces = Capstone_SWP490.Services.Interfaces;
using services = Capstone_SWP490.Services;

namespace Capstone_SWP490.Controllers.Coach
{
    public class SignUpController : Controller
    {
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        private readonly interfaces.Iteam_memberService _iteam_memberService = new services.teamMemberService();
        private readonly CoachSignUpHelper coachSignUpHelper = new CoachSignUpHelper();

        // GET: SignUp
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(coachSignUpViewModel data)
        {
            try
            {
                string validateMsg = coachSignUpHelper.validateDataFromView(data);
                if (!StringUtils.isNullOrEmpty(validateMsg))
                {
                    @ViewData["CREATE_ERROR"] = validateMsg;
                    return View();
                }

                app_user coachUser = coachSignUpHelper.buildCoachUser(data);
                app_user createdAppUser = await _iapp_UserService.CreateUser(coachUser);

                school school = coachSignUpHelper.buildSchoolRegistered(data, createdAppUser.user_id);
                school insertedSchool = await _ischoolService.insert(school);

                team team = coachSignUpHelper.buildCoachTeam(insertedSchool.school_id);
                team insertedTeam = await _iteamService.insert(team);

                member member = coachSignUpHelper.buildCoachMember(data, createdAppUser.user_id);
                member insertedMember = await _imemberService.insert(member);

                team_member teamMember = coachSignUpHelper.buildCoachTeamMember(insertedTeam.team_id, insertedMember.member_id);
                _ = await _iteam_memberService.insert(teamMember);

                new MailHelper().sendMailToInsertedUser(coachUser);
                @ViewData["CREATE_SUCCESS"] = data.email;
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
                @ViewData["CREATE_ERROR"] = Message.SYSTEM_ERROR;
            }
            return View();
        }
    }
}