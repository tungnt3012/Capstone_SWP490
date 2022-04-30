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
                _postService.UpdateScheduler();
            }
            catch
            {
                
            }
        }
    }
}