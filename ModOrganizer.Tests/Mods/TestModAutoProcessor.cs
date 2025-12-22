using Dalamud.Interface.ImGuiNotification;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Stubbables;

namespace ModOrganizer.Tests.Mods;

[TestClass]
public class TestModAutoProcessor : TestClass
{
    [TestMethod]
    public async Task TestAutoProcessDisabled()
    {
        var observer = new StubObserver();
        var builder = new ModAutoProcessorBuilder();

        var modAutoProcessor = builder
            .WithPluginLogDefaults()
            .WithModProcessorDefaults()
            .WithNotificationManagerDefaults()
            .WithPluginLogObserver(observer)
            .WithModProcessorObserver(observer)
            .WithNotificationManagerObserver(observer)
            .WithConfigAutoProcessEnabled(false)
            .Build();

        builder.ModInteropStub.OnModAddedEvent("Mod Directory");

        Assert.IsEmpty(observer.GetCalls());

        var task = modAutoProcessor.Task;

        await task;

        Assert.IsTrue(task.IsCompletedSuccessfully);
        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    public async Task TestAutoProcessEnabled()
    {
        var logObserver = new StubObserver();
        var notificationObserver = new StubObserver();
        var processorObserver = new StubObserver();

        var builder = new ModAutoProcessorBuilder();

        ushort delay = 5;
        var modDirectory = "Mod Directory";
        var newModPath = "New Mod Path";

        var modAutoProcessor = builder
            .WithPluginLogDefaults()
            .WithModProcessorDefaults()
            .WithNotificationManagerDefaults()
            .WithConfigAutoProcessDelay(delay)
            .WithConfigAutoProcessEnabled(true)
            .WithPluginLogObserver(logObserver)
            .WithModProcessorTryProcess(newModPath)
            .WithModProcessorObserver(processorObserver)
            .WithNotificationManagerObserver(notificationObserver)
            .Build();

        builder.ModInteropStub.OnModAddedEvent(modDirectory);

        var logCalls = logObserver.GetCalls();
        Assert.HasCount(1, logCalls);

        var logCall = logCalls[0];
        Assert.AreEqual("Debug", logCall.StubbedMethod.Name);

        var logArguments = logCall.GetArguments();
        Assert.HasCount(2, logArguments);
        Assert.AreEqual($"Waiting [{delay}] ms before processing mod [{modDirectory}]", logArguments[0] as string);

        var task = modAutoProcessor.Task;

        await task;

        Assert.IsTrue(task.IsCompletedSuccessfully);

        var processorCalls = processorObserver.GetCalls();
        Assert.HasCount(1, processorCalls);

        var processorCall = processorCalls[0];
        Assert.AreEqual("TryProcess", processorCall.StubbedMethod.Name);

        var processorArguments = processorCall.GetArguments();
        Assert.HasCount(3, processorArguments);

        Assert.AreEqual(modDirectory, processorArguments[0] as string);

        var notificationCalls = notificationObserver.GetCalls();
        Assert.HasCount(1, notificationCalls);

        var notificationCall = notificationCalls[0];
        Assert.AreEqual("AddNotification", notificationCall.StubbedMethod.Name);

        var notificationArguments = notificationCall.GetArguments();
        Assert.HasCount(1, notificationArguments);

        var notification = notificationArguments[0] as Notification;

        Assert.IsNotNull(notification);
        Assert.AreEqual("ModOrganizer", notification.Title);
        Assert.AreEqual($"Updated mod [{modDirectory}] path to [{newModPath}]", notification.MinimizedText);
    }
}
