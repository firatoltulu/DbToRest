using DbToRest.Core.Data.Provider;
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
        private Guid lastInsertedDocument = Guid.NewGuid();


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
                session.Store(new
                {
                    FirstName = "John",
                    LastName = "Doe"
                }, lastInsertedDocument.ToString());
                session.SaveChanges();
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

        //[Test, Order(3)]
        //public void Try_Delete_LastInsertedDocument()
        //{
        //    var result = dataProvider.Provider.Documents.DeleteAsync(lastInsertedDocument.Id, lastInsertedDocument.Rev);
        //    result.Wait();
        //    Assert.IsTrue(result.Result.IsSuccess);
        //}
    }
}
