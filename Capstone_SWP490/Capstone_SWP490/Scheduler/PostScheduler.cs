using Capstone_SWP490.Models;
using Capstone_SWP490.Services;
using Capstone_SWP490.Services.Interfaces;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone_SWP490.Scheduler
{
    public class PostScheduler 
    {
        public static void Start()

        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            IJobDetail job = JobBuilder.Create<PostJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()

                .WithDailyTimeIntervalSchedule

                  (s =>

                     s.WithIntervalInMinutes(2)

                    .OnEveryDay()

                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))

                  )

                .Build();



            scheduler.ScheduleJob(job, trigger);

        }

    }
}