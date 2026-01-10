using System.Runtime.CompilerServices;

namespace Dalamud.Bindings.ImGui;

[InterpolatedStringHandler]
public struct ImU8String(string text)
{
    public static implicit operator ImU8String(string text) => new(text);

    public override readonly string ToString() => text;
}
