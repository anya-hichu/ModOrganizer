using ModOrganizer.Json.Readers.Files.Fakes;

namespace ModOrganizer.Tests.Json.Readers.Files;

public static class IShimmableIReadableFileExtensions
{
    public static S WithIReadableFileTryReadFromFile<S, T>(this S builder, T? value) where S: IShimmableIReadableFile where T : class
    {
        ShimIReadableFileExtensions.TryReadFromFileOf1IReadableFileOfM0StringM0Out<T>((readableFile, path, out instance) =>
        {
            instance = value!;
            return value != null;
        });

        return builder;
    }
}
