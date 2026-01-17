using Dalamud.Bindings.ImGui;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.Bindings.ImGuis;
using ModOrganizer.Virtuals;
using ModOrganizer.Windows.Results.Rules;

namespace ModOrganizer.Tests.Windows.Previews;

[TestClass]
public class TestPreviewWindow
{
    [TestMethod]
    [DoNotParallelize]
    public void TestDraw()
    {
        var observer = new StubObserver();

        using var previewWindowResource = new PreviewWindowResourceBuilder()
            .WithImGuiDefaults()
            .WithImGuiObserver(observer)
            .WithRuleResultFileSystemRootFolder(new())
            .Build();

        previewWindowResource.Value.Draw();

        var calls = observer.GetCalls();

        Assert.HasCount(6, calls);

        Assert.AreEqual(nameof(ImGui.InputTextWithHint), calls[0].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.SameLine), calls[1].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.Button), calls[2].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.GetWindowWidth), calls[3].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.SameLine), calls[4].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.Checkbox), calls[5].StubbedMethod.Name);
    }

    [TestMethod]
    [DoNotParallelize]
    public void TestDrawWithResult()
    {
        var observer = new StubObserver();

        var folderName = "Folder Name";

        var resultName = "Result Name";
        var resultDirectory = "Result Directory";
        var resultNewPath = "Result New Path";

        var rootFolder = new VirtualFolder()
        {
            Folders = [
                new() 
                {
                    Name = folderName,
                    Path = "Folder Path",
                    Files = [
                        new()
                        {
                            Name = resultName,
                            Directory = resultDirectory,
                            Path = resultNewPath
                        }
                    ]
                }
            ]
        };

        var fileData = new RulePathResult()
        {
            Directory = resultDirectory,
            Path = "Result Path",
            NewPath = resultNewPath
        };

        using var previewWindowResource = new PreviewWindowResourceBuilder()
            .WithImGuiDefaults()
            .WithImGuiObserver(observer)
            .WithImGuiTreeNodeEx(true)
            .WithRuleResultFileSystemRootFolder(rootFolder)
            .WithRuleResultFileSystemTryGetFileData(fileData)
            .Build();

        previewWindowResource.Value.Draw();

        var calls = observer.GetCalls();
        Assert.HasCount(18, calls);

        // TODO: Add label checks
        Assert.AreEqual(nameof(ImGui.InputTextWithHint), calls[0].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.SameLine), calls[1].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.Button), calls[2].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.GetWindowWidth), calls[3].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.SameLine), calls[4].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.Checkbox), calls[5].StubbedMethod.Name);

        Assert.AreEqual(nameof(ImGui.GetColorU32), calls[6].StubbedMethod.Name);
        Assert.AreEqual(nameof(ImGui.PushStyleColor), calls[7].StubbedMethod.Name);
        {
            Assert.AreEqual(nameof(ImGui.TreeNodeEx), calls[8].StubbedMethod.Name);
            Assert.StartsWith($"{folderName}###resultVirtualFolder", calls[8].GetArguments()[0].ToString());
            {
                Assert.AreEqual(nameof(ImGui.GetColorU32), calls[9].StubbedMethod.Name);
                Assert.AreEqual(nameof(ImGui.PushStyleColor), calls[10].StubbedMethod.Name);
                {
                    Assert.AreEqual(nameof(ImGui.TreeNodeEx), calls[11].StubbedMethod.Name);
                    Assert.StartsWith($"{resultName}###resultVirtualFile", calls[11].GetArguments()[0].ToString());
                    {
                        Assert.AreEqual(nameof(ImGui.IsItemClicked), calls[12].StubbedMethod.Name);
                        Assert.AreEqual(nameof(ImGui.IsItemHovered), calls[13].StubbedMethod.Name);
                    }
                    Assert.AreEqual(nameof(ImGui.TreePop), calls[14].StubbedMethod.Name);
                }
                Assert.AreEqual(nameof(ImGui.PopStyleColor), calls[15].StubbedMethod.Name);
            }
            Assert.AreEqual(nameof(ImGui.TreePop), calls[16].StubbedMethod.Name);
        }
        Assert.AreEqual(nameof(ImGui.PopStyleColor), calls[17].StubbedMethod.Name);
    }
}
