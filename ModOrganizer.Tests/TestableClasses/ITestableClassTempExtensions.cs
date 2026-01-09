namespace ModOrganizer.Tests.TestableClasses;

public static class ITestableClassTempExtensions
{
    public static string CreateResultsTempDirectory(this ITestableClassTemp testable)
    {
        var textContext = testable.TestContext;

        return Directory.CreateDirectory(Path.Combine(textContext.ResultsDirectory!, HashCode.Combine(textContext.TestName, textContext.TestData).ToString())).FullName;
    }
}
