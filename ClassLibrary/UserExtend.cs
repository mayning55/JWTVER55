using System;
using Microsoft.AspNetCore.Identity;

namespace ClassLibrary;

public class UserExtend:IdentityUser
{
    public bool IsDisabled { get; set; }
    public DateTime CreateDatetime { get; set; }
    public long JWTVer { get; set; }

}
