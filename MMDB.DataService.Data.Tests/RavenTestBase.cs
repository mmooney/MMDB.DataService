using NUnit.Framework;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace MMDB.DataService.Data.Tests
{
    public class RavenTestBase
    {
        protected IDocumentSession DocumentSession { get; private set; }
        protected TransactionScope Transaction { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.DocumentSession = MMDB.DataService.Data.Tests.EmbeddedRavenProvider.DocumentStore.OpenSession();
            //this.Transaction = new TransactionScope();
        }

        [TearDown]
        public void TearDown()
        {
            //using(this.Transaction) {};
            //this.Transaction = null;
            using (this.DocumentSession) { };
            this.DocumentSession = null;
        }

    }
}
