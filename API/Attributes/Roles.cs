using KeepGrouped.API.Roles;

namespace KeepGrouped.API.Attributes.Roles;

public class RolesAttribute : Attribute
{
	public Perms Permission { get; set; }
	public RolesAttribute(Perms permission)
	{
		Permission = permission;
	}
}
