using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Tests.Mods.ModInterops;

namespace ModOrganizer.Tests.Mods.ModFileSystems;

public class ModFileSystemBuilder : Builder<ModFileSystem>, IStubbableModInterop
{
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override ModFileSystem Build() => new(ModInteropStub);
}
