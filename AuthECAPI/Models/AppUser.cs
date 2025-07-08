using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthECAPI.Models;

public class AppUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "varchar(150)")]
    public string FullName { get; set; }
}
