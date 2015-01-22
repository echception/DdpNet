namespace DdpNet.UnitTest.Collections.ObjectChangerTestObjects
{
    using Newtonsoft.Json.Linq;

    public class ComplexObject
    {
        public float testField;

        public bool TestBoolean { get; set; }

        public NestedObject NestedObject { get; set; }

        public ComplexObject CreateCopy()
        {
            return new ComplexObject()
            {
                TestBoolean = this.TestBoolean,
                testField = this.testField,
                NestedObject = this.NestedObject.CreateCopy()
            };
        }

        public JToken CreateJToken()
        {
            return JToken.FromObject(this);
        }
    }

    public class NestedObject
    {
        public string testfield;
        public int TestProperty { get; set; }

        public NestedObject CreateCopy()
        {
            return new NestedObject()
            {
                testfield = this.testfield,
                TestProperty = this.TestProperty
            };
        }
    }
    
}
