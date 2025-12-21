namespace ModOrganizer.Tests;

public abstract class TestClass
{
    public TestContext TestContext { get; set; } = null!;

    protected string CreateResultsTempDirectory() => Directory.CreateDirectory(Path.Combine(TestContext.ResultsDirectory!, HashCode.Combine(TestContext.TestName, TestContext.TestData).ToString())).FullName;
}
