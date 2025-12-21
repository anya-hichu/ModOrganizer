namespace ModOrganizer.Tests.Shared;

public static class TestContextExtensions
{
    public static string CreateTestTempDirectory(this TestContext context) => Directory.CreateDirectory(Path.Combine(context.ResultsDirectory!, HashCode.Combine(context.TestName, context.TestData).ToString())).FullName;
}
