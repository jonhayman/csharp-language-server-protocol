﻿using System;
using FluentAssertions;
using Lsp.Capabilities.Server;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextDocumentChangeRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentChangeRegistrationOptions() {
                DocumentSelector = new DocumentSelector(new DocumentFilter() {
                    Language = "csharp"
                }),
                SyncKind = TextDocumentSyncKind.Full
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextDocumentChangeRegistrationOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}