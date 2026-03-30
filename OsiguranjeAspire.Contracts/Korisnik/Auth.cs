using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsiguranjeAspire.Contracts.Korisnik
{
    public sealed class LoginRequestDto
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public sealed class LoginResponseDto
    {
        public string Token { get; set; } = "";
        public DateTime ExpiresAtUtc { get; set; }
    }
}
