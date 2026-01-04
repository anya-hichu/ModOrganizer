using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Asserts.Fakes;

namespace ModOrganizer.Tests.Json.Asserts;

public interface IStubbableAssert
{
    StubIAssert AssertStub { get; }
}
