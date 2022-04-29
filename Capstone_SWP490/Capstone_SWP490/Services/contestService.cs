using Capstone_SWP490.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Models.contestViewModel;
using System.Threading.Tasks;
using Capstone_SWP490.Models;
using Capstone_SWP490.Models.school_memberViewModel;
using Capstone_SWP490.Models.statisticViewModel;

namespace Capstone_SWP490.Services
{
    public class contestService : IcontestService
    {
        private readonly IcontestRepository _icontestRepository = new contestRepository();
        private readonly ImemberRepository _imemberRepository = new memberRepository();
        private readonly Icontest_memberRepository _icontest_memberRepository = new contest_memberRepository();
        private readonly IschoolRepository _ischoolRepository = new schoolRepository();
        private readonly Iteam_memberRepository _iteam_memberRepository = new teamMemberRepository();

        public contest getByCode(string code)
        {
            try
            {
                return _icontestRepository.getByCode(code);
            }
            catch
            {
                return null;
            }
        }

        public contest getByCodeOrName(string code, string name)
        {
            if (code == null || name == null)
            {
                throw new Exception("Contest code and name cannot null");
            }
            return _icontestRepository.FindBy(x => x.max_contestant == 1 && (x.code.Equals(code) || x.contest_name.Equals(name))).FirstOrDefault();
        }

        public contest getById(int? id)
        {
            return _icontestRepository.FindBy(x => x.contest_id == id).FirstOrDefault();
        }

        public List<contest> getIndividualContest()
        {
            return _icontestRepository.FindBy(x => x.max_contestant == 1).ToList();
        }

        public List<contestViewModel> GetContests()
        {
            var con = _icontestRepository.FindBy(x => x.max_contestant != -1).ToList();
            if (con != null)
            {
                var rsLst = new List<contestViewModel>();
                foreach (var c in con)
                {
                    var conTemp = new contestViewModel
                    {
                        code = c.code,
                        contest_id = c.contest_id,
                        contest_member = c.contest_member,
                        contest_name = c.contest_name,
                        end_date = c.end_date,
                        max_contestant = c.max_contestant,
                        note = c.note,
                        shirt_id = c.shirt_id,
                        start_date = c.start_date,
                        venue = c.venue
                    };
                    rsLst.Add(conTemp);
                }
                return rsLst;
            }
            return null;
        }

        public async Task<contestViewModel> UpdateContest(contestViewModel contestIn)
        {
            var c = _icontestRepository.FindBy(x => x.contest_id == contestIn.contest_id).FirstOrDefault();
            if (c != null)
            {
                c.contest_name = contestIn.contest_name;
                c.code = contestIn.code;
                c.venue = contestIn.venue;

                if (contestIn.contest_type.Equals("Team"))
                {
                    c.max_contestant = contestIn.max_contestant;
                }
                else if (contestIn.contest_type.Equals("Individual"))
                {
                    c.max_contestant = 1;
                }

                c.note = contestIn.note;
                if (await _icontestRepository.Update(c, c.contest_id) != -1)
                {
                    var cOutput = new contestViewModel
                    {
                        code = c.code,
                        contest_id = c.contest_id,
                        contest_member = c.contest_member,
                        contest_name = c.contest_name,
                        end_date = c.end_date,
                        max_contestant = c.max_contestant,
                        note = c.note,
                        shirt_id = c.shirt_id,
                        start_date = c.start_date,
                        venue = c.venue,
                    };
                    if (cOutput.max_contestant == 1)
                    {
                        cOutput.contest_type = "Individual";
                    }
                    if (cOutput.max_contestant > 1)
                    {
                        cOutput.contest_type = "Team";
                    }
                    return cOutput;
                }
            }
            return null;
        }

