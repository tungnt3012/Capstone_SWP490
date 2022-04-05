using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Models
{
    public class PostJob : IJob
    {
        private readonly IpostService _postService = new postService();
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                List<post> posts = _postService.getToScheduler();
                foreach (var item in posts)
                {
                    long now = DateTime.Now.Millisecond;
                    long scheduleTime2 = DateTime.Parse(item.schedule_date.ToString()).Millisecond;
                    if ((scheduleTime2 + 5*60000) >= now)
                    {
                        item.enabled = true;
                        item.schedule_date = null;
                        _postService.update(item);
                    }
                }
            }
            catch
            {
                
            }
        }
    }
}