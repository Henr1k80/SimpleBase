﻿/*
     Copyright 2014-2016 Sedat Kapanoglu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System;
using System.Text;
using SimpleBase;
using NUnit.Framework;

namespace SimpleBaseTest.Base32Test;

[TestFixture]
class Rfc4648Test
{
    private static readonly string[][] testData = new[]
    {
        new[] { "", "" },
        new[] {"f", "MY======" },
        new[] {"fo", "MZXQ====" },
        new[] {"foo", "MZXW6===" },
        new[] {"foob", "MZXW6YQ=" },
        new[] {"fooba", "MZXW6YTB" },
        new[] {"foobar", "MZXW6YTBOI======" },
        new[] {"foobar1", "MZXW6YTBOIYQ====" },
        new[] {"foobar12", "MZXW6YTBOIYTE===" },
        new[] {"foobar123", "MZXW6YTBOIYTEMY=" },
        new[] {"1234567890123456789012345678901234567890", "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ" },
    };

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Encode_ReturnsExpectedValues(string input, string expectedOutput)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(input);
        string result = Base32.Rfc4648.Encode(bytes, padding: true);
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    [TestCaseSource(nameof(testData))]
    public void Decode_ReturnsExpectedValues(string expectedOutput, string input)
    {
        var bytes = Base32.Rfc4648.Decode(input);
        string result = Encoding.ASCII.GetString(bytes.ToArray());
        Assert.That(result, Is.EqualTo(expectedOutput));
        bytes = Base32.Rfc4648.Decode(input.ToLowerInvariant());
        result = Encoding.ASCII.GetString(bytes.ToArray());
        Assert.That(result, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void Encode_NullBytes_ReturnsEmptyString()
    {
        Assert.That(Base32.Rfc4648.Encode(null, true), Is.EqualTo(String.Empty));
    }

    [Test]
    public void Decode_InvalidInput_ThrowsArgumentException()
    {
        _ = Assert.Throws<ArgumentException>(() => Base32.Rfc4648.Decode("[];',m."));
    }
}
