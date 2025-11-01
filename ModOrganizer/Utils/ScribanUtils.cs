using Scriban.Runtime;
using System.Reflection;

namespace ModOrganizer.Utils;

public static class ScribanUtils
{
    // Remove starting and ending underscores
    public static string RenameMember(MemberInfo memberInfo) => StandardMemberRenamer.Rename(memberInfo.Name).Trim('_');
}
