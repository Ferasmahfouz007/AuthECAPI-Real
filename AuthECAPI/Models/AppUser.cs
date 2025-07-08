using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthECAPI.Models;

public class AppUser : IdentityUser
{
    // The PersonalData attribute marks this property as containing sensitive personal data. 
    [PersonalData]
    [Column(TypeName = "varchar(150)")]
    public string FullName { get; set; }
    // It can be used by Entity Framework to help with data privacy concerns, such as ensuring that 
    [PersonalData]
    [Column(TypeName = "nvarchar(10)")]
    public string Gender { get; set; }
    // this data is handled according to privacy regulations (e.g., GDPR).
    [PersonalData]
    public DateOnly DOB { get; set; } //Date of Birthday
    [PersonalData]
    public int? LibraryId { get; set; }
}
