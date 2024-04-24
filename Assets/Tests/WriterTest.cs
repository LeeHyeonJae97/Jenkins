using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class WriterTest
{
    [Test]
    public void TestToString()
    {
        Assert.AreEqual("8", new Writer().ToString(8));
    }
}
