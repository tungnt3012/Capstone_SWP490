using Capstone_SWP490.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
    public interface ImemberService
    {
        Task<member> insert(member member);
        Task<IEnumerable<member>> insertMany(IEnumerable<member> member);
        Task<int> update(member member, int key);
        Task<member> RegisterShirtSize(string username, string size);
        Task<int> deleteAsync(member member);
        member GetMemberByUserId(int? userId);
        member getByEmail(string email);

        Task<AjaxResponseViewModel<bool>> JoinEvent(int id);

    }
}
