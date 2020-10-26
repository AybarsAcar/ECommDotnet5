using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Identity
{
  public class Address
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zipcode { get; set; }

    // Navigation props
    [Required]
    public string AppUserId { get; set; } // the relationship bw user and the address
    public AppUser AppUser { get; set; }
  }
}