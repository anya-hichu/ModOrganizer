using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Commands;
using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Systems;

namespace ModOrganizer.Tests.Commands;

[TestClass]
public class TestCommand
{
    private static readonly string EXPECTED_COMMAND_NAME = "/modorganizer";
    private static readonly string EXPECTED_HELP_MESSAGE = "Available subcommands for /modorganizer are about, backup, config (export|import)?, main and preview";

    [TestMethod]
    public void TestHandler()
    {
        var observer = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(observer)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .Build();

        var beforeCalls = observer.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var beforeCall = beforeCalls[0];
        Assert.AreEqual(nameof(ICommandManager.AddHandler), beforeCall.StubbedMethod.Name);
        Assert.AreEqual(EXPECTED_COMMAND_NAME, beforeCall.GetArguments()[0] as string);

        var commandInfo = beforeCall.GetArguments()[1] as CommandInfo;
        Assert.IsNotNull(commandInfo);
        Assert.AreEqual(EXPECTED_HELP_MESSAGE, commandInfo.HelpMessage);

        command.Dispose();

        var afterCalls = observer.GetCalls();
        Assert.HasCount(2, afterCalls);

        var afterCall = afterCalls[1];
        Assert.AreEqual(nameof(ICommandManager.RemoveHandler), afterCall.StubbedMethod.Name);
        Assert.AreEqual(EXPECTED_COMMAND_NAME, afterCall.GetArguments()[0] as string);
    }

    [TestMethod]
    public void TestHelp()
    {
        var managerObserver = new StubObserver();

        var printerObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithCommandPrinterDefaults()
            .WithCommandPrinterObserver(printerObserver)
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(EXPECTED_COMMAND_NAME, string.Empty);

        var afterCalls = printerObserver.GetCalls();
        Assert.HasCount(1, afterCalls);

        var beforeCall = afterCalls[0];
        Assert.AreEqual(nameof(ICommandPrinter.PrintError), beforeCall.StubbedMethod.Name);
        Assert.AreEqual(EXPECTED_HELP_MESSAGE, beforeCall.GetArguments()[0] as string);
    }

    [TestMethod]
    public void TestToggleAboutWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleAboutWindow(ActionDecorator.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(EXPECTED_COMMAND_NAME, "about");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestToggleBackupWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleBackupWindow(ActionDecorator.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(EXPECTED_COMMAND_NAME, "backup");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestToggleConfigWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleConfigWindow(ActionDecorator.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(EXPECTED_COMMAND_NAME, "config");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestToggleConfigExportWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleConfigExportWindow(ActionDecorator.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(EXPECTED_COMMAND_NAME, "config export");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestToggleConfigImportWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleConfigImportWindow(ActionDecorator.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(EXPECTED_COMMAND_NAME, "config import");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestToggleMainWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleMainWindow(ActionDecorator.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(EXPECTED_COMMAND_NAME, "main");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestTogglePreviewWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithTogglePreviewWindow(ActionDecorator.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(EXPECTED_COMMAND_NAME, "preview");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }
}
