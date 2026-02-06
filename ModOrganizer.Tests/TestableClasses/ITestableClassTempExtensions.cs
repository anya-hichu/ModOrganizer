namespace ModOrganizer.Tests.TestableClasses;

public static class ITestableClassTempExtensions
{
    public static string CreateResultsTempDirectory(this ITestableClassTemp testable)
    {
        var testContext = testable.TestContext;

        var resultsDirectoryName = HashCode.Combine(testContext.TestName, testContext.TestData).ToString();
        var resultsTempDirectory = Directory.CreateDirectory(Path.Combine(testContext.ResultsDirectory!, resultsDirectoryName));

        return resultsTempDirectory.FullName;
    }
}
