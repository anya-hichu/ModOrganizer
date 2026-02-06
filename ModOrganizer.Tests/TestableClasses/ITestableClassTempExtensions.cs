namespace ModOrganizer.Tests.TestableClasses;

public static class ITestableClassTempExtensions
{
    public static string CreateResultsTempDirectory(this ITestableClassTemp testable)
    {
        var testContext = testable.TestContext;

        var testDirectoryName = HashCode.Combine(testContext.TestName, testContext.TestData).ToString();
        var directory = Directory.CreateDirectory(Path.Combine(testContext.ResultsDirectory!, testDirectoryName));

        return directory.FullName;
    }
}
