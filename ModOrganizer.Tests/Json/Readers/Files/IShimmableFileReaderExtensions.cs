using ModOrganizer.Json.Readers.Files.Fakes;

namespace ModOrganizer.Tests.Json.Readers.Files;

public static class IShimmableFileReaderExtensions
{
    public static S WithFileReaderTryReadFromFile<S, T>(this S shimmable, T? value) where S: IShimmableFileReader where T : class
    {
        shimmable.OnShimsContext += () =>
        {
            ShimIFileReaderExtensions.TryReadFromFileOf1IFileReaderOfM0StringM0Out<T>((readableFile, path, out instance) =>
            {
                instance = value!;
                return value != null;
            });
        };

        return shimmable;
    }
}
