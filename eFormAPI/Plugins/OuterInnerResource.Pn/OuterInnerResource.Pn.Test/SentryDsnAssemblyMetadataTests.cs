/*
The MIT License (MIT)

Copyright (c) 2007 - 2026 Microting A/S

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace MachineArea.Pn.Test
{
    /// <summary>
    /// The API host routes this plugin's Sentry events to the plugin's own project, which it finds through
    /// <c>[assembly: AssemblyMetadata("SentryDsn", ...)]</c> on the plugin assembly (an
    /// <c>AssemblyMetadata</c> item in the csproj). Only the shape is asserted; the value lives in the csproj.
    /// </summary>
    [TestFixture]
    public class SentryDsnAssemblyMetadataTests
    {
        [Test]
        public void PluginAssembly_DeclaresExactlyOneWellFormedSentryDsn()
        {
            var values = typeof(OuterInnerResource.Pn.EformOuterInnerResourcePlugin).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(x => x.Key == "SentryDsn")
                .Select(x => x.Value)
                .ToList();

            Assert.That(values, Has.Count.EqualTo(1), "exactly one SentryDsn assembly metadata value");
            Assert.That(Uri.TryCreate(values[0], UriKind.Absolute, out var dsn), Is.True, "absolute URI");
            Assert.That(dsn!.Scheme, Is.EqualTo(Uri.UriSchemeHttps));
            Assert.That(dsn.UserInfo, Is.Not.Empty, "public key in the user info");
            Assert.That(dsn.AbsolutePath.Trim('/'), Does.Match(@"^\d+$"), "numeric project id as the path");
            Assert.That(dsn.Host, Does.Match(@"^o\d+\.ingest(\.[a-z]+)?\.sentry\.io$"), "sentry.io ingest host");
        }
    }
}
