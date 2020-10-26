using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
  public class AppUser : IdentityUser
  {
    // our props on top of IdentityUser
    public string DisplayName { get; set; }
    public Address Address { get; set; }
  }
}