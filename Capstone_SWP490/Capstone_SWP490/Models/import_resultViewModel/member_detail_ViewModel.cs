using Capstone_SWP490.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models.school_memberViewModel
{
    public class member_detail_ViewModel
    {
        public member_detail_ViewModel()
        {
            errors = new Dictionary<string, string>();
        }
        public int team_id { get; set; }
        public int member_id { get; set; }
        public bool is_leader { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public System.DateTime dob { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public short gender { get; set; }
        public int? year { get; set; }
        public string award { get; set; }
        public int? icpc_id { get; set; }

        public Dictionary<string, string> errors { get; set; }
        public List<member_contest_ViewModel> contest_Members { get; set; }

        public member buildMember()
        {
            member member = new member();
            member.first_name = first_name;
            member.middle_name = middle_name;
            member.last_name = last_name;
            member.dob = CommonHelper.toDateTime(dob.ToString());
            member.email = email;
            member.phone_number = phone_number;
            member.year = year == null ? 0 : (int)year;
            member.gender = gender;
            member.award = award;
            member.icpc_id = icpc_id;
            member.member_role = (is_leader ? (short)3 : (short)4);
            return member;
        }
        public member_detail_ViewModel buildFromTeamMember(team_member teamMember)
        {
            team_id = (int)teamMember.team_id;
            member_id = (int)teamMember.member_id;
            first_name = teamMember.member.first_name;
            middle_name = teamMember.member.middle_name;
            last_name = teamMember.member.last_name;
            dob = teamMember.member.dob;
            email = teamMember.member.email;
            phone_number = teamMember.member.phone_number;
            icpc_id = teamMember.member.icpc_id;
            year = teamMember.member.year;
            gender = teamMember.member.gender;
            award = teamMember.member.award;
            is_leader = teamMember.member.member_role == 3;
            return this;
        }
    }
}