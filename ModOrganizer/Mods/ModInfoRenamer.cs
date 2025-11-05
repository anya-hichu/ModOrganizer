using Scriban.Runtime;
using System.Reflection;

namespace ModOrganizer.Mods;

public static class ModInfoRenamer
{
    public static string RenameMember(MemberInfo memberInfo) => StandardMemberRenamer.Rename(memberInfo.Name).Trim('_');
}
