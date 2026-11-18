/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using System.Linq;
using EExtensions.DataPersistance;
using NUnit.Framework;

public class DataPersistanceTests
{
    [TestCase("", "")]
    [TestCase("C:", "C:")]
    [TestCase("C:/Users/Test", "C:/Users/Test")]
    [TestCase("C:\\Users\\Test", "C:/Users/Test")]
    [TestCase("C:/Users/Test/", "C:/Users/Test")]
    [TestCase("C:/Users//Test", "C:/Users/Test")]
    public void TestPathCreateString(string input, string expected)
    {
        SimplePath path = new SimplePath(input);
        Assert.AreEqual(path.ToString(), expected);
    }

    [TestCase(new string[] { }, "")]
    [TestCase(new string[] { "" }, "")]
    [TestCase(new string[] { "C:" }, "C:")]
    [TestCase(new string[] { "C:", "Users", "Test" }, "C:/Users/Test")]
    [TestCase(new string[] { "C:/", "Users\\", "Test/" }, "C:/Users/Test")]
    [TestCase(new string[] { "", "" }, "")]
    [TestCase(new string[] { "", "/", "" }, "")]
    [TestCase(new string[] { "", "C:", "Users", "Test" }, "C:/Users/Test")]
    public void TestPathCreateArrayList(string[] input, string expected)
    {
        SimplePath pathByArray = new SimplePath(input);
        SimplePath pathByList = new SimplePath(input.ToList());

        Assert.AreEqual(pathByArray.ToString(), expected);
        Assert.AreEqual(pathByList.ToString(), expected);
    }
}
