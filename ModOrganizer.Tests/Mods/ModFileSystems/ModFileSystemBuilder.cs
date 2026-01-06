using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Mods.ModInterops;

namespace ModOrganizer.Tests.Mods.ModFileSystems;

public class ModFileSystemBuilder : IBuilder<ModFileSystem>, IStubbableModInterop
{
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ModFileSystem Build() => new(ModInteropStub);
}
