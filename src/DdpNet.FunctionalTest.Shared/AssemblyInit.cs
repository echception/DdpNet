namespace DdpNet.FunctionalTest
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssemblyInit
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            TestEnvironment.Cleanup();
            TestEnvironment.InitializeTestUser().Wait();
        }
    }
}
