namespace DdpNet.UnitTest.Results
{
    using DdpNet.Results;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ResultFilterFactoryTests
    {
        [TestMethod]
        public void ResultFilterFactory_CreateSubscribeResultFilter_CorrectFilter()
        {
            var resultFilter = ResultFilterFactory.CreateSubscribeResultFilter("SubID");

            var subFilter = resultFilter as SubscribeResultFilter;

            Assert.IsNotNull(subFilter);
        }

        [TestMethod]
        public void ResultFilterFactory_CreateConnectResultFilter_CorrectFilter()
        {
            var resultFilter = ResultFilterFactory.CreateConnectResultFilter();

            var connectFilter = resultFilter as ConnectedResultFilter;

            Assert.IsNotNull(connectFilter);
        }

        [TestMethod]
        public void ResultFilterFactory_CreateCallResultFilter_CorrectFilter()
        {
            var resultFilter = ResultFilterFactory.CreateCallResultFilter("Method");

            var callFilter = resultFilter as MethodCallResultFilter;

            Assert.IsNotNull(callFilter);
        }
    }
}
