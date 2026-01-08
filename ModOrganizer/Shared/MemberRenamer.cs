using Scriban.Runtime;
using System.Reflection;

namespace ModOrganizer.Shared;

public static class MemberRenamer
{
    public static string Rename(MemberInfo memberInfo) => StandardMemberRenamer.Rename(memberInfo).Trim('_');
}
