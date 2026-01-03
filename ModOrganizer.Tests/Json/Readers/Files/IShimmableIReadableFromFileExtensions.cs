using ModOrganizer.Json.Readers.Files.Fakes;

namespace ModOrganizer.Tests.Json.Readers.Files;

public static class IShimmableIReadableFromFileExtensions
{
    public static S WithIReadableFromFileTryReadFromFile<S, T>(this S shimmable, T? value) where S: IShimmableIReadableFromFile where T : class
    {
        shimmable.OnShimsContext += () =>
        {
            ShimIReadableFromFileExtensions.TryReadFromFileOf1IReadableFromFileOfM0StringM0Out<T>((readableFile, path, out instance) =>
            {
                instance = value!;
                return value != null;
            });
        };

        return shimmable;
    }
}
