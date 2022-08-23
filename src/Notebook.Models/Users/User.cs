using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook.Models.Users
{
    public class User : BaseEntity
    {
        public  string? FirstName { get; set; }
        public  string? LastName { get; set; }
        public  string? Email { get; set; }
        public  Guid IdentityId { get; set; }
    }

    public class RefreshToken : BaseEntity
    {
        public Guid OwnerId { get; set; }
        public string? Token { get; set; }
        public string? JwtTokenId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
