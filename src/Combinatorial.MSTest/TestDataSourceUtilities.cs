﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Combinatorial.MSTest;

// Copy from https://github.com/microsoft/testfx/blob/47dee826a0a3eb7a2d9d089ed8aba9d2dabfe82e/src/TestFramework/TestFramework/Internal/TestDataSourceUtilities.cs
internal static class TestDataSourceUtilities
{
    public static string? ComputeDefaultDisplayName(MethodInfo methodInfo, object?[]? data)
    {
        if (data is null)
        {
            return null;
        }

        ParameterInfo[] parameters = methodInfo.GetParameters();

        // We want to force call to `data.AsEnumerable()` to ensure that objects are casted to strings (using ToString())
        // so that null do appear as "null". If you remove the call, and do string.Join(",", new object[] { null, "a" }),
        // you will get empty string while with the call you will get "null,a".
        IEnumerable<object?> displayData = parameters.Length == 1 && parameters[0].ParameterType == typeof(object[])
            ? [data.AsEnumerable()]
            : data.AsEnumerable();

        return $"{methodInfo.Name}({string.Join(",", displayData.Select(x => GetHumanizedArguments(x)))})";
    }

    /// <summary>
    /// Recursively resolve collections of objects to a proper string representation.
    /// </summary>
    /// <param name="data">The method arguments.</param>
    /// <returns>The humanized representation of the data.</returns>
    private static string? GetHumanizedArguments(object? data)
    {
        if (data is null)
        {
            return "null";
        }

        if (!data.GetType().IsArray)
        {
            return data switch
            {
                string s => $"\"{s}\"",
                char c => $"'{c}'",
                _ => data.ToString(),
            };
        }

        // We need to box the object here so that we can support value types
        IEnumerable<object> boxedObjectEnumerable = ((IEnumerable)data).Cast<object>();
        IEnumerable<string?> elementStrings = boxedObjectEnumerable.Select(x => GetHumanizedArguments(x));
        return $"[{string.Join(",", elementStrings)}]";
    }
}