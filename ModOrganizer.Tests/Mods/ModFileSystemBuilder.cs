using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Tests.Stubbables;

namespace ModOrganizer.Tests.Mods;

public class ModFileSystemBuilder : Builder<ModFileSystem>, IStubbableModInterop
{
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override ModFileSystem Build() => new(ModInteropStub);
}
