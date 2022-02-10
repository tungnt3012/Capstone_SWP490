using Capstone_SWP490.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Capstone_SWP490.Helper
{
    public class RegistrationHelper
    {
        public team_member getTeamByTeamName(List<team_member> teams, string teamName)
        {
            if (teams == null)
                return null;
            return teams.Find(x => x.team.team_name == teamName);
        }
        public bool isTeamLeader(member member, member compared)
        {
            if (compared == null)
            {
                return false;
            }
            return (member.email.Equals(compared.email));
        }
        public string createFileName(string fileExtension)
        {
            return DateTime.UtcNow.Ticks + "." + fileExtension;
        }
        public member getCoach(string[,] data)
        {
            member coach = new member();
            coach.first_name = extractFirstName(data[4, 0]);
            coach.middle_name = extractMiddleName(data[4, 0]);
            coach.last_name = extractLastName(data[4, 0]);
            coach.phone_number = data[5, 0];
            coach.email = data[6, 0];
            coach.member_role = 1;
            return coach;
        }
        public member getViceCoach(string[,] data)
        {
            member viceCoach = new member();
            viceCoach.first_name = extractFirstName(data[7, 0]);
            viceCoach.first_name = extractMiddleName(data[7, 0]);
            viceCoach.first_name = extractLastName(data[7, 0]);
            viceCoach.phone_number = data[8, 0];
            viceCoach.email = data[9, 0];
            viceCoach.member_role = 2;
            return viceCoach;
        }
        public school getSchool(string[,] data)
        {
           string name = upperCaseFirstCharacter(data[0, 0]);
            string shortName = data[1, 0].ToUpper();
            string address = data[2, 0];
            string type = upperCaseFirstCharacter(data[3, 0]);
            if(StringUtils.isNullOrEmpty(name) || StringUtils.isNullOrEmpty(shortName))
            {
                throw new SchoolException("1","School name or short name cannot be null",null);
            }
            school school = new school();
            school.school_name = name;
            school.short_name = shortName;
            school.address = address;
            school.type = type;
            return school;
        }
        public string extractFirstName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return upperCaseFirstCharacter(arr.FirstOrDefault());
        }
        public string extractLastName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return upperCaseFirstCharacter(arr.LastOrDefault());
        }
        public string extractMiddleName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length <= 2)
            {
                return "";
            }
            if (arr.Length == 3) { return upperCaseFirstCharacter(arr[1]); }
            string result = "";
            for (int i = 1; i < arr.Length - 1; i++)
            {
                result += " ";
            }
            return upperCaseFirstCharacter(result);
        }
        public school cleanSchool(school school)
        {
            school.city = "";
            school.teams = null;
            return school;
        }

        public bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
        public int toInt32(string s, int valueIfAbsent)
        {
            try
            {
               return Int32.Parse(s);
            }
            catch
            {
                return valueIfAbsent;
            }
        }
        public short toShort(string s)
        {
            try
            {
                return short.Parse(s);
            }
            catch
            {
                return -1;
            }
        }

        public DateTime toDateTime(string s)
        {
            try
            {
                return DateTime.Parse(s);
            }
            catch
            {
                return new DateTime();
            }
        }
        public short getGender(string text)
        {
            if(text.ToUpper().Equals("NAM") || text.ToUpper().Equals("MALE"))
            {
                return 0;
            }
            else if (text.ToUpper().Equals("NỮ") || text.ToUpper().Equals("FEMALE"))
            {
                return 1;
            }
            return 2;
        }

        public string upperCaseFirstCharacter(string text)
        {
            if (text.Length == 0)
            {
                return text;
            }

            string result = "";
            string[] arr = text.Trim().Split(' ');
            for(int i = 0;i < arr.Length; i++)
            {
                if(!arr[i].Equals(" ") && !arr[i].Equals(""))
                {
                    string upper = arr[i].ToUpper();
                    result += arr[i].Replace(arr[i].ElementAt(0), upper.ElementAt(0)) + " ";
                }
            }
            return result.Trim();   
        }
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
       public team_member getTeamMember(int memberId, team team)
        {
            team_member result = team.team_member.Where(x => x.member.member_id == memberId).FirstOrDefault();
            if (result != null)
            {
                return result;
            }
            return null;
        }
        public team getTeamByTeamName(List<team> teams, string teamName)
        {
            foreach(var team in teams)
            {
                if (team.team_name.ToUpper().Equals(teamName.ToUpper()))
                {
                    return team;
                }
            }
            return new team();
        }

        public bool checkExistEmail(List<string> emailList, string email)
        {
            return emailList.Where(x => x.Equals(email)).FirstOrDefault() != null;
        }
        public app_user createAppUserFromMember(member member)
        {
            if(member == null)
            {
                return null;
            }
            app_user user = new app_user();
            user.psw = CreatePassword(8);
            user.user_name = member.email;
            user.user_role = "MEMBER";
            user.encrypted_psw = user.psw;
            user.full_name = member.first_name + " " + member.middle_name + " " + member.last_name;
            user.email = member.email;
            user.verified = false;
            user.active = true;
            return user;
        }
        public member cleanMember(member member)
        {
            member result = new member();
            if(member == null)
            {
                return member;
            }

            result.first_name = member.first_name;
            result.middle_name = member.middle_name;
            result.last_name = member.last_name;
            result.member_role = member.member_role;
            result.dob = member.dob;
            result.email = member.email;
            result.phone_number = member.phone_number;
            result.gender = member.gender;
            result.year = member.year;
            result.award = member.award;
            result.shirt_sizing = member.shirt_sizing;
            result.event_notify = member.event_notify;
            result.icpc_id = member.icpc_id;
            return result;
        }
    }

}