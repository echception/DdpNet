namespace DdpNet.FunctionalTest.DataObjects
{
    using System;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
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

        public static Entry Random()
        {
            Random r = new Random();

            var name = Guid.NewGuid().ToString("N");
            var count = r.Next();

            bool isActive = r.Next(2) != 0;

            return new Entry() {Name = name, Count = count, IsActive = isActive};
        }
    }
}
