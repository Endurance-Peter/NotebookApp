using System.ComponentModel.DataAnnotations;

namespace Notebook.Api.Authorizations.Authentication
{
    public class AuthResponse
    {
        public string? Token { get; set; }
        public bool IsSuccess { get; set; }
        public List<string>? Erros { get; set; }
    }

    public class LoginResponse : AuthResponse
    {

    }

    public class RegistrationResponse : AuthResponse
    {

    }

    public class LoginRequest
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }

    public class RegistrationRequest
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
