using DbToRest.Core.Data.Provider;
using DbToRest.Tests.Model;
using MyCouch.Responses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbToRest.Tests.DataProvider
{
    [TestFixture]
    public class DataProviderTest : BaseTest
    {
        private string lastInsertedDocument ;


        private readonly IDataProvider dataProvider;
        public DataProviderTest()
        {
            dataProvider = DbToRest.Core.DbToRestContext.Current.Resolve<IDataProvider>();
        }

        [Test, Order(1)]
        public void Try_Post_ADocument()
        {
            using (var session = dataProvider.Database.OpenSession())
            {
                var entity = new TestDocumentModel
                {
                    FirstName = "John",
                    LastName = "Doe"
                };
                session.Store(entity);
                session.SaveChanges();

                lastInsertedDocument = entity.Id;

            }
            Assert.IsTrue(true);
        }

        [Test, Order(2)]
        public void Try_Get_LastInsertedDocument()
        {
            using (var session = dataProvider.Database.OpenSession())
            {
                dynamic obj = session.Load<dynamic>(lastInsertedDocument.ToString());
                Assert.IsNotNull(obj);
            }
        }

        [Test, Order(3)]
        public void Try_Delete_LastInsertedDocument()
        {
            using (var session = dataProvider.Database.OpenSession())
            {
                session.Delete(lastInsertedDocument);
            }
        }
    }
}
