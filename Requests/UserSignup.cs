using System;
using System.Collections.Generic;

namespace WastedApi.Requests
{
    public partial class CustomerSignup
    {
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public List<string> isValid()
        {
            var messages = new List<string>();

            if (FirstName == "" || LastName == "" || UserName == "" || Password == "" || Email == "" || Password == "")
                return new List<string> { "There cannot be any empty input" };

            if (UserName.Length < 4)
                messages.Append("Username length must be higher than 4");

            if (Password.Length < 4)
                messages.Append("Password length must be higher than 4");

            if (Email.Length < 4)
                messages.Append("Email is not a proper email");

            return messages;
        }
    }
}
