using KeepGrouped.API.Middlewares;

namespace KeepGrouped.API.Attributes.Roles;

public class RolesAttribute: Attribute
{
    public int Permission {get; set;}
    public RolesAttribute(int permission)
    {
        Permission = permission;
    }
}