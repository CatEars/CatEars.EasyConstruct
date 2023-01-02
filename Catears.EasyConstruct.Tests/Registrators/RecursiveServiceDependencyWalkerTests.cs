﻿using System;
using System.Collections.Generic;
using System.Linq;
using Catears.EasyConstruct.Registration;
using Xunit;

namespace Catears.EasyConstruct.Tests.Registrators;

public class RecursiveServiceDependencyWalkerTests
{

    internal record BasicRecord(string Value);

    internal record ComplexRecord(BasicRecord Inner);

    internal class SelfReferentialClass
    {
        public SelfReferentialClass(SelfReferentialClass? parent)
        {
            
        }
    }

    [Theory]
    [InlineData(typeof(BasicRecord), typeof(BasicRecord))]
    [InlineData(typeof(ComplexRecord), typeof(ComplexRecord), typeof(BasicRecord))]
    [InlineData(typeof(SelfReferentialClass), typeof(SelfReferentialClass))]
    public void ListDependencies_WithType_ReturnsExpectedListOfTypes(Type rootType, params Type[] expectedTypes)
    {
        var walker = new RecursiveServiceDependencyWalker(rootType);

        var result = walker.ListDependencies();

        Assert.Equal(expectedTypes, result.Select(x => x.ServiceToRegister).ToArray());
    }
    
}