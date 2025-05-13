﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MSTestCombinatorial;

// Adopted from https://github.com/AArnott/Xunit.Combinatorial/blob/78ee72cb6ad53ebdbf85c824ec38205ccd5986e7/src/Xunit.Combinatorial/CombinatorialRangeAttribute.cs

/// <summary>
/// Specifies which range of values for this parameter should be used for running the test method.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class CombinatorialRangeAttribute : Attribute, ICombinatorialValuesProvider
{
    private readonly object[] _values;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinatorialRangeAttribute"/> class.
    /// </summary>
    /// <param name="from">The value at the beginning of the range.</param>
    /// <param name="count">
    /// The quantity of consecutive integer values to include.
    /// Cannot be less than 1, which would conceptually result in zero test cases.
    /// </param>
    public CombinatorialRangeAttribute(int from, int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        object[] values = new object[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = from + i;
        }

        _values = values;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinatorialRangeAttribute"/> class.
    /// </summary>
    /// <param name="from">The value at the beginning of the range.</param>
    /// <param name="to">
    /// The value at the end of the range.
    /// Cannot be less than "from" parameter.
    /// When "to" and "from" are equal, CombinatorialValues is more appropriate.
    /// </param>
    /// <param name="step">
    /// The number of integers to step for each value in result.
    /// Cannot be less than one. Stepping zero or backwards is not useful.
    /// Stepping over "to" does not add another value to the range.
    /// </param>
    public CombinatorialRangeAttribute(int from, int to, int step)
    {
        if (step > 0)
        {
            if (to < from)
            {
                throw new ArgumentOutOfRangeException(nameof(to));
            }
        }
        else if (step < 0)
        {
            if (to > from)
            {
                throw new ArgumentOutOfRangeException(nameof(to));
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(step));
        }

        int count = ((to - from) / step) + 1;
        object[] values = new object[count];
        for (int i = 0; i < count; i++)
        {
            values[i] = from + (i * step);
        }

        _values = values;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinatorialRangeAttribute"/> class.
    /// </summary>
    /// <param name="from">The value at the beginning of the range.</param>
    /// <param name="count">
    /// The quantity of consecutive integer values to include.
    /// Cannot be less than 1, which would conceptually result in zero test cases.
    /// </param>
    public CombinatorialRangeAttribute(uint from, uint count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        object[] values = new object[count];
        for (uint i = 0; i < count; i++)
        {
            values[i] = from + i;
        }

        _values = values;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CombinatorialRangeAttribute"/> class.
    /// </summary>
    /// <param name="from">The value at the beginning of the range.</param>
    /// <param name="to">
    /// The value at the end of the range.
    /// Cannot be less than "from" parameter.
    /// When "to" and "from" are equal, CombinatorialValues is more appropriate.
    /// </param>
    /// <param name="step">
    /// The number of unsigned integers to step for each value in result.
    /// Cannot be less than one. Stepping zero is not useful.
    /// Stepping over "to" does not add another value to the range.
    /// </param>
    public CombinatorialRangeAttribute(uint from, uint to, uint step)
    {
        if (step == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(step));
        }

        var values = new List<uint>();

        if (from < to)
        {
            for (uint i = from; i <= to; i += step)
            {
                values.Add(i);
            }
        }
        else
        {
            for (uint i = from; i >= to && i <= from; i -= step)
            {
                values.Add(i);
            }
        }

        _values = values.Cast<object>().ToArray();
    }

    /// <inheritdoc />
    public object[] GetValues(ParameterInfo parameter)
    {
        return _values;
    }
}