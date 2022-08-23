namespace Notebook.Api.Authorizations
{
    public class JwtSecrets
    {
        public string? Secrets { get; set; }
        public TimeSpan ExpiryTime { get; set; }
    }
}
