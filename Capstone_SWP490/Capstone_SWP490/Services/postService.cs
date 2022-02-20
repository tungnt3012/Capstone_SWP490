using Capstone_SWP490.Repositories;
using Capstone_SWP490.Repositories.Interfaces;
using Capstone_SWP490.Services.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Capstone_SWP490.Services
{
    public class postService : IpostService
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(postService));
        private readonly IpostRepository _ipostRepository = new postRepository();

        public List<post> getByAuthorId(int authorId, string status)
        {
            try
            {
                if (status != null)
                {
                    if (status.Equals("posted"))
                    {
                        return _ipostRepository.FindBy(x => x.post_by == authorId && x.enabled == true).ToList();
                    }
                    else if (status.Equals("scheduling"))
                    {
                        return _ipostRepository.FindBy(x => x.post_by == authorId && x.enabled == false && (x.schedule_date != null && !x.schedule_date.Equals(""))).ToList();
                    }
                }
                return _ipostRepository.FindBy(x => x.post_by == authorId).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw e;
            }
        }

        public post getById(int id)
        {
            try
            {
                return _ipostRepository.FindBy(x => x.post_id == id).FirstOrDefault();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw e;
            }
        }

        public List<post> getToScheduler()
        {
            try
            {
                DateTime now2 = DateTime.Now;
                return _ipostRepository.FindBy(x => x.enabled == false && x.schedule_date != null).ToList();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                throw e;
            }
        }

        public async Task<post> insert(post post)
        {
            return await _ipostRepository.Create(post);
        }

        public async Task<int> update(post post)
        {
            return await _ipostRepository.Update(post, post.post_id);
        }
    }
}