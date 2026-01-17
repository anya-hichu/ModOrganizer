using Microsoft.Extensions.DependencyInjection;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Readers;

public interface IStubbableReaderProvider<D> where D : class
{
    StubIReader<D> GetReaderStub(string type);
}
