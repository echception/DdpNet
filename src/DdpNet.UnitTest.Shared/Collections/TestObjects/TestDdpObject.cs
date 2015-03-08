namespace DdpNet.UnitTest.Collections.TestObjects
{
    using System.Diagnostics;
    [DebuggerDisplay("{integerField}")]
    public class TestDdpObject : DdpObject
    {
        public string StringProperty { get; set; }

        public int integerField;
    }
}
