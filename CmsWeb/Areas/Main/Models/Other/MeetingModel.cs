﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Web.Mvc;
using UtilityExtensions;
using CmsWeb.Areas.Main.Models.Report;

namespace CmsWeb.Models
{
    public class MeetingModel
    {
        public CmsData.Meeting meeting;

        public bool showall { get; set; }

        public MeetingModel(int id)
        {
            meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingId == id);
        }
        public IEnumerable<RollsheetModel.AttendInfo> Attends()
        {
            return RollsheetModel.RollList(meeting.MeetingId, meeting.OrganizationId, meeting.MeetingDate.Value);
        }
        public string AttendCreditType()
        {
            if (meeting.AttendCredit == null)
                return "| Every Meeting";
            return "| " + meeting.AttendCredit.Description;
        }
    }
}