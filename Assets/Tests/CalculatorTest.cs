using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CalculatorTest
{
    [Test]
    public void TestAdd()
    {
        Assert.AreEqual(Calculator.Add(3, 5), 8);
    }

    [Test]
    public void TestSubtract()
    {
        Assert.AreEqual(Calculator.Subtract(3, 5), -2);
    }
}
