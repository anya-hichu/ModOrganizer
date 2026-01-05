using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Shared;

public interface IConverter<I, O>
{
    bool TryConvert(I input, [NotNullWhen(true)] out O? output);
    bool TryConvertMany(IEnumerable<I> inputs, [NotNullWhen(true)] out HashSet<O>? outputs);
}
