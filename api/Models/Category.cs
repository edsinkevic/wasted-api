
using System.ComponentModel.DataAnnotations;

namespace WastedApi.Models;
public enum Category
{
    [Display(Name = "groceries")]
    Groceries,
    [Display(Name = "drinks")]
    Drinks,
    [Display(Name = "meat")]
    Meat,
    [Display(Name = "sweets")]
    Sweets,
    [Display(Name = "other")]
    Other
}