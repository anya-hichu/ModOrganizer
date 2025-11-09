using Scriban.Runtime;
using System.Reflection;

namespace ModOrganizer.Mods;

public static class ModInfoMemberRenamer
{
    public static string Rename(MemberInfo memberInfo) => Rename(memberInfo.Name);
    public static string Rename(string name) => StandardMemberRenamer.Rename(name).Trim('_');
}
