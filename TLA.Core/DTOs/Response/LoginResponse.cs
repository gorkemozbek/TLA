namespace TLA.Core.DTOs.Response
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}