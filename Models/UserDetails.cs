namespace WebApplication2.Models
{
    public class UserDetails
    {
        public int Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public byte[]? PasswordHash { get; set;}

        public byte[]? PasswordSalt { get; set;}

        public string? Token { get; set;}
    }
}
