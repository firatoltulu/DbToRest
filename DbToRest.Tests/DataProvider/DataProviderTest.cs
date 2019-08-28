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
        private DocumentHeaderResponse lastInsertedDocument = null;


        private readonly IDataProvider dataProvider;
        public DataProviderTest()
        {
            dataProvider = DbToRest.Core.DbToRestContext.Current.Resolve<IDataProvider>();
        }

        //[Test, Order(1)]
        //public void Try_Post_ADocument()
        //{
        //    var result = dataProvider.Provider.Documents.PostAsync("{\"name\":\"TCFOLTULU\"}");
        //    result.Wait();

        //    lastInsertedDocument = result.Result;
        //    Assert.IsTrue(result.IsCompletedSuccessfully);
        //}

        //[Test, Order(2)]
        //public void Try_Get_LastInsertedDocument()
        //{
        //    var result = dataProvider.Provider.Documents.GetAsync(lastInsertedDocument.Id);
        //    result.Wait();
        //    Assert.IsTrue(result.Result.Content.Contains("TCFOLTULU"));
        //}

        //[Test, Order(3)]
        //public void Try_Delete_LastInsertedDocument()
        //{
        //    var result = dataProvider.Provider.Documents.DeleteAsync(lastInsertedDocument.Id,lastInsertedDocument.Rev);
        //    result.Wait();
        //    Assert.IsTrue(result.Result.IsSuccess);
        //}
    }
}
