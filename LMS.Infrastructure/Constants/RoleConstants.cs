using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Constants
{
    public static class RoleConstants
    {
        public const string Admin = "admin";
        public const string Tutor = "tutor";
        public const string Student = "student";

        public static readonly string[] List = { Admin, Tutor, Student };
    }
}
