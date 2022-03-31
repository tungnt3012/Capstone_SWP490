using Capstone_SWP490.Helper;
using Capstone_SWP490.Models.coachViewModel;
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
        private readonly RegistrationHelper registrationHelper = new RegistrationHelper();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly interfaces.IschoolService _ischoolService = new services.schoolService();
        private readonly interfaces.IteamService _iteamService = new services.teamService();
        private readonly interfaces.ImemberService _imemberService = new services.memberService();
        private readonly interfaces.Iteam_memberService _iteam_memberService = new services.teamMemberService();
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
                if (!registrationHelper.IsValidEmail(data.email))
                {
                    @ViewData["CREATE_ERROR"] = "In valid email!";
                    return View();
                }
                if (_iapp_UserService.GetUserByUsername(data.email) != null)
                {
                    @ViewData["CREATE_ERROR"] = "Email is used, please try again!";
                    return View();
                }

                app_user coachUser = new app_user();
                coachUser.user_name = data.email;
                coachUser.full_name = data.full_name;
                coachUser.active = false;
                coachUser.verified = false;
                coachUser.user_role = "COACH";
                coachUser.email = data.email;
                coachUser.insert_date = Convert.ToString(DateTime.Now);
                coachUser.update_date = Convert.ToString(DateTime.Now);
                coachUser.psw = CommonHelper.CreatePassword(8);
                coachUser.encrypted_psw = CommonHelper.createEncryptedPassWord(coachUser.psw);
                app_user created = await _iapp_UserService.CreateUser(coachUser);

                school school = new school();
                school.school_name = data.school_name;
                school.institution_name = data.institution_name;
                school.address = data.school_address;
                school.insert_date = DateTime.Now + "";
                school.update_date = DateTime.Now + "";
                school.active = false;
                school.enabled = false;
                school.coach_id = created.user_id;
                school insertedSchool = await _ischoolService.insert(school);
                team team = new team();
                team.team_name = "COACH-TEAM";
                team.school_id = insertedSchool.school_id;
                team.type = "COACH-TEAM";
                team.enabled = false;
                team insertedTeam = await _iteamService.insert(team);
                member member = new member();
                member.first_name = registrationHelper.extractFirstName(data.full_name);
                member.middle_name = registrationHelper.extractMiddleName(data.full_name);
                member.last_name = registrationHelper.extractLastName(data.full_name);
                member.dob = DateTime.Now;
                member.email = data.email;
                member.phone_number = data.phone_numer;
                member.gender = -1;
                member.year = -1;
                member.member_role = 1;
                member.user_id = created.user_id;
                member insertedMember = await _imemberService.insert(member);
                team_member teamMember = new team_member();
                teamMember.member_id = insertedMember.member_id;
                teamMember.team_id = insertedTeam.team_id;
                _ = _iteam_memberService.insert(teamMember);
                new MailHelper().sendMailToInsertedUser(coachUser);
                @ViewData["CREATE_SUCCESS"] = data.email;
            }
            catch (Exception e)
            {
                @ViewData["CREATE_ERROR"] = "SYSTEM ERROR, please try again !";
            }
            return View();
        }
    }
}