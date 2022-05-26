
using System.ComponentModel.DataAnnotations;

namespace WastedApi.Models;
public class AdminRole
{
    public Guid Id { get; set; }

    public Member Member { get; set; } = null!;
    public Guid MemberId { get; set; }


}