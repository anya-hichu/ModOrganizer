using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Mods.Interops;

namespace ModOrganizer.Tests.Mods.FileSystems;

public class ModFileSystemBuilder : IBuilder<ModFileSystem>, IStubbableModInterop
{
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ModFileSystem Build() => new(ModInteropStub);
}
