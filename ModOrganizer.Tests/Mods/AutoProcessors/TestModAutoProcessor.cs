using Dalamud.Interface.ImGuiNotification;
using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Mods.Processors;
using ModOrganizer.Tests.Dalamuds.NotificationManagers;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.TestableClasses;

namespace ModOrganizer.Tests.Mods.AutoProcessors;

[TestClass]
public class TestModAutoProcessor : ITestableClassTemp
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestAutoProcessDisabled()
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

        Assert.IsEmpty(modAutoProcessor.GetRunningTasks());
        Assert.IsEmpty(observer.GetCalls());
    }

    [TestMethod]
    [DoNotParallelize]
    public async Task TestAutoProcessEnabled()
    {
        var logObserver = new StubObserver();
        var notificationObserver = new StubObserver();
        var processorObserver = new StubObserver();

        var builder = new ModAutoProcessorBuilder();

        ushort delay = 1;
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

        var runningTasks = modAutoProcessor.GetRunningTasks();
        Assert.HasCount(1, runningTasks);
        var task = runningTasks.ElementAt(0);

        Task.WaitAll(runningTasks, TestContext.CancellationToken);
        Assert.IsTrue(task.IsCompletedSuccessfully);

        var logCalls = logObserver.GetCalls();
        Assert.HasCount(1, logCalls);
        AssertPluginLog.MatchObservedCall(logCalls[0], nameof(IPluginLog.Debug), 
            actualMessage => Assert.StartsWith($"Waiting [{delay}] ms before processing mod [{modDirectory}] inside task", actualMessage));

        var processorCalls = processorObserver.GetCalls();
        Assert.HasCount(1, processorCalls);

        var processorCall = processorCalls[0];
        Assert.AreEqual(nameof(IModProcessor.TryProcess), processorCall.StubbedMethod.Name);

        var processorArguments = processorCall.GetArguments();
        Assert.HasCount(3, processorArguments);

        Assert.AreEqual(modDirectory, processorArguments[0] as string);

        var notificationCalls = notificationObserver.GetCalls();
        Assert.HasCount(1, notificationCalls);

        var notificationCall = notificationCalls[0];
        Assert.AreEqual(nameof(INotificationManager.AddNotification), notificationCall.StubbedMethod.Name);

        var notificationArguments = notificationCall.GetArguments();
        Assert.HasCount(1, notificationArguments);

        var notification = notificationArguments[0] as Notification;

        Assert.IsNotNull(notification);
        Assert.AreEqual("ModOrganizer", notification.Title);
        Assert.AreEqual($"Updated mod [{modDirectory}] path to [{newModPath}]", notification.MinimizedText);
    }
}
