using System;

namespace DbToRest.Core.Services.Authentication
{
    public class SessionUser
    {
        public Guid Id { get; set; }

        public Guid SessionId { get; set; }

        public string Data { get; set; }

        public string Username { get; internal set; }

        public string Email { get; internal set; }

    }
}