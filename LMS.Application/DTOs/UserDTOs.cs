using LMS.Application.Strategies.Sorting;
using LMS.Domain.Entities;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.DTOs
{
    // Read

    public class GetUsersQueryDto
    {
        public string? Role {  get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? SortBy { get; set; }
        public bool? SortAsc { get; set; }
    }

    public class UserUpdateDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string BirthDate { get; set; } = null!;
    }


    // Response
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
