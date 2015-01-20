namespace DdpNet.UnitTest.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Remoting;
    using DdpNet.Collections;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json.Bson;
    using Newtonsoft.Json.Linq;
    using ObjectChangerTestObjects;

    [TestClass]
    public class ObjectChangerTests
    {
        private SimpleField testFieldObject;
        private SimpleProperty testPropertyObject;
        private ComplexObject testComplexObject;

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
                NestedObject = new NestedObject() {testfield = "test", TestProperty = 5}
            };
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_NoChangeInformation()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testFieldObject.CreateCopy();

            changer.ChangeObject(objectToChange, null, null);

            var expected = this.testFieldObject.CreateCopy();

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_SingleValidField()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testFieldObject.CreateCopy();

            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"boolField", JToken.FromObject(false)}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateCopy();
            expected.boolField = false;
            
            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_ChangeMultipleFields()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testFieldObject.CreateCopy();
            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"boolField", JToken.FromObject(false)},
                {"integerField", JToken.FromObject(123456)},
                {"stringField", JToken.FromObject("foobar")},
                {"floatField", JToken.FromObject(125.6f)}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateCopy();
            expected.boolField = false;
            expected.integerField = 123456;
            expected.stringField = "foobar";
            expected.floatField = 125.6f;

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_InvalidFieldsIgnored()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testFieldObject.CreateCopy();
            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"invalidField1", JToken.FromObject(34234)},
                {"invalidField2", JToken.FromObject("this is a test. this is only a test")},
                {"stringField", JToken.FromObject("foobar")},
                {"floatField", JToken.FromObject(125.6f)}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateCopy();
            expected.stringField = "foobar";
            expected.floatField = 125.6f;

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_FieldNameIsCaseSensitive()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testFieldObject.CreateCopy();

            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"BoolField", JToken.FromObject(false)}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateCopy();
            
            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_FieldMatchedByJsonPropertyAttribute()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testFieldObject.CreateCopy();
            var fieldsToChange = new Dictionary<string, JToken>
            {
                {"NAMEINATTRIBUTE", "abcdefg"}
            };

            changer.ChangeObject(objectToChange, fieldsToChange, null);

            var expected = this.testFieldObject.CreateCopy();
            expected.actualFieldNameIsInAttribute = "abcdefg";

            this.AssertSimpleFieldEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_ChangeSingleProperty()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testPropertyObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"StringProperty", JToken.FromObject("foobar")}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateCopy();
            expected.StringProperty = "foobar";

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_ChangeMultipleProperties()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testPropertyObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"StringProperty", JToken.FromObject("foobar")},
                {"BoolProperty", JToken.FromObject(false)},
                {"FloatProperty", JToken.FromObject(3.14f)},
                {"IntegerProperty",  JToken.FromObject(1000)}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateCopy();
            expected.StringProperty = "foobar";
            expected.BoolProperty = false;
            expected.FloatProperty = 3.14f;
            expected.IntegerProperty = 1000;

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_InvalidPropertiesIgnored()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testPropertyObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"StringProperty", JToken.FromObject("foobar")}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateCopy();
            expected.StringProperty = "foobar";

            AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_PropertiesCaseSensitive()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testPropertyObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"stringProperty", JToken.FromObject("foobar")}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateCopy();

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_PropertiesMatchedByJsonPropertyAttribute()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testPropertyObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"foobar", JToken.FromObject(1337)}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateCopy();
            expected.ActualNameInJsonAttribute = 1337;

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_ReadOnlyProperty()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testPropertyObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"ReadOnlyProperty", "ReadOnlyValue"}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateCopy();

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_PrivateSetMethod()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testPropertyObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"PrivateSetProperty", false}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateCopy(privateSetProperty:false);

            this.AssertSimplePropertyEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_InvokesPropertyChanged()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testPropertyObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"PropertyWithPropertyChanged", 3.14f}
            };

            bool propertyChangedInvoked = false;
            objectToChange.PropertyChanged += (sender, args) =>
            {
                Assert.AreEqual("PropertyWithPropertyChanged", args.PropertyName);
                propertyChangedInvoked = true;
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testPropertyObject.CreateCopy(propertyWithPropertyChanged:3.14f);

            this.AssertSimplePropertyEqual(expected, objectToChange);
            Assert.IsTrue(propertyChangedInvoked);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_ChangePropertyAndField()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testComplexObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"testField", 3.14f},
                {"TestBoolean", false}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testComplexObject.CreateCopy();
            expected.testField = 3.14f;
            expected.TestBoolean = false;

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_ChangeNestedObject()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testComplexObject.CreateCopy();
            var propertiesToChange = new Dictionary<string, JToken>
            {
                {"NestedObject", JToken.FromObject(new NestedObject() { testfield = "foobar", TestProperty = 1000})}
            };

            changer.ChangeObject(objectToChange, propertiesToChange, null);

            var expected = this.testComplexObject.CreateCopy();
            expected.NestedObject.testfield = "foobar";
            expected.NestedObject.TestProperty = 1000;

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_ClearedFieldsReturnedToDefault()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testComplexObject.CreateCopy();
            var fieldsToClear = new String[] {"testField", "TestBoolean", "NestedObject"};

            changer.ChangeObject(objectToChange, null, fieldsToClear);

            var expected = this.testComplexObject.CreateCopy();
            expected.NestedObject = null;
            expected.testField = default(float);
            expected.TestBoolean = default(bool);

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_ClearedFieldsCaseSensitive()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testComplexObject.CreateCopy();
            var fieldsToClear = new String[] { "TestField", "Testboolean", "Nestedobject" };

            changer.ChangeObject(objectToChange, null, fieldsToClear);

            var expected = this.testComplexObject.CreateCopy();

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        [TestMethod]
        public void ObjectChanger_ChangeObject_InvalidName()
        {
            var changer = new ObjectChanger();
            var objectToChange = this.testComplexObject.CreateCopy();
            var fieldsToClear = new String[] { "INVALID"};

            changer.ChangeObject(objectToChange, null, fieldsToClear);

            var expected = this.testComplexObject.CreateCopy();

            this.AssertComplexObjectEqual(expected, objectToChange);
        }

        private void AssertSimplePropertyEqual(SimpleProperty expectedValues, SimpleProperty actualValues)
        {
            Assert.AreNotEqual(expectedValues, actualValues);

            Assert.AreEqual(expectedValues.StringProperty, actualValues.StringProperty);
            Assert.AreEqual(expectedValues.IntegerProperty, actualValues.IntegerProperty);
            Assert.AreEqual(expectedValues.FloatProperty, actualValues.FloatProperty);
            Assert.AreEqual(expectedValues.BoolProperty, actualValues.BoolProperty);
            Assert.AreEqual(expectedValues.ActualNameInJsonAttribute, actualValues.ActualNameInJsonAttribute);
            Assert.AreEqual(expectedValues.PrivateSetProperty, actualValues.PrivateSetProperty);
            Assert.AreEqual(expectedValues.ReadOnlyProperty, actualValues.ReadOnlyProperty);
            Assert.AreEqual(expectedValues.PropertyWithPropertyChanged, actualValues.PropertyWithPropertyChanged);
        }

        private void AssertSimpleFieldEqual(SimpleField expectedValues, SimpleField actualValues)
        {
            Assert.AreNotEqual(expectedValues, actualValues);

            Assert.AreEqual(expectedValues.boolField, actualValues.boolField);
            Assert.AreEqual(expectedValues.floatField, actualValues.floatField);
            Assert.AreEqual(expectedValues.integerField, actualValues.integerField);
            Assert.AreEqual(expectedValues.stringField, actualValues.stringField);
        }

        private void AssertComplexObjectEqual(ComplexObject expectedValues, ComplexObject actualValues)
        {
            Assert.AreNotEqual(expectedValues, actualValues);

            Assert.AreEqual(expectedValues.TestBoolean, actualValues.TestBoolean);
            Assert.AreEqual(expectedValues.testField, actualValues.testField);

            if (expectedValues.NestedObject != null)
            {
                Assert.AreEqual(expectedValues.NestedObject.TestProperty, actualValues.NestedObject.TestProperty);
                Assert.AreEqual(expectedValues.NestedObject.testfield, expectedValues.NestedObject.testfield);
            }
            else
            {
                Assert.IsNull(actualValues.NestedObject);
            }
        }
    }
}
