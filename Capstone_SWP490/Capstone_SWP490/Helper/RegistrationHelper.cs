using System;
using System.Collections.Generic;
using System.Linq;
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
            return (member.first_name == compared.first_name && member.middle_name == compared.middle_name && member.last_name == compared.last_name
                && member.email == compared.email && member.phone_number == member.phone_number);
        }
        public string createFileName(string fileExtension)
        {
            return DateTime.UtcNow.Ticks + "." + fileExtension;
        }
        public member getCoach(string[,] data)
        {
            member coach = new member();
            coach.first_name = extractFirstName(data[2, 0]);
            coach.middle_name = extractMiddleName(data[2, 0]);
            coach.last_name = extractLastName(data[2, 0]);
            coach.phone_number = data[3, 0];
            coach.email = data[4, 0];
            coach.member_role = 1;
            return coach;
        }
        public member getViceCoach(string[,] data)
        {
            member viceCoach = new member();
            viceCoach.first_name = extractFirstName(data[5, 0]);
            viceCoach.first_name = extractMiddleName(data[5, 0]);
            viceCoach.first_name = extractLastName(data[5, 0]);
            viceCoach.phone_number = data[6, 0];
            viceCoach.email = data[7, 0];
            viceCoach.member_role = 2;
            return viceCoach;
        }
        public school getSchool(string[,] data)
        {
            school school = new school();
            school.school_name = data[0, 0];
            school.address = data[1, 0];
            return school;
        }
        public string extractFirstName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return arr.FirstOrDefault();
        }
        public string extractLastName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length == 0)
            {
                return "";
            }
            return arr.LastOrDefault();
        }
        public string extractMiddleName(string name)
        {
            string[] arr = name.Split(' ');
            if (arr.Length <= 2)
            {
                return "";
            }
            if (arr.Length == 3) { return arr[1]; }
            string result = "";
            for (int i = 1; i < arr.Length - 1; i++)
            {
                result += " ";
            }
            return result;
        }
        public school cleanSchool(school school)
        {
            school.city = "";
            school.type = "";
            school.short_name = "FPT";
            school.teams = null;
            return school;
        }
    }
}