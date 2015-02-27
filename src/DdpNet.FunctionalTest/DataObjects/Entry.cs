namespace DdpNet.FunctionalTest.DataObjects
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class Entry : DdpObject
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public bool IsActive { get; set; }

        public static void AssertAreEqual(Entry expected, Entry actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
        }
    }
}
