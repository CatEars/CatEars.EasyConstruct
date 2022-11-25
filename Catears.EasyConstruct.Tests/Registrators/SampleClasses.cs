﻿namespace Catears.EasyConstruct.Tests.Registrators;

public enum TestEnum
{
    A, B, C, D, E
}

[TestableForRegistration]
public static class ClassThatIsStatic {}

[TestableForRegistration]
public record RecordWithSingleConstructor;

[TestableForRegistration]
public abstract record RecordThatIsAbstract {}

[TestableForRegistration]
public sealed record RecordThatIsSealed {}

[TestableForRegistration]
public record RecordWithMultipleConstructors(string Value)
{
    public RecordWithMultipleConstructors() : this("Sample Value") {}
}

[TestableForRegistration]
public record RecordWithSingleMarkedConstructor
{
    [EasyConstruct.PreferredConstructor]
    public RecordWithSingleMarkedConstructor() {}
}

[TestableForRegistration]
public record RecordWithSingleMarkedConstructorAmongMultipleConstructors(string Value)
{
    [EasyConstruct.PreferredConstructor]
    public RecordWithSingleMarkedConstructorAmongMultipleConstructors() : this("Sample Value") {}
}

[TestableForRegistration]
public record RecordWithSingleConstructorWithMultiplePrimitiveParameters(
    string StringValue, int IntValue, TestEnum EnumValue, float FloatValue,
    bool BoolValue, byte ByteValue, char CharValue, decimal DecimalValue,
    double DoubleValue, long LongValue, sbyte SByteValue, short ShortValue,
    uint UIntValue, ulong ULongValue, ushort UShortValue);


[TestableForRegistration]
public record RecordWithSingleConstructorContainingComplexParameter(RecordWithSingleConstructor _);

[TestableForRegistration]
public class ClassWithSingleConstructor {}

[TestableForRegistration]
public abstract class ClassThatIsAbstract {}

[TestableForRegistration]
public sealed class ClassThatIsSealed {}

[TestableForRegistration]
public class ClassWithMultipleConstructors
{
    public string Value { get; }

    public ClassWithMultipleConstructors()
    {
        Value = "Sample Value";
    }

    public ClassWithMultipleConstructors(string value)
    {
        Value = value;
    }
}

[TestableForRegistration]
public class ClassWithSingleMarkedConstructor
{
    [EasyConstruct.PreferredConstructor]
    public ClassWithSingleMarkedConstructor() {}
}

[TestableForRegistration]
public class ClassWithSingleMarkedConstructorAmongMultipleConstructors
{
    public string Value { get; }

    [EasyConstruct.PreferredConstructor]
    public ClassWithSingleMarkedConstructorAmongMultipleConstructors()
    {
        Value = "Sample Value";
    }

    public ClassWithSingleMarkedConstructorAmongMultipleConstructors(string value)
    {
        Value = value;
    }
}

[TestableForRegistration]
public class ClassWithSingleConstructorWithMultiplePrimitiveParameters
{
    public ClassWithSingleConstructorWithMultiplePrimitiveParameters(
        string StringValue, int IntValue, TestEnum EnumValue, float FloatValue,
        bool BoolValue, byte ByteValue, char CharValue, decimal DecimalValue,
        double DoubleValue, long LongValue, sbyte SByteValue, short ShortValue,
        uint UIntValue, ulong ULongValue, ushort UShortValue)
    {
        
    }
}


[TestableForRegistration]
public class ClassWithSingleConstructorContainingComplexParameter
{
    public ClassWithSingleConstructorContainingComplexParameter(RecordWithSingleConstructor _)
    {
    }
}

[TestableForRegistration]
public struct StructWithNoConstructor {}

[TestableForRegistration]
public struct StructWithSingleConstructor
{
    public StructWithSingleConstructor() {}
}

[TestableForRegistration]
public struct StructWithMultipleConstructors
{
    public string Value { get; }

    public StructWithMultipleConstructors()
    {
        Value = "Sample Value";
    }

    public StructWithMultipleConstructors(string value)
    {
        Value = value;
    }
}

[TestableForRegistration]
public struct StructWithSingleMarkedConstructor
{
    [EasyConstruct.PreferredConstructor]
    public StructWithSingleMarkedConstructor() {}
}


[TestableForRegistration]
public struct StructWithSingleMarkedConstructorAmongMultipleConstructors
{
    public string Value { get; }
    
    [EasyConstruct.PreferredConstructor]
    public StructWithSingleMarkedConstructorAmongMultipleConstructors()
    {
        Value = "Sample Value";
    }

    public StructWithSingleMarkedConstructorAmongMultipleConstructors(string value)
    {
        Value = value;
    }
}

[TestableForRegistration]
public struct StructWithSingleConstructorWithMultiplePrimitiveParameters
{
    public StructWithSingleConstructorWithMultiplePrimitiveParameters(
        string StringValue, int IntValue, TestEnum EnumValue, float FloatValue,
        bool BoolValue, byte ByteValue, char CharValue, decimal DecimalValue,
        double DoubleValue, long LongValue, sbyte SByteValue, short ShortValue,
        uint UIntValue, ulong ULongValue, ushort UShortValue)
    {
        
    }
}

[TestableForRegistration]
public struct StructWithSingleConstructorContainingComplexParameter
{
    public StructWithSingleConstructorContainingComplexParameter(RecordWithSingleConstructor _)
    {
    }
}
