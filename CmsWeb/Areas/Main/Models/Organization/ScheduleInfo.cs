﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMSPresenter;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Models.OrganizationPage
{
    public class ScheduleInfo
    {
        private OrgSchedule sc;
        public ScheduleInfo (OrgSchedule sc)
    	{
            this.sc = sc;
    	}
        public ScheduleInfo ()
    	{
            sc = new OrgSchedule();
    	}
        public int Id
        {
            get { return sc.Id; }
            set { sc.Id = value; }
        }
        public int SchedDay
        {
            get { return sc.SchedDay ?? 0; }
            set { sc.SchedDay = value; }
        }
        public string Time
        {
            get
            {
                if (!sc.SchedTime.HasValue)
                    return "8:00 AM";
                return sc.SchedTime.ToString2("h:mm tt");
            }
            set { sc.SchedTime = value.ToDate(); }
        }
        public int AttendCreditId
        {
            get { return sc.AttendCreditId ?? 1; }
            set { sc.AttendCreditId = value; }
        }
        public SelectList DaysOfWeek()
        {
            return new SelectList(new[] {
                new { Text = "Sun", Value = "0" },
                new { Text = "Mon", Value = "1" },
                new { Text = "Tue", Value = "2" },
                new { Text = "Wed", Value = "3" },
                new { Text = "Thu", Value = "4" },
                new { Text = "Fri", Value = "5" },
                new { Text = "Sat", Value = "6" },
                new { Text = "Any", Value = "10" }
                }, "Value", "Text", SchedDay.ToString());
        }
        public SelectList AttendCredits()
        {
            return new SelectList(CodeValueController.AttendCredits(),
                "Id", "Value", AttendCreditId.ToString());
        }
        public string DisplayAttendCredit
        {
            get
            {
                return (from i in CodeValueController.AttendCredits()
                        where i.Id == AttendCreditId
                        select i.Value).Single();
            }
        }
        public string DisplayDay
        {
            get
            {
                return (from i in DaysOfWeek()
                        where i.Value == SchedDay.ToString()
                        select i.Text).Single();
            }
        }
        public string Display
        {
            get
            {
                return "{0}, {1}, {2}".Fmt(DisplayDay, Time, DisplayAttendCredit);
            }
        }
        public string Value
        {
            get
            {
                return NewMeetingTime.ToString("M/d/yy,h:mm tt,") + AttendCreditId;
            }
        }
        public DateTime NewMeetingTime
        {
            get
            {
                DateTime dt;
                if (sc.SchedDay < 9)
                {
                    dt = Util.Now.Date;
                    dt = dt.AddDays(-(int)dt.DayOfWeek); // prev sunday
                    dt = dt.AddDays(sc.SchedDay ?? 0);
                    if (dt > Util.Now.Date)
                        dt = dt.AddDays(-7);
                }
                else
                    dt = Util.Now.Date;
                var tm = Time.ToDate().Value.TimeOfDay;
                dt = dt.Add(tm);
                return dt;
            }
        }
    }
}