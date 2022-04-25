﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Capstone_SWP490
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class gocyberx_icpcEntities : DbContext
    {
        public gocyberx_icpcEntities()
            : base("name=gocyberx_icpcEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<app_user> app_user { get; set; }
        public virtual DbSet<contest> contests { get; set; }
        public virtual DbSet<contest_member> contest_member { get; set; }
        public virtual DbSet<@event> events { get; set; }
        public virtual DbSet<member> members { get; set; }
        public virtual DbSet<member_event> member_event { get; set; }
        public virtual DbSet<page_content> page_content { get; set; }
        public virtual DbSet<school> schools { get; set; }
        public virtual DbSet<team> teams { get; set; }
        public virtual DbSet<team_member> team_member { get; set; }
        public virtual DbSet<post> posts { get; set; }
        public virtual DbSet<documentation> documentations { get; set; }
        public virtual DbSet<image> images { get; set; }
    
        public virtual ObjectResult<Nullable<int>> Check_Mail_In_Use(string email, Nullable<int> coach_id)
        {
            var emailParameter = email != null ?
                new ObjectParameter("email", email) :
                new ObjectParameter("email", typeof(string));
    
            var coach_idParameter = coach_id.HasValue ?
                new ObjectParameter("coach_id", coach_id) :
                new ObjectParameter("coach_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("Check_Mail_In_Use", emailParameter, coach_idParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> Count_Contestant()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("Count_Contestant");
        }
    
        public virtual ObjectResult<Nullable<int>> Count_Member_In_School(Nullable<int> schoolId)
        {
            var schoolIdParameter = schoolId.HasValue ?
                new ObjectParameter("schoolId", schoolId) :
                new ObjectParameter("schoolId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("Count_Member_In_School", schoolIdParameter);
        }
    
        public virtual int Enable_App_User(Nullable<int> schoolId)
        {
            var schoolIdParameter = schoolId.HasValue ?
                new ObjectParameter("schoolId", schoolId) :
                new ObjectParameter("schoolId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Enable_App_User", schoolIdParameter);
        }
    
        public virtual int Disable_App_User_Data(Nullable<int> school_id)
        {
            var school_idParameter = school_id.HasValue ?
                new ObjectParameter("school_id", school_id) :
                new ObjectParameter("school_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Disable_App_User_Data", school_idParameter);
        }
    
        public virtual int Disable_App_User_School(Nullable<int> school_id, Nullable<int> coach_id)
        {
            var school_idParameter = school_id.HasValue ?
                new ObjectParameter("school_id", school_id) :
                new ObjectParameter("school_id", typeof(int));
    
            var coach_idParameter = coach_id.HasValue ?
                new ObjectParameter("coach_id", coach_id) :
                new ObjectParameter("coach_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Disable_App_User_School", school_idParameter, coach_idParameter);
        }
    
        public virtual int Disable_Member_Data(Nullable<int> school_id)
        {
            var school_idParameter = school_id.HasValue ?
                new ObjectParameter("school_id", school_id) :
                new ObjectParameter("school_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Disable_Member_Data", school_idParameter);
        }
    
        public virtual int Disable_School_Data(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Disable_School_Data", idParameter);
        }
    
        public virtual int Disable_Team_Data(Nullable<int> school_id)
        {
            var school_idParameter = school_id.HasValue ?
                new ObjectParameter("school_id", school_id) :
                new ObjectParameter("school_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Disable_Team_Data", school_idParameter);
        }
    
        public virtual int Enable_App_User_Data(Nullable<int> school_id)
        {
            var school_idParameter = school_id.HasValue ?
                new ObjectParameter("school_id", school_id) :
                new ObjectParameter("school_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Enable_App_User_Data", school_idParameter);
        }
    
        public virtual int Enable_Member_Data(Nullable<int> school_id)
        {
            var school_idParameter = school_id.HasValue ?
                new ObjectParameter("school_id", school_id) :
                new ObjectParameter("school_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Enable_Member_Data", school_idParameter);
        }
    
        public virtual int Enable_School_Data(Nullable<int> id)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Enable_School_Data", idParameter);
        }
    
        public virtual int Enable_Team_Data(Nullable<int> school_id)
        {
            var school_idParameter = school_id.HasValue ?
                new ObjectParameter("school_id", school_id) :
                new ObjectParameter("school_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Enable_Team_Data", school_idParameter);
        }
    }
}
