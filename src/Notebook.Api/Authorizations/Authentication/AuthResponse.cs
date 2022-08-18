namespace Notebook.Api.Authorizations.Authentication
{
    public class AuthResponse
    {
        public AuthResponse()
        {
            Erros = new List<string>();
        }
        public string? Token { get; set; }
        public bool IsSuccess { get; set; }
        public IList<string>? Erros { get; set; }
    }

    public class LoginResponse : AuthResponse
    {

    }

    public class RegistrationResponse : AuthResponse
    {

    }

    public class Login
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class Registration
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
