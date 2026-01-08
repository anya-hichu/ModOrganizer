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
    [TestMethod]
    public void TestHandle()
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
        Assert.AreEqual(Command.NAME, beforeCall.GetArguments()[0] as string);

        var commandInfo = beforeCall.GetArguments()[1] as CommandInfo;
        Assert.IsNotNull(commandInfo);
        Assert.AreEqual(Command.HELP_MESSAGE, commandInfo.HelpMessage);

        command.Dispose();

        var afterCalls = observer.GetCalls();
        Assert.HasCount(2, afterCalls);

        var afterCall = afterCalls[1];
        Assert.AreEqual(nameof(ICommandManager.RemoveHandler), afterCall.StubbedMethod.Name);
        Assert.AreEqual(Command.NAME, afterCall.GetArguments()[0] as string);
    }

    [TestMethod]
    public void TestHandleError()
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
        commandInfo.Handler.Invoke(Command.NAME, string.Empty);

        var afterCalls = printerObserver.GetCalls();
        Assert.HasCount(1, afterCalls);

        var beforeCall = afterCalls[0];
        Assert.AreEqual(nameof(ICommandPrinter.PrintError), beforeCall.StubbedMethod.Name);
        Assert.AreEqual(Command.HELP_MESSAGE, beforeCall.GetArguments()[0] as string);
    }

    [TestMethod]
    public void TestHandleToggleAboutWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleAboutWindow(StubAction.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(Command.NAME, "about");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestHandleToggleBackupWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleBackupWindow(StubAction.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(Command.NAME, "backup");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestHandleToggleConfigWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleConfigWindow(StubAction.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(Command.NAME, "config");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestHandleToggleConfigExportWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleConfigExportWindow(StubAction.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(Command.NAME, "config export");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestHandleToggleConfigImportWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleConfigImportWindow(StubAction.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(Command.NAME, "config import");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestHandleToggleMainWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithToggleMainWindow(StubAction.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(Command.NAME, "main");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }

    [TestMethod]
    public void TestHandleTogglePreviewWindow()
    {
        var managerObserver = new StubObserver();
        var toggleObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandManagerAddHandler(true)
            .WithCommandManagerRemoveHandler(true)
            .WithTogglePreviewWindow(StubAction.WithObserver(toggleObserver, () => { }))
            .Build();

        var beforeCalls = managerObserver.GetCalls();
        Assert.HasCount(1, beforeCalls);

        var commandInfo = beforeCalls[0].GetArguments()[1] as CommandInfo;

        Assert.IsNotNull(commandInfo);
        commandInfo.Handler.Invoke(Command.NAME, "preview");

        Assert.HasCount(1, toggleObserver.GetCalls());
    }
}
