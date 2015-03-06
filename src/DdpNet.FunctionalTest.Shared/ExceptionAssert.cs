namespace DdpNet.FunctionalTest
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class ExceptionAssert
    {
        public static async Task AssertThrowsWithMessage<T>(Func<Task> action , params string[] expectedMessages) where T: Exception
        {
            bool thrown = false;

            try
            {
                await action();
            }
            catch (T e)
            {
                thrown = true;

                foreach (var message in expectedMessages)
                {
                    Assert.IsTrue(e.Message.Contains(message),
                        "Expected \"{0}\" in the exception message. Actual: {1}", message, e.Message);
                }
            }

            Assert.IsTrue(thrown);
        }

        public static async Task AssertDdpServerExceptionThrown(Func<Task> action, string expectedError)
        {
            bool thrown = false;

            try
            {
                await action();
            }
            catch (DdpServerException e)
            {
                thrown = true;

                Assert.AreEqual(expectedError, e.Error);
            }

            Assert.IsTrue(thrown);
        }
    }
}
