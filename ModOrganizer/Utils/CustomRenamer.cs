using Scriban.Runtime;
using System.Reflection;

namespace ModOrganizer.Utils;

public static class CustomRenamer
{
    public static string RenameMember(MemberInfo memberInfo) => StandardMemberRenamer.Rename(memberInfo.Name).Trim('_');
}
