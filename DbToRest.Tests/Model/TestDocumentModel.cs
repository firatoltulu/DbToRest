using System;
using System.Collections.Generic;
using System.Text;

namespace DbToRest.Tests.Model
{
    public class TestDocumentModel
    {
        public TestDocumentModel()
        {
            Id = string.Empty;
        }

        public string Id { get; set; } = string.Empty;

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}
