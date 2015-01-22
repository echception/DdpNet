namespace DdpNet.UnitTest.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DdpNet.Collections;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Linq;
    using ObjectChangerTestObjects;

    [TestClass]
    public class UntypedObjectChangerTests
    {
        private SimpleField testFieldObject;
        private SimpleProperty testPropertyObject;
        private ComplexObject testComplexObject;

        private UntypedObjectChanger changer;

        [TestInitialize]
        public void TestInitialize()
        {
            this.testFieldObject = new SimpleField
            {
                boolField = true,
                floatField = 1.2f,
                integerField = 5,
                stringField = "test"
            };

            this.testPropertyObject = new SimpleProperty(true, "test", 1.2f)
            {
                StringProperty = "test",
                IntegerProperty = 5,
                FloatProperty = 1.2f,
                BoolProperty = true,
                ActualNameInJsonAttribute = 5
            };

            this.testComplexObject = new ComplexObject()
            {
                TestBoolean = true,
                testField = 1.2f,
                NestedObject = new NestedObject() { testfield = "test", TestProperty = 5 }
            };

            this.changer = new UntypedObjectChanger();
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_NoChangeInformation()
        {
            var objectToChange = this.testFieldObject.CreateJToken();

            changer.ChangeObject(objectToChange, null, null);

            var expected = this.testFieldObject.CreateJToken();

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_SingleValidField()
        {
            var objectToChange = this.testFieldObject.CreateJToken();

            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"boolField", JToken.FromObject(false)}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateJToken();
            expected["boolField"] = false;

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_ChangeMultipleFields()
        {
            var objectToChange = this.testFieldObject.CreateJToken();
            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"boolField", JToken.FromObject(false)},
                {"integerField", JToken.FromObject(123456)},
                {"stringField", JToken.FromObject("foobar")},
                {"floatField", JToken.FromObject(125.6f)}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateJToken();
            expected["boolField"] = false;
            expected["integerField"] = 123456;
            expected["stringField"] = "foobar";
            expected["floatField"] = 125.6f;

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_InvalidFieldsIgnored()
        {
            var objectToChange = this.testFieldObject.CreateJToken();
            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"invalidField1", JToken.FromObject(34234)},
                {"invalidField2", JToken.FromObject("this is a test. this is only a test")},
                {"stringField", JToken.FromObject("foobar")},
                {"floatField", JToken.FromObject(125.6f)}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateJToken();
            expected["stringField"] = "foobar";
            expected["floatField"] = 125.6f;

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_FieldNameIsCaseSensitive()
        {
            var objectToChange = this.testFieldObject.CreateJToken();

            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"BoolField", JToken.FromObject(false)}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateJToken();

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_FieldMatchedByJsonPropertyAttribute()
        {
            var objectToChange = this.testFieldObject.CreateJToken();
            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"NAMEINATTRIBUTE", "abcdefg"}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateJToken();
            expected["actualFieldNameIsInAttribute"] = "abcdefg";

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_ChangeSingleProperty()
        {
            var objectToChange = this.testPropertyObject.CreateJToken();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"StringProperty", JToken.FromObject("foobar")}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateJToken();
            expected["StringProperty"] = "foobar";

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_ChangeMultipleProperties()
        {
            var objectToChange = this.testPropertyObject.CreateJToken();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"StringProperty", JToken.FromObject("foobar")},
                {"BoolProperty", JToken.FromObject(false)},
                {"FloatProperty", JToken.FromObject(3.14f)},
                {"IntegerProperty",  JToken.FromObject(1000)}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateJToken();
            expected["StringProperty"] = "foobar";
            expected["BoolProperty"] = false;
            expected["FloatProperty"] = 3.14f;
            expected["IntegerProperty"] = 1000;

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_InvalidPropertiesIgnored()
        {
            var objectToChange = this.testPropertyObject.CreateJToken();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"StringProperty", JToken.FromObject("foobar")}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateJToken();
            expected["StringProperty"] = "foobar";

            AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_PropertiesCaseSensitive()
        {
            var objectToChange = this.testPropertyObject.CreateJToken();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"stringProperty", JToken.FromObject("foobar")}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateJToken();

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_PrivateSetMethod()
        {
            var objectToChange = this.testPropertyObject.CreateJToken();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"PrivateSetProperty", false}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateJToken();
            expected["PrivateSetProperty"] = false;

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_ChangePropertyAndField()
        {
            var objectToChange = this.testComplexObject.CreateJToken();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"testField", 3.14f},
                {"TestBoolean", false}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testComplexObject.CreateJToken();
            expected["testField"] = 3.14f;
            expected["TestBoolean"] = false;

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_ChangeNestedObject()
        {
            var objectToChange = this.testComplexObject.CreateJToken();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"NestedObject", JToken.FromObject(new NestedObject() { testfield = "foobar", TestProperty = 1000})}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testComplexObject.CreateJToken();
            expected["NestedObject"]["testfield"] = "foobar";
            expected["NestedObject"]["TestProperty"] = 1000;

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_ClearedFieldsRemoved()
        {
            var objectToChange = this.testComplexObject.CreateJToken();
            var fieldsToClear = new String[] { "testField", "TestBoolean", "NestedObject" };

            changer.ChangeObject(objectToChange, null, fieldsToClear);

            var expected = this.testComplexObject.CreateJToken();
            expected.Children<JProperty>().Single(x => x.Name == "NestedObject").Remove();
            expected.Children<JProperty>().Single(x => x.Name == "testField").Remove();
            expected.Children<JProperty>().Single(x => x.Name == "TestBoolean").Remove();

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_ClearedFieldsCaseSensitive()
        {
            var objectToChange = this.testComplexObject.CreateJToken();
            var fieldsToClear = new String[] { "TestField", "Testboolean", "Nestedobject" };

            changer.ChangeObject(objectToChange, null, fieldsToClear);

            var expected = this.testComplexObject.CreateJToken();

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        [TestMethod]
        public void UntypedObjectChanger_ChangeObject_InvalidName()
        {
            var objectToChange = this.testComplexObject.CreateJToken();
            var fieldsToClear = new String[] { "INVALID" };

            changer.ChangeObject(objectToChange, null, fieldsToClear);

            var expected = this.testComplexObject.CreateJToken();

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        private void AssertSimplePropertyEqual(JToken expectedValues, JToken actualValues)
        {
            Assert.AreNotEqual(expectedValues, actualValues);

            Assert.AreEqual(expectedValues["StringProperty"], actualValues["StringProperty"]);
            Assert.AreEqual(expectedValues["IntegerProperty"], actualValues["IntegerProperty"]);
            Assert.AreEqual(expectedValues["FloatProperty"], actualValues["FloatProperty"]);
            Assert.AreEqual(expectedValues["BoolProperty"], actualValues["BoolProperty"]);
            Assert.AreEqual(expectedValues["ActualNameInJsonAttribute"], actualValues["ActualNameInJsonAttribute"]);
            Assert.AreEqual(expectedValues["PrivateSetProperty"], actualValues["PrivateSetProperty"]);
            Assert.AreEqual(expectedValues["ReadOnlyProperty"], actualValues["ReadOnlyProperty"]);
            Assert.AreEqual(expectedValues["PropertyWithPropertyChanged"], actualValues["PropertyWithPropertyChanged"]);
        }

        private void AssertSimpleFieldEqual(JToken expectedValues, JToken actualValues)
        {
            Assert.AreNotEqual(expectedValues, actualValues);

            Assert.AreEqual(expectedValues["boolField"], actualValues["boolField"]);
            Assert.AreEqual(expectedValues["floatField"], actualValues["floatField"]);
            Assert.AreEqual(expectedValues["integerField"], actualValues["integerField"]);
            Assert.AreEqual(expectedValues["stringField"], actualValues["stringField"]);
        }

        private void AssertComplexObjectEqual(JToken expectedValues, JToken actualValues)
        {
            Assert.AreNotEqual(expectedValues, actualValues);

            Assert.AreEqual(expectedValues["TestBoolean"], actualValues["TestBoolean"]);
            Assert.AreEqual(expectedValues["testField"], actualValues["testField"]);

            if (expectedValues["NestedObject"] != null)
            {
                Assert.AreEqual(expectedValues["NestedObject"]["TestProperty"], actualValues["NestedObject"]["TestProperty"]);
                Assert.AreEqual(expectedValues["NestedObject"]["testfield"], expectedValues["NestedObject"]["testfield"]);
            }
            else
            {
                Assert.IsNull(actualValues["NestedObject"]);
            }
        }

    }
}
