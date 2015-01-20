namespace DdpNet.UnitTest.Collections.ObjectChangerTestObjects
{
    using Newtonsoft.Json;

    internal class SimpleField
    {
        public int integerField;

        public string stringField;

        public float floatField;

        public bool boolField;

        [JsonProperty(PropertyName = "NAMEINATTRIBUTE")]
        public string actualFieldNameIsInAttribute;

        public SimpleField CreateCopy()
        {
            return new SimpleField
            {
                boolField = this.boolField,
                stringField = this.stringField,
                floatField = this.floatField,
                integerField = this.integerField,
                actualFieldNameIsInAttribute = this.actualFieldNameIsInAttribute
            };
        }
    }
}
