using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Readers.Elements;

public class ElementBuilder : IBuilder<JsonElement>, IStubbablePluginLog
{
    public string? JsonValue { get; set; }
    public object? Value { get; set; }

    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ElementBuilder WithValue(object? value)
    {
        if (JsonValue != null) throw new NotImplementedException();

        Value = value;
        return this;
    }

    public ElementBuilder WithJsonValue(string? value)
    {
        if (Value != null) throw new NotImplementedException();

        JsonValue = value;
        return this;
    }

    public JsonElement Build() => JsonValue == null ? JsonSerializer.SerializeToElement(Value) : JsonSerializer.Deserialize<JsonElement>(JsonValue);
}
