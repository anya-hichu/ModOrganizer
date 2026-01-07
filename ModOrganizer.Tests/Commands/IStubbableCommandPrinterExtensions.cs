using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Commands;

public static class IStubbableCommandPrinterExtensions
{
    public static T WithCommandPrinterDefaults<T>(this T stubbable) where T : IStubbableCommandPrinter
    {
        stubbable.CommandPrinterStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithCommandPrinterObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableCommandPrinter
    {
        stubbable.CommandPrinterStub.InstanceObserver = observer;

        return stubbable;
    }
}