        public async Task<contestViewModel> CreateContest(contestViewModel contestIn)
        {
            if (!string.IsNullOrWhiteSpace(contestIn.contest_name))
            {
                var c = new contest
                {
                    contest_name = contestIn.contest_name,
                    code = contestIn.code,
                    note = contestIn.note,
                    venue = contestIn.venue,
                    start_date = DateTime.Now, 
                    end_date = DateTime.Now,
                };

                if (contestIn.contest_type.Equals("Team"))
                {
                    c.max_contestant = contestIn.max_contestant;
                }
                else if (contestIn.contest_type.Equals("Individual"))
                {
                    c.max_contestant = 1;
                }
                var newContest = await _icontestRepository.Create(c);
                if (newContest != null)
                {
                    var cOutput = new contestViewModel
                    {
                        code = newContest.code,
                        contest_id = newContest.contest_id,
                        contest_member = newContest.contest_member,
                        contest_name = newContest.contest_name,
                        end_date = newContest.end_date,
                        max_contestant = newContest.max_contestant,
                        note = newContest.note,
                        shirt_id = newContest.shirt_id,
                        start_date = newContest.start_date,
                        venue = newContest.venue,
                    };
                    if (cOutput.max_contestant == 1)
                    {
                        cOutput.contest_type = "Individual";
                    }
                    if (cOutput.max_contestant > 1)
                    {
                        cOutput.contest_type = "Team";
                    }
                    return cOutput;
                }
            }
            return null;
        }

