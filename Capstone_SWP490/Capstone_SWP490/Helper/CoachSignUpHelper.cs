using Capstone_SWP490.Constant.Const;
using Capstone_SWP490.Models.coachViewModel;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using interfaces = Capstone_SWP490.Services.Interfaces;
using services = Capstone_SWP490.Services;

namespace Capstone_SWP490.Helper
{
    public class CoachSignUpHelper
    {
        private readonly RegistrationHelper registrationHelper = new RegistrationHelper();
        private readonly interfaces.Iapp_userService _iapp_UserService = new services.app_userService();
        private readonly ResourceManagerHelper resourceManagerHelper = new ResourceManagerHelper("Message");
        public app_user buildCoachUser(coachSignUpViewModel data)
        {
            app_user coachUser = new app_user();
            coachUser.user_name = data.email;
            coachUser.full_name = data.full_name;
            coachUser.active = false;
            coachUser.verified = false;
            coachUser.user_role = APP_CONST.APP_ROLE.getUserRole(1);
            coachUser.email = data.email;
            coachUser.insert_date = Convert.ToString(DateTime.Now);
            coachUser.update_date = null;
            coachUser.psw = CommonHelper.CreatePassword(8);
            coachUser.encrypted_psw = CommonHelper.createEncryptedPassWord(coachUser.psw);
            return coachUser;
        }
        public school buildSchoolRegistered(coachSignUpViewModel data, int coachUserId)
        {
            school school = new school();
            school.school_name = StringUtils.isNullOrEmpty(data.school_name) ? "" : data.school_name;
            school.institution_name = StringUtils.isNullOrEmpty(data.institution_name) ? "" : data.institution_name;
            school.address = StringUtils.isNullOrEmpty(data.school_address) ? "" : data.school_address;
            school.insert_date = DateTime.Now + "";
            school.update_date = DateTime.Now + "";
            school.active = false;
            school.enabled = false;
            school.phone_number = StringUtils.isNullOrEmpty(data.school_phone) ? "" : data.school_phone;
            school.coach_id = coachUserId;
            return school;
        }
        public team buildCoachTeam(int schoolId)
        {
            team team = new team();
            team.team_name = APP_CONST.TEAM_ROLE.COACH_TEAM;
            team.school_id = schoolId;
            team.type = APP_CONST.TEAM_ROLE.COACH_TEAM;
            team.enabled = false;
            return team;
        }
        public member buildCoachMember(coachSignUpViewModel data, int coachUserId)
        {

            member member = new member();
            member.first_name = registrationHelper.extractFirstName(data.full_name);
            member.middle_name = registrationHelper.extractMiddleName(data.full_name);
            member.last_name = registrationHelper.extractLastName(data.full_name);
            member.dob = DateTime.Now;
            member.email = data.email;
            member.phone_number = StringUtils.isNullOrEmpty(data.phone_numer) ? "" : data.phone_numer;
            member.gender = -1;
            member.year = -1;
            member.member_role = 1;
            member.enabled = true;
            member.user_id = coachUserId;
            return member;
        }
        public team_member buildCoachTeamMember(int teamId, int memberId)
        {
            team_member teamMember = new team_member();
            teamMember.member_id = memberId;
            teamMember.team_id = teamId;
            return teamMember;
        }

        public string validateDataFromView(coachSignUpViewModel data)
        {
            try
            {
                if (!registrationHelper.IsValidEmail(data.email))
                {
                    return Message.MSG03;
                }
                if (_iapp_UserService.GetUserByUsername(data.email) != null)
                {
                    return Message.MSG02;
                }
                return "";
            }
            catch
            {
                return Message.SYSTEM_ERROR ;
            }
        }
    }
}