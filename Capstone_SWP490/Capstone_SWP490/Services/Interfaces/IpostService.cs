using Capstone_SWP490.Models.post_ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.Services.Interfaces
{
    interface IpostService
    {
        Task<post> insert(post post);
        Task<int> update(post post);
        post getById(int id);
        List<post> getByAuthorId(int authorId, string statu);
        List<post> getToScheduler();

        List<post_TopViewModel> GetTop5Posts();
        Task<int> Delete(int postId);
    }
}
