﻿using System.Linq;
using CmsData;

namespace CmsWeb.Areas.People.Models
{
    public class EmailReceivedModel : EmailModel
    {
        public EmailReceivedModel(int id) : base(id) { }

        public override IQueryable<EmailQueue> DefineModelList()
        {
            var q = from e in DbUtil.Db.EmailQueues
                    where e.Sent != null
                    where !(e.Transactional ?? false)
                    where e.EmailQueueTos.Any(ee => ee.PeopleId == person.PeopleId)
                    where e.QueuedBy != person.PeopleId
                    where e.FromAddr != (person.EmailAddress ?? "")
                    where e.FromAddr != (person.EmailAddress2 ?? "")
                    select e;
            return FilterForUser(q);
        }
    }
}
