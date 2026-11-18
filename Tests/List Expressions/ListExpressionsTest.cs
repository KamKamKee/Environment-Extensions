/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using System.Collections.Generic;
using System.Linq;
using EExtensions.ListExpressions;
using NUnit.Framework;

public class ListExpressionsTest
{
    [TestCase(new int[] { 1, 5, 9 }, ", ", "1, 5, 9")]
    [TestCase(new int[] { 1 }, ", ", "1")]
    [TestCase(new int[] {}, ", ", "")]
    [TestCase(new int[] { 1, 5, 9 }, " | ", "1 | 5 | 9")]
    [TestCase(new int[] { 1, 5, 9 }, "", "159")]
    public void TestJoin(int[] input, string seperator, string expected)
    {
        List<int> listInput = input.ToList();
        Assert.AreEqual(listInput.Join(seperator), expected);
    }

    [TestCase(new int[] { 1, 5, 9 }, "List { 1, 5, 9 }")]
    [TestCase(new int[] { 1 }, "List { 1 }")]
    [TestCase(new int[] { }, "List { EMPTY }")]
    public void TestPrintable(int[] input, string expected)
    {
        List<int> listInput = input.ToList();
        Assert.AreEqual(listInput.Printable(), expected);
    }

    [TestCase(new int[] { 1, 5, 9 }, "myInts", "List myInts { 1, 5, 9 }")]
    [TestCase(new int[] { 1 }, "singleInt", "List singleInt { 1 }")]
    [TestCase(new int[] { }, "emptyList" , "List emptyList { EMPTY }")]
    public void TestPrintableWithName(int[] input, string displayName, string expected)
    {
        List<int> listInput = input.ToList();
        Assert.AreEqual(listInput.Printable(displayName), expected);
    }

    [TestCase(new int[] { 1, 5, 9, 12, 15, 90 }, new int[] { 1, 5, 9 })]
    [TestCase(new int[] { 1, 5, 9 }, new int[] { 1, 5, 9 })]
    [TestCase(new int[] { 12, 1, 15, 5, 9, 55 }, new int[] { 1, 5, 9 })]
    [TestCase(new int[] { 12, 1, 15, 55 }, new int[] { 1 })]
    [TestCase(new int[] { 12, 15, 55, 150 }, new int[] { })]
    public void TestFilterLower12(int[] input, int[] expected)
    {
        List<int> listInput = input.ToList();
        List<int> listExpected = expected.ToList();
        Assert.AreEqual(listInput.Filter((int value) => { return value < 12; }), listExpected);

        if (input.Equals(expected))
            Assert.AreNotEqual(listInput, input.ToList());
    }

    [TestCase(new int[] { 1, 5, 9, 12, 15, 90 }, new int[] { 1, 5, 9 })]
    [TestCase(new int[] { 1, 5, 9 }, new int[] { 1, 5, 9 })]
    [TestCase(new int[] { 12, 1, 15, 5, 9, 55 }, new int[] { 1, 5, 9 })]
    [TestCase(new int[] { 12, 1, 15, 55 }, new int[] { 1 })]
    [TestCase(new int[] { 12, 15, 55, 150 }, new int[] { })]
    public void TestFilteredLower12(int[] input, int[] expected)
    {
        List<int> listInput = input.ToList();
        List<int> listExpected = expected.ToList();
        Assert.AreEqual(listInput.Filtered((int value) => { return value < 12; }), listExpected);
        Assert.AreEqual(listInput, input.ToList());
    }

    [TestCase(new int[] { 1, 5, 9 }, new int[] { 2, 10, 18 })]
    [TestCase(new int[] { }, new int[] { })]
    [TestCase(new int[] { 5 }, new int[] { 10 })]
    public void MapTimes2(int[] input, int[] expected)
    {
        List<int> listInput = input.ToList();
        List<int> listExpected = expected.ToList();
        Assert.AreEqual(listInput.Map((int value) => { return value * 2; }), listExpected);

        if (input.Equals(expected))
            Assert.AreNotEqual(listInput, input.ToList());
    }

    [TestCase(new int[] { 1, 5, 9 }, new int[] { 2, 10, 18 })]
    [TestCase(new int[] { }, new int[] { })]
    [TestCase(new int[] { 5 }, new int[] { 10 })]
    public void MappedTimes2(int[] input, int[] expected)
    {
        List<int> listInput = input.ToList();
        List<int> listExpected = expected.ToList();
        Assert.AreEqual(listInput.Mapped((int value) => { return value * 2; }), listExpected);
        Assert.AreEqual(listInput, input.ToList());
    }

    [TestCase(new int[] { 1, 5, 9 }, new string[] { "1", "5", "9" })]
    [TestCase(new int[] { }, new string[] { })]
    [TestCase(new int[] { 5 }, new string[] { "5" })]
    public void SwitchMappedToString(int[] input, string[] expected)
    {
        List<int> listInput = input.ToList();
        List<string> listExpected = expected.ToList();
        Assert.AreEqual(listInput.SwitchMapped((int value) => { return value.ToString(); }), listExpected);
        Assert.AreEqual(listInput, input.ToList());
    }

    [TestCase(new int[] { 1, 5, 9 }, new string[] { "2", "10", "18" })]
    [TestCase(new int[] { }, new string[] { })]
    [TestCase(new int[] { 5 }, new string[] { "10" })]
    public void SwitchMappedTimes2ToString(int[] input, string[] expected)
    {
        List<int> listInput = input.ToList();
        List<string> listExpected = expected.ToList();
        Assert.AreEqual(listInput.SwitchMapped((int value) => { return (value * 2).ToString(); }), listExpected);
        Assert.AreEqual(listInput, input.ToList());
    }
}
