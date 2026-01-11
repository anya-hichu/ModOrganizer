using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Dalamud.Bindings.ImGui;

// Ref-less stubbable class (probably buggy but works well enough for testing)

public struct ImU8String
{
    public const int AllocFreeBufferSize = 512;
    private const int MinimumRentSize = AllocFreeBufferSize * 2;

    private byte[]? rentedBuffer;
    private ReadOnlyMemory<byte> externalBytes;
    private State state;
    private int length;

    [Flags]
    private enum State : byte
    {
        None = 0,
        Initialized = 1 << 0,
        NullTerminated = 1 << 1
    }

    public ImU8String(string? text)
    {
        this = text is null
            ? default
            : new ImU8String(text.AsSpan());
    }

    public ImU8String(ReadOnlySpan<char> text)
    {
        rentedBuffer = null;
        externalBytes = default;
        state = State.None;
        length = 0;

        if (text.IsEmpty)
        {
            state = State.Initialized | State.NullTerminated;
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(text.ToArray());
        length = bytes.Length;
        if (length + 1 < AllocFreeBufferSize)
        {
            rentedBuffer = ArrayPool<byte>.Shared.Rent(AllocFreeBufferSize);
            Array.Copy(bytes, 0, rentedBuffer, 0, length);
            rentedBuffer[length] = 0;
        }
        else
        {
            rentedBuffer = ArrayPool<byte>.Shared.Rent(length + 1);
            Array.Copy(bytes, 0, rentedBuffer, 0, length);
            rentedBuffer[length] = 0;
        }

        state = State.Initialized | State.NullTerminated;
    }

    public ImU8String(int byteLength, int extraPadding)
    {
        rentedBuffer = null;
        externalBytes = default;
        state = State.Initialized;
        length = byteLength;

        var totalSize = byteLength + extraPadding + 1;

        if (totalSize < AllocFreeBufferSize)
            totalSize = AllocFreeBufferSize;

        rentedBuffer = ArrayPool<byte>.Shared.Rent(totalSize);

        // Ensure null-termination
        rentedBuffer[byteLength] = 0;
        state |= State.NullTerminated;
    }

    public static ImU8String Empty => default;

    public readonly ReadOnlySpan<byte> Span
    {
        get
        {
            if ((state & State.Initialized) == 0)
                return ReadOnlySpan<byte>.Empty;

            if (rentedBuffer != null)
                return rentedBuffer.AsSpan(0, length);

            return externalBytes.Span;
        }
    }

    public readonly bool IsNull => (state & State.Initialized) == 0;
    public readonly bool IsEmpty => length == 0;

    public override readonly string ToString()
        => Encoding.UTF8.GetString(Span);

    public void Recycle()
    {
        if (rentedBuffer is { })
        {
            var buf = rentedBuffer;
            rentedBuffer = null;
            ArrayPool<byte>.Shared.Return(buf);
        }
    }

    public static implicit operator ImU8String(string? s) => new ImU8String(s);
    public static implicit operator ImU8String(ReadOnlySpan<char> x) => new ImU8String(x);

    public readonly ref readonly byte GetPinnableReference()
    {
        if (IsNull)
            return ref Unsafe.NullRef<byte>();
        if (rentedBuffer != null)
            return ref rentedBuffer![0];
        return ref MemoryMarshal.GetReference(externalBytes.Span);
    }

    public void AppendLiteral(string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        var vbytes = Encoding.UTF8.GetBytes(value);
        EnsureCapacity(length + vbytes.Length + 1);
        Span.Slice(length).CopyTo(vbytes);
        length += vbytes.Length;
        rentedBuffer![length] = 0;
    }

    private void EnsureCapacity(int minSize)
    {
        if (rentedBuffer is not null && rentedBuffer.Length >= minSize)
            return;

        var newSize = Math.Max(minSize, Math.Max(rentedBuffer?.Length ?? 0 * 2, MinimumRentSize));
        var newBuf = ArrayPool<byte>.Shared.Rent(newSize);
        if (rentedBuffer is { } old)
        {
            old.AsSpan(0, length).CopyTo(newBuf);
            ArrayPool<byte>.Shared.Return(old);
        }
        length = Span.Length;
        rentedBuffer = newBuf;
    }

    public void AppendFormatted<T>(T value)
    {
        if (value is null)
            return;

        if (value is ISpanFormattable spanFormattable)
        {
            // Try stack formatting first
            Span<char> temp = stackalloc char[128];
            if (spanFormattable.TryFormat(temp, out var written, default, null))
            {
                AppendChars(temp.Slice(0, written));
                return;
            }
        }

        // Fallback
        AppendLiteral(value.ToString()!);
    }

    private void AppendChars(ReadOnlySpan<char> chars)
    {
        if (chars.IsEmpty)
            return;

        var byteCount = Encoding.UTF8.GetByteCount(chars);
        EnsureCapacity(length + byteCount + 1);

        Encoding.UTF8.GetBytes(
            chars,
            rentedBuffer.AsSpan(length));

        length += byteCount;
        rentedBuffer![length] = 0;
    }
}
