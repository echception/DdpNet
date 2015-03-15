namespace DdpNet.UnitTest
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Collections.TestObjects;
    using DdpNet.Collections;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Newtonsoft.Json.Linq;
    using ParameterObjects;

    [TestClass]
    public class DdpCollectionTests
    {
        [TestMethod]
        public void DdpCollection_Constructor_NullClientThrows()
        {
            PCLTesting.ExceptionAssert.Throws<ArgumentNullException>(
                () => { var collection = new DdpCollection<TestDdpObject>(null, "collectionName"); });
        }

        [TestMethod]
        public void DdpCollection_Constructor_NullCollectionNameThrows()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            PCLTesting.ExceptionAssert.Throws<ArgumentNullException>(() =>
            {
                var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, string.Empty); });
        }

        [TestMethod]
        public void DdpCollection_AddAsync_NullItem()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            PCLTesting.ExceptionAssert.Throws<ArgumentNullException>(() => collection.AddAsync(null));
        }

        [TestMethod]
        public void DdpCollection_AddAsync_CallsServerMethod()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject = new TestDdpObject() {Id = "1", integerField = 10, StringProperty = "FooBar"};

            remoteMethodCall.Setup(x => x.Call(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(Task.FromResult(true));

            collection.AddAsync(testObject).Wait();

            remoteMethodCall.Verify(
                x =>
                    x.Call(It.Is<string>(s => s.Equals(@"/TestCollection/insert")),
                        It.Is<object[]>(c => c.Length == 1 && c.First() == testObject)));
        }

        [TestMethod]
        public void DdpCollection_RemoveAsync_NullItem()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            PCLTesting.ExceptionAssert.Throws<ArgumentNullException>(() => collection.RemoveAsync(null));
        }

        [TestMethod]
        public void DdpCollection_RemoveAsync_CallsServerMethod()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject = new TestDdpObject() {Id = "1", integerField = 10, StringProperty = "FooBar"};

            remoteMethodCall.Setup(x => x.Call<int>(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(Task.FromResult(1));

            collection.RemoveAsync(testObject.Id).Wait();

            remoteMethodCall.Verify(
                x =>
                    x.Call<int>(It.Is<string>(s => s.Equals(@"/TestCollection/remove")),
                        It.Is<object[]>(c => c.Length == 1 && ((IdParameter) c.First()).ID == "1")));
        }

        [TestMethod]
        public void DdpCollection_UpdateAsync_NullItem()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            PCLTesting.ExceptionAssert.Throws<ArgumentNullException>(() => collection.UpdateAsync("ID", null));
        }

        [TestMethod]
        public void DdpCollection_UpdateAsync_NullID()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject = new TestDdpObject() { Id = "11", integerField = 10, StringProperty = "FooBar" };

            PCLTesting.ExceptionAssert.Throws<ArgumentNullException>(() => collection.UpdateAsync(string.Empty, testObject));
        }

        [TestMethod]
        public void DdpCollection_UpdateAsync_CallsServerMethod()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject = new TestDdpObject() { Id = "11", integerField = 10, StringProperty = "FooBar" };

            remoteMethodCall.Setup(x => x.Call<int>(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(Task.FromResult(1));

            collection.UpdateAsync(testObject.Id, testObject);

            remoteMethodCall.Verify(
                x =>
                    x.Call<int>(It.Is<string>(s => s.Equals(@"/TestCollection/update")),
                        It.Is<object[]>(
                            c =>
                                c.Length == 2 && ((IdParameter) c[0]).ID == "11" && ((Set) c[1]).ObjectToSet == testObject)));
        }

        [TestMethod]
        public void DdpCollection_Added_InvokedOnChangedEvents()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject = new TestDdpObject() { Id = "11", integerField = 101, StringProperty = "FooBar" };

            var collectionChangedCalled = false;
            var propertyChangedCalled = false;

            ((INotifyCollectionChanged)collection).CollectionChanged += (sender, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Add, args.Action);
                Assert.AreEqual(1, args.NewItems.Count);

                var addedObject = args.NewItems[0] as TestDdpObject;

                Assert.IsNotNull(addedObject);

                Assert.AreEqual(testObject.Id, addedObject.Id);
                Assert.AreEqual(testObject.integerField, addedObject.integerField);
                Assert.AreEqual(testObject.StringProperty, addedObject.StringProperty);

                collectionChangedCalled = true;
            };

            ((INotifyPropertyChanged)collection).PropertyChanged += (sender, args) =>
            {
                propertyChangedCalled = true;
            };

            ((IDdpCollection)collection).Added(testObject.Id, JObject.FromObject(testObject));

            Assert.IsTrue(collectionChangedCalled);
            Assert.IsTrue(propertyChangedCalled);
            Assert.AreEqual(1, collection.Count);
        }

        [TestMethod]
        public void DdpCollection_Changed_ChangesObject()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject = new TestDdpObject() { Id = "11", integerField = 101, StringProperty = "FooBar" };

            ((IDdpCollection)collection).Added(testObject.Id, JObject.FromObject(testObject));

            var objectChanged = collection.First();

            Assert.AreEqual(101, objectChanged.integerField);

            ((IDdpCollection) collection).Changed(testObject.Id,
                new Dictionary<string, JToken>() {{"integerField", 900}}, new string[0]);

            Assert.AreEqual(900, objectChanged.integerField);
        }

        [TestMethod]
        public void DdpCollection_Removed_InvokedOnChangedEvents()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject = new TestDdpObject() { Id = "11", integerField = 101, StringProperty = "FooBar" };

            var collectionChangedCalled = false;
            var propertyChangedCalled = false;

            ((IDdpCollection)collection).Added(testObject.Id, JObject.FromObject(testObject));

            Assert.AreEqual(1, collection.Count);

            ((INotifyCollectionChanged)collection).CollectionChanged += (sender, args) =>
            {
                Assert.AreEqual(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.AreEqual(1, args.OldItems.Count);

                var removedItem = args.OldItems[0] as TestDdpObject;

                Assert.IsNotNull(removedItem);

                Assert.AreEqual(testObject.Id, removedItem.Id);
                Assert.AreEqual(testObject.integerField, removedItem.integerField);
                Assert.AreEqual(testObject.StringProperty, removedItem.StringProperty);

                collectionChangedCalled = true;
            };

            ((INotifyPropertyChanged)collection).PropertyChanged += (sender, args) =>
            {
                propertyChangedCalled = true;
            };

            ((IDdpCollection)collection).Removed(testObject.Id);

            Assert.IsTrue(collectionChangedCalled);
            Assert.IsTrue(propertyChangedCalled);
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void DdpCollection_Enumerator_CanEnumerateItems()
        {

            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();

            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject = new TestDdpObject() { Id = "11", integerField = 101, StringProperty = "FooBar" };

            ((IDdpCollection)collection).Added(testObject.Id, JObject.FromObject(testObject));
            ((IDdpCollection)collection).Added(testObject.Id, JObject.FromObject(testObject));

            int loopTimes = 0;
            foreach (var itemObject in (IEnumerable)collection)
            {
                var item = (TestDdpObject) itemObject;
                Assert.AreEqual(testObject.Id, item.Id);
                Assert.AreEqual(testObject.integerField, item.integerField);
                Assert.AreEqual(testObject.StringProperty, item.StringProperty);
                loopTimes++;
            }

            Assert.AreEqual(2, loopTimes);
        }

        [TestMethod]
        public void DdpCollection_Filter_SortsNewFilter()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "3", integerField = 3};
            var testObject1 = new TestDdpObject() { Id="1", integerField = 1 };
            var testObject2 = new TestDdpObject() { Id="2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var sortedCollection = collection.Filter(sortFilter:(x, y) => x.integerField - y.integerField);

            Assert.AreEqual(3, collection[0].integerField);
            Assert.AreEqual(1, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);

            Assert.AreEqual(1, sortedCollection[0].integerField);
            Assert.AreEqual(2, sortedCollection[1].integerField);
            Assert.AreEqual(3, sortedCollection[2].integerField);
        }

        [TestMethod]
        public void DdpCollection_Filter_InsertsSorted()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var sortedCollection = collection.Filter(sortFilter: (x, y) => x.integerField - y.integerField);

            var testObject3 = new TestDdpObject() { Id = "3", integerField = 3 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 1 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));

            Assert.AreEqual(3, collection[0].integerField);
            Assert.AreEqual(3, sortedCollection[0].integerField);

            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));

            Assert.AreEqual(3, collection[0].integerField);
            Assert.AreEqual(1, collection[1].integerField);

            Assert.AreEqual(1, sortedCollection[0].integerField);
            Assert.AreEqual(3, sortedCollection[1].integerField);

            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            Assert.AreEqual(3, collection[0].integerField);
            Assert.AreEqual(1, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);


            Assert.AreEqual(1, sortedCollection[0].integerField);
            Assert.AreEqual(2, sortedCollection[1].integerField);
            Assert.AreEqual(3, sortedCollection[2].integerField);
        }

        [TestMethod]
        public void DdpCollection_Filter_ChangesSortedMoveToEnd()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "3", integerField = 3 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 1 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var sortedCollection = collection.Filter(sortFilter:(x, y) => x.integerField - y.integerField);

            Assert.AreEqual(3, collection[0].integerField);
            Assert.AreEqual(1, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);

            Assert.AreEqual(1, sortedCollection[0].integerField);
            Assert.AreEqual(2, sortedCollection[1].integerField);
            Assert.AreEqual(3, sortedCollection[2].integerField);

            ((IDdpCollection)collection).Changed(testObject2.Id, new Dictionary<string, JToken>() {{"integerField", 4}}, new string[0]);

            Assert.AreEqual(3, collection[0].integerField);
            Assert.AreEqual(1, collection[1].integerField);
            Assert.AreEqual(4, collection[2].integerField);

            Assert.AreEqual(1, sortedCollection[0].integerField);
            Assert.AreEqual(3, sortedCollection[1].integerField);
            Assert.AreEqual(4, sortedCollection[2].integerField);
        }

        [TestMethod]
        public void DdpCollection_Filter_ChangesSortedMoveToBeginning()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "3", integerField = 3 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 1 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var sortedCollection = collection.Filter(sortFilter: (x, y) => x.integerField - y.integerField);

            Assert.AreEqual(3, collection[0].integerField);
            Assert.AreEqual(1, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);

            Assert.AreEqual(1, sortedCollection[0].integerField);
            Assert.AreEqual(2, sortedCollection[1].integerField);
            Assert.AreEqual(3, sortedCollection[2].integerField);

            ((IDdpCollection)collection).Changed(testObject3.Id, new Dictionary<string, JToken>() { { "integerField", 0 } }, new string[0]);

            Assert.AreEqual(0, collection[0].integerField);
            Assert.AreEqual(1, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);

            Assert.AreEqual(0, sortedCollection[0].integerField);
            Assert.AreEqual(1, sortedCollection[1].integerField);
            Assert.AreEqual(2, sortedCollection[2].integerField);
        }

        [TestMethod]
        public void DdpCollection_Filter_ChangesSortedMoveToMiddleFromFront()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "4", integerField = 4 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 1 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var sortedCollection = collection.Filter(sortFilter: (x, y) => x.integerField - y.integerField);

            Assert.AreEqual(4, collection[0].integerField);
            Assert.AreEqual(1, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);

            Assert.AreEqual(1, sortedCollection[0].integerField);
            Assert.AreEqual(2, sortedCollection[1].integerField);
            Assert.AreEqual(4, sortedCollection[2].integerField);

            ((IDdpCollection)collection).Changed(testObject1.Id, new Dictionary<string, JToken>() { { "integerField", 3 } }, new string[0]);

            Assert.AreEqual(4, collection[0].integerField);
            Assert.AreEqual(3, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);

            Assert.AreEqual(2, sortedCollection[0].integerField);
            Assert.AreEqual(3, sortedCollection[1].integerField);
            Assert.AreEqual(4, sortedCollection[2].integerField);
        }

        [TestMethod]
        public void DdpCollection_Filter_ChangesSortedMoveToMiddleFromEnd()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "4", integerField = 4 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 0 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var sortedCollection = collection.Filter(sortFilter: (x, y) => x.integerField - y.integerField);

            Assert.AreEqual(4, collection[0].integerField);
            Assert.AreEqual(0, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);

            Assert.AreEqual(0, sortedCollection[0].integerField);
            Assert.AreEqual(2, sortedCollection[1].integerField);
            Assert.AreEqual(4, sortedCollection[2].integerField);

            ((IDdpCollection)collection).Changed(testObject3.Id, new Dictionary<string, JToken>() { { "integerField", 1 } }, new string[0]);

            Assert.AreEqual(1, collection[0].integerField);
            Assert.AreEqual(0, collection[1].integerField);
            Assert.AreEqual(2, collection[2].integerField);

            Assert.AreEqual(0, sortedCollection[0].integerField);
            Assert.AreEqual(1, sortedCollection[1].integerField);
            Assert.AreEqual(2, sortedCollection[2].integerField);
        }

        [TestMethod]
        public void DdpCollection_Filter_WhereIncluded()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "4", integerField = 4 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 0 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var filteredCollection = collection.Filter(whereFilter: x => x.integerField == 4);

            Assert.AreEqual(1, filteredCollection.Count);
            Assert.AreEqual(4, filteredCollection.Single().integerField);
        }

        [TestMethod]
        public void DdpCollection_Filter_WhereMatchNone()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "4", integerField = 4 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 0 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var filteredCollection = collection.Filter(whereFilter: x => x.integerField == 5);

            Assert.AreEqual(0, filteredCollection.Count);
        }

        [TestMethod]
        public void DdpCollection_Filter_WhereMatchAll()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "4", integerField = 4 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 0 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var filteredCollection = collection.Filter(whereFilter: x => true);

            Assert.AreEqual(3, filteredCollection.Count);
        }

        [TestMethod]
        public void DdpCollection_Filter_WhereObjectAddedAfterChange()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "4", integerField = 4 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 0 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var filteredCollection = collection.Filter(whereFilter: x => x.integerField == 4);

            Assert.AreEqual(1, filteredCollection.Count);

            ((IDdpCollection)collection).Changed(testObject1.Id, new Dictionary<string, JToken>() { { "integerField", 4 } }, new string[0]);

            Assert.AreEqual(2, filteredCollection.Count);
        }

        [TestMethod]
        public void DdpCollection_Filter_WhereObjectAddedRemovedChange()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "4", integerField = 4 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 0 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 2 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var filteredCollection = collection.Filter(whereFilter: x => x.integerField == 4);

            Assert.AreEqual(1, filteredCollection.Count);

            ((IDdpCollection)collection).Changed(testObject3.Id, new Dictionary<string, JToken>() { { "integerField", 1 } }, new string[0]);

            Assert.AreEqual(0, filteredCollection.Count);
        }

        [TestMethod]
        public void DdpCollection_Filter_SortedAndWhere()
        {
            var remoteMethodCall = new Mock<IDdpRemoteMethodCall>();
            var collection = new DdpCollection<TestDdpObject>(remoteMethodCall.Object, "TestCollection");

            var testObject3 = new TestDdpObject() { Id = "4", integerField = 4 };
            var testObject1 = new TestDdpObject() { Id = "1", integerField = 2 };
            var testObject2 = new TestDdpObject() { Id = "2", integerField = 1 };

            ((IDdpCollection)collection).Added(testObject3.Id, JObject.FromObject(testObject3));
            ((IDdpCollection)collection).Added(testObject1.Id, JObject.FromObject(testObject1));
            ((IDdpCollection)collection).Added(testObject2.Id, JObject.FromObject(testObject2));

            var filteredCollection = collection.Filter(whereFilter: x => x.integerField < 4, sortFilter: (x,y) => x.integerField - y.integerField);

            Assert.AreEqual(2, filteredCollection.Count);

            Assert.AreEqual(1, filteredCollection[0].integerField);
            Assert.AreEqual(2, filteredCollection[1].integerField);
        }
    }
}
