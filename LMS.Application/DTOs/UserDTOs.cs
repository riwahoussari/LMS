using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{

    

    public class UserUpdateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string BirthDate { get; set; } = null!;
    }

    public class UserResponseDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public string BirthDate { get; set; } = null!;
    }

}
