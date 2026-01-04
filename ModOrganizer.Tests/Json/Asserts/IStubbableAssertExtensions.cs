using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using System.Text.Json;

namespace ModOrganizer.Tests.Json.Asserts;

public static class IStubbableAssertExtensions
{
    public static T WithAssertObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableAssert
    {
        stubbable.AssertStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithAssertIsValue<T>(this T stubbable, bool stubValue) where T : IStubbableAssert
    {
        stubbable.AssertStub.IsValueJsonElementJsonValueKind = (element, kind) => stubValue;

        return stubbable;
    }

    public static T WithAssertIsOptionalValue<T>(this T stubbable, string? stubValue, bool success) where T : IStubbableAssert
    {
        stubbable.AssertStub.IsOptionalValueJsonElementStringStringOut = (element, propertyName, out value) =>
        {
            value = stubValue;
            return success;
        };

        return stubbable;
    }

    public static T WithAssertIsOptionalValueSuccessOnTrue<T>(this T stubbable) where T : IStubbableAssert
    {
        stubbable.AssertStub.IsOptionalValueJsonElementStringStringOut = (element, propertyName, out value) =>
        {
            value = element.GetProperty(propertyName).GetBoolean() ? string.Empty : null;
            return value != null;
        };

        return stubbable;
    }

    public static T WithAssertIsStringDict<T>(this T stubbable, Dictionary<string, string>? stubValue) where T : IStubbableAssert
    {
        stubbable.AssertStub.IsStringDictJsonElementDictionaryOfStringStringOut = (element, out value) =>
        {
            value = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }

    public static T WithAssertIsStringDictSuccessOnTrue<T>(this T stubbable) where T : IStubbableAssert
    {
        stubbable.AssertStub.IsStringDictJsonElementDictionaryOfStringStringOut = (element, out value) =>
        {
            value = element.GetBoolean() ? [] : null;
            return value != null;
        };

        return stubbable;
    }

    public static T WithAssertIsStringArray<T>(this T stubbable, string[]? stubValue) where T : IStubbableAssert
    {
        stubbable.AssertStub.IsStringArrayJsonElementStringArrayOut = (element, out value) =>
        {
            value = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
