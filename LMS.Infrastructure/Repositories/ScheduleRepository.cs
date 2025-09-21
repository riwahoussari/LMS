using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories
{
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(AppDbContext context) : base(context)
        {
        }
    }

    public class ScheduleSessionRepository : Repository<ScheduleSession>, IScheduleSessionRepository
    {
        public ScheduleSessionRepository(AppDbContext context) : base(context)
        {
        }
    }

}
