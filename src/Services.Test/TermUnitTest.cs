// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Text.Json;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Marketplace.SaaS.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Marketplace.SaaS.Accelerator.Services.Test;

[TestClass]
public class TermUnitTest
{
    [TestMethod]
    [DataRow("P18M", "18 months")]
    [DataRow("P1Y6M", "1 year 6 months")]
    [DataRow("P4Y", "4 years")]
    [DataRow("P1Y", "1 year")]
    [DataRow("P1M", "1 month")]
    [DataRow("P2Y3M", "2 years 3 months")]
    public void ToDisplayString_YearMonthDuration_ReturnsExpectedText(string value, string expected)
    {
        var termUnit = new TermUnit(value);

        Assert.AreEqual(expected, termUnit.ToDisplayString());
    }

    [TestMethod]
    public void ToDisplayString_UnrecognizedFormat_FallsBackToRawValue()
    {
        var termUnit = new TermUnit("PT1H30M");

        Assert.AreEqual("PT1H30M", termUnit.ToDisplayString());
    }

    [TestMethod]
    public void ImplicitStringConversion_RoundTripsAnyValue()
    {
        TermUnit termUnit = "P18M";

        Assert.AreEqual("P18M", termUnit.ToString());
    }

    [TestMethod]
    public void Equals_IsCaseInsensitive()
    {
        Assert.AreEqual(new TermUnit("p1m"), new TermUnit("P1M"));
    }

    [TestMethod]
    public void Read_TermResult_PreservesCustomDuration()
    {
        var json = "{\"termUnit\":\"P18M\"}";

        var result = JsonSerializer.Deserialize<TermResult>(json);

        Assert.AreEqual("P18M", result.TermUnit.ToString());
        Assert.AreEqual("18 months", result.TermUnit.ToDisplayString());
    }

    [TestMethod]
    public void Write_TermResult_SerializesRawValue()
    {
        var term = new TermResult { TermUnit = new TermUnit("P18M") };

        var json = JsonSerializer.Serialize(term);

        StringAssert.Contains(json, "\"termUnit\":\"P18M\"");
    }
}
