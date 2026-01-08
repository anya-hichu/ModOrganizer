using Scriban.Runtime;
using System.Reflection;
using System.Xml.Linq;

namespace ModOrganizer.Shared;

public static class MemberRenamer
{
    public static string Rename(MemberInfo memberInfo) => StandardMemberRenamer.Rename(memberInfo.Name).Trim('_');
}
