using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

namespace Dalamud.Bindings.ImGui;

[InterpolatedStringHandler]
public struct ImU8String(string text)
{
    public static implicit operator ImU8String(string text) => new(text);

    public override readonly string ToString() => text;
}
