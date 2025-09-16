using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LMS.Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email is required");
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!regex.IsMatch(value)) throw new ArgumentException("Invalid email format");
            Value = value;
        }
        public override string ToString() => Value;
    }
    
    public sealed record PhoneNumber
    {
        public string Value { get; }
        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Phone number required");
            // simple check - can extend later
            if (value.Length < 6) throw new ArgumentException("Invalid phone number");
            Value = value;
        }
        public override string ToString() => Value;
    }
    
    public sealed record Address(string Street, string City, string ZipCode, string Country)
    {
        public override string ToString() => $"{Street}, {City}, {ZipCode}, {Country}";
    }
}
