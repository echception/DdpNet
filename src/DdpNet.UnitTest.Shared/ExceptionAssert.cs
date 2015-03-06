namespace DdpNet.UnitTest
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ExceptionAssert
    {
        public static void ExpectAggregateException(Action action, Type innerException)
        {
            bool exceptionCaught = false;
            try
            {
                action();
            }
            catch (AggregateException e)
            {
                exceptionCaught = true;
                Assert.AreEqual(innerException, e.InnerException.GetType());
            }

            Assert.IsTrue(exceptionCaught);
        }
    }
}
