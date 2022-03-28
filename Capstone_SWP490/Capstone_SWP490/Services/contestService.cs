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

namespace Capstone_SWP490.Services
{
    public class contestService : IcontestService
    {
        private readonly IcontestRepository _icontestRepository = new contestRepository();

        public contest getByCode(string code)
        {
            return _icontestRepository.getByCode(code);
        }

        public contest getByCodeOrName(string code, string name)
        {
            if (code == null || name == null)
            {
                throw new Exception("Contest code and name cannot null");
            }
            return _icontestRepository.FindBy(x => x.code.Equals(code) || x.contest_name.Equals(name)).FirstOrDefault();
        }

        public contest getById(int? id)
        {
            return _icontestRepository.FindBy(x => x.contest_id == id).FirstOrDefault();
        }

        public List<contest> getIndividualContest()
        {
            return _icontestRepository.FindBy(x => x.max_contestant == 0).ToList();
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
                if (contestIn.end_date > contestIn.start_date)
                {
                    c.contest_name = contestIn.contest_name;
                    c.start_date = contestIn.start_date;
                    c.end_date = contestIn.end_date;
                    c.code = contestIn.code;
                    c.venue = contestIn.venue;

                    if (contestIn.contest_type.Equals("Team"))
                    {
                        c.max_contestant = 0;
                    }
                    else if (contestIn.contest_type.Equals("Individual"))
                    {
                        c.max_contestant = contestIn.max_contestant;
                    }

                    c.note = contestIn.note;
                    if (await _icontestRepository.Update(c, c.contest_id) != -1)
                    {
                        return new contestViewModel
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
                    }
                }
            }
            return null;
        }

        public async Task<contestViewModel> CreateContest(contestViewModel contestIn)
        {
            if (!string.IsNullOrWhiteSpace(contestIn.contest_name)
                && Convert.ToDateTime("01/01/0001") != contestIn.start_date
                && Convert.ToDateTime("01/01/0001") != contestIn.end_date
                )
            {
                if (contestIn.end_date > contestIn.start_date)
                {
                    var c = new contest
                    {
                        contest_name = contestIn.contest_name,
                        code = contestIn.code,
                        note = contestIn.note,
                        start_date = contestIn.start_date,
                        end_date = contestIn.end_date,
                        venue = contestIn.venue,
                    };
                    if (contestIn.contest_type.Equals("Team"))
                    {
                        c.max_contestant = 0;
                    }
                    else if (contestIn.contest_type.Equals("Individual"))
                    {
                        c.max_contestant = contestIn.max_contestant;
                    }
                    var newContest = await _icontestRepository.Create(c);
                    if (newContest != null)
                    {
                        return new contestViewModel
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
                    }
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
                return new contestViewModel
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
    }
}