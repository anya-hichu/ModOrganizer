using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Commands;
using ModOrganizer.Tests.Commands.Printers;
using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Windows.Togglers;
using ModOrganizer.Windows;
using ModOrganizer.Windows.Configs;
using ModOrganizer.Windows.Togglers;

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

        var beforeCallArguments = beforeCall.GetArguments();
        Assert.AreEqual(Command.NAME, beforeCallArguments[0] as string);

        var commandInfo = beforeCallArguments[1] as CommandInfo;
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
            .WithCommandManagerDefaults()
            .WithCommandManagerObserver(managerObserver)
            .WithCommandPrinterDefaults()
            .WithCommandPrinterObserver(printerObserver)
            .Build();

        var managerCalls = managerObserver.GetCalls();
        Assert.HasCount(1, managerCalls);

        var managerCall = managerCalls[0];
        Assert.AreEqual(nameof(ICommandManager.AddHandler), managerCall.StubbedMethod.Name);

        var commandInfo = managerCall.GetArguments()[1] as CommandInfo;
        Assert.IsNotNull(commandInfo);

        commandInfo.Handler.Invoke(Command.NAME, string.Empty);

        var printerCalls = printerObserver.GetCalls();
        Assert.HasCount(1, printerCalls);

        var printerCall = printerCalls[0];
        Assert.AreEqual(nameof(ICommandPrinter.PrintError), printerCall.StubbedMethod.Name);
        Assert.AreEqual(Command.HELP_MESSAGE, printerCall.GetArguments()[0] as string);
    }

    [TestMethod]
    [DataRow("about", typeof(AboutWindow))]
    [DataRow("backup", typeof(BackupWindow))]
    [DataRow("config", typeof(ConfigWindow))]
    [DataRow("main", typeof(MainWindow))]
    [DataRow("preview", typeof(PreviewWindow))]
    public void TestHandleToggleWindow(string arguments, Type expectedType)
    {
        var managerObserver = new StubObserver();

        var togglerObserver = new StubObserver();

        var command = new CommandBuilder()
            .WithCommandManagerDefaults()
            .WithCommandManagerObserver(managerObserver)
            .WithStubbableWindowTogglerDefaults()
            .WithStubbableWindowTogglerObserver(togglerObserver)
            .Build();

        var managerCalls = managerObserver.GetCalls();
        Assert.HasCount(1, managerCalls);

        var managerCall = managerCalls[0];
        Assert.AreEqual(nameof(ICommandManager.AddHandler), managerCall.StubbedMethod.Name);

        var commandInfo = managerCall.GetArguments()[1] as CommandInfo;
        Assert.IsNotNull(commandInfo);

        commandInfo.Handler.Invoke(Command.NAME, arguments);

        var togglerCalls = togglerObserver.GetCalls();
        Assert.HasCount(1, togglerCalls);

        var togglerCallMethod = togglerCalls[0].StubbedMethod;
        Assert.AreEqual(nameof(IWindowToggler.Toggle), togglerCallMethod.Name);
        Assert.AreEqual(expectedType, togglerCallMethod.GetGenericArguments()[0]);
    }
}