        public async Task<bool> DeleteContest(int id)
        {
            var c = _icontestRepository.FindBy(x => x.contest_id == id).FirstOrDefault();
            if (c != null)
            {
                c.max_contestant = -1;
                if (await _icontestRepository.Update(c, c.contest_id) != -1)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<contestViewModel> GetAllContestAvailale()
        {
            var contests = _icontestRepository.FindBy(x => x.max_contestant != -1).ToList();
            var lstContestOut = new List<contestViewModel>();
            if (contests != null)
            {
                foreach (var newContest in contests)
                {
                    var cOut = new contestViewModel
                    {
                        code = newContest.code,
                        contest_id = newContest.contest_id,
                        contest_member = newContest.contest_member,
                        contest_name = newContest.contest_name,
                        end_date = newContest.end_date,
                        max_contestant = newContest.max_contestant,
                        note = newContest.note,
                        shirt_id = newContest.shirt_id,
                        start_date = newContest.start_date,
                        venue = newContest.venue,
                    };
                    lstContestOut.Add(cOut);
                }
                return lstContestOut;
            }
            return null;
        }

        public contestViewModel GetContestById(int id)
        {
            var newContest = _icontestRepository.FindBy(x => x.contest_id == id).FirstOrDefault();
            if (newContest != null)
            {
                var cOutput = new contestViewModel
                {
                    code = newContest.code,
                    contest_id = newContest.contest_id,
                    contest_member = newContest.contest_member,
                    contest_name = newContest.contest_name,
                    end_date = newContest.end_date,
                    max_contestant = newContest.max_contestant,
                    note = newContest.note,
                    shirt_id = newContest.shirt_id,
                    start_date = newContest.start_date,
                    venue = newContest.venue,
                };
                if (cOutput.max_contestant == 1)
                {
                    cOutput.contest_type = "Individual";
                }
                if (cOutput.max_contestant > 1)
                {
                    cOutput.contest_type = "Team";
                }
                return cOutput;
            }
            return null;
        }

        public AjaxResponseViewModel<List<contestViewModel>> FilterContest(string keyFilter)
        {
            var output = new AjaxResponseViewModel<List<contestViewModel>>
            {
                Status = 0,
                Data = null
            };
            var lstFind = new List<contest>();
            if (keyFilter.Equals("Team"))
            {
                lstFind = _icontestRepository.FindBy(x => x.max_contestant == 0).ToList();
            }
            if (keyFilter.Equals("Individual"))
            {
                lstFind = _icontestRepository.FindBy(x => x.max_contestant > 0).ToList();
            }
            var lstOut = new List<contestViewModel>();
            if (lstFind != null)
            {
                foreach (var newContest in lstFind)
                {
                    var c = new contestViewModel
                    {
                        code = newContest.code,
                        contest_id = newContest.contest_id,
                        contest_member = newContest.contest_member,
                        contest_name = newContest.contest_name,
                        end_date = newContest.end_date,
                        max_contestant = newContest.max_contestant,
                        note = newContest.note,
                        shirt_id = newContest.shirt_id,
                        start_date = newContest.start_date,
                        venue = newContest.venue,
                    };
                    lstOut.Add(c);
                }
                output.Message = "success";
                output.Data = lstOut;
                output.Status = 1;
                return output;
            }
            output.Message = "Fail";
            return output;
        }

        public List<member_contest_ViewModel> getContestMemberModel(List<contest_member> contestMember)
        {
            List<member_contest_ViewModel> listContestModel = new List<member_contest_ViewModel>();
            member_contest_ViewModel contestModel;
            List<contest> individualContest = getIndividualContest();
            foreach (var item in individualContest)
            {
                contestModel = new member_contest_ViewModel();
                contestModel.code = item.code;
                contestModel.name = item.contest_name;
                contest_member joined = contestMember.Where(x => x.contest.max_contestant == -1 && x.contest.code.Equals(item.code)).FirstOrDefault();
                if (joined != null)
                {
                    contestModel.selected = true;
                }
                else
                {
                    contestModel.selected = false;
                }
                listContestModel.Add(contestModel);
            }
            return listContestModel;
        }

        public List<registered_contest_ViewModel> GetStaticAllContestAvailale()
        {
            var contests = _icontestRepository.FindBy(x => x.max_contestant != -1).ToList();
            var lstContestOut = new List<registered_contest_ViewModel>();
            if (contests != null)
            {
                foreach (var newContest in contests)
                {
                    var lstMembers = new List<member>();
                    var cTemp = new registered_contest_ViewModel
                    {
                        contest = newContest,
                    };
                    var contestMember = _icontest_memberRepository.FindBy(cm => cm.contest_id == newContest.contest_id).ToList();
                    if (contestMember.Count > 0)
                    {
                        foreach (var mem in contestMember)
                        {
                            var mTemp = _imemberRepository.FindBy(x => x.member_id == mem.member_id).FirstOrDefault();
                            lstMembers.Add(mTemp);
                        }
                        cTemp.lstMember = lstMembers;
                        cTemp.contestant_number = lstMembers.Count();
                    }
                    lstContestOut.Add(cTemp);
                }
                return lstContestOut;
            }
            return null;
        }

        public List<registered_contest_ViewModel> GetStaticContest()
        {
            var schools = _ischoolRepository.FindBy(x => x.active == 2 && x.enabled == true).ToList();
            List<team> teamRs = new List<team>();
            var lstOut = new List<registered_contest_ViewModel>();
            if (schools.Count > 0)
            {
                var lstMembers = new List<member>();
                foreach (var x in schools)
                {
                    if (x.teams.Count > 0)
                    {
                        foreach (var t in x.teams)
                        {
                            if (t.enabled == true)
                            {
                                var lstTeamMember = _iteam_memberRepository.FindBy(tm => tm.team_id == t.team_id).ToList();
                                foreach (var mem in lstTeamMember)
                                {
                                    var findMem = _imemberRepository.FindBy(fm => fm.member_id == mem.member_id).FirstOrDefault();
                                    if (findMem != null)
                                    {
                                        lstMembers.Add(findMem);
                                    }
                                }
                            }
                        }
                    }
                }

                var fContest = _icontestRepository.FindBy(contests => contests.max_contestant != -1).ToList();
                if (fContest.Count > 0)
                {
                    foreach (var c in fContest)
                    {
                        var memberInContest = _icontest_memberRepository.FindBy(x => x.contest_id == c.contest_id).ToList();
                        var lstM = new List<member>();

                        if (memberInContest.Count > 0)
                        {
                            foreach (var mc in memberInContest)
                            {
                                foreach (var m in lstMembers)
                                {
                                    if (mc.member_id == m.member_id)
                                    {
                                        lstM.Add(m);
                                    }
                                }
                            }
                        }
                        lstOut.Add(new registered_contest_ViewModel { contest = c, lstMember = lstM, contestant_number = lstM.Count });
                    }
                }
                return lstOut;
            }
            return null;
        }
    }
}