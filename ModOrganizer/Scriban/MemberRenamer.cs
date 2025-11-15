using Scriban.Runtime;
using System.Reflection;

namespace ModOrganizer.Scriban;

public static class MemberRenamer
{
    public static string Rename(MemberInfo memberInfo) => Rename(memberInfo.Name);
    public static string Rename(string name) => StandardMemberRenamer.Rename(name).Trim('_');
}
