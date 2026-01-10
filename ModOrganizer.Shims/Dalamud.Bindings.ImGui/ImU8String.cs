using System.Runtime.CompilerServices;

namespace Dalamud.Bindings.ImGui;

[InterpolatedStringHandler]
public struct ImU8String(string text)
{
    public const int AllocFreeBufferSize = 512;

    public static implicit operator ImU8String(string text) => new(text);

    public override readonly string ToString() => text;
}
