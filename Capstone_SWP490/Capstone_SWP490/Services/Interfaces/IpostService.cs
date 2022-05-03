using Capstone_SWP490.Models;
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
        List<post_TopViewModel> getByAuthorId(string statu);
        List<post> getToScheduler();

        List<post_TopViewModel> GetTop8Posts();
        List<post_TopViewModel> GetTopAllPosts();
        Task<int> Delete(int postId);
        Task<AjaxResponseViewModel<bool>> PinPost(int postId);
        Task<AjaxResponseViewModel<bool>> UnPinPost(int postId);
        Task<int> Disable(int id);
        Task<int> Enable(int id);
        Task<int> UpdatePost(post post);
        Task<int> Unpin(int id);
        Task<int> UpdateScheduler();
    }
}
