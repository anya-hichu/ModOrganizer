using ModOrganizer.Json.Readers.Asserts.Fakes;

namespace ModOrganizer.Tests.Json.Readers.Asserts;

public interface IStubbableAssert
{
    StubIAssert AssertStub { get; }
}
