namespace DdpNet.Collections
{
    using System;
    using System.Linq;

    public class InvalidCollectionTypeException : Exception
    {
        public InvalidCollectionTypeException(string collectionName, Type existingType, Type targetType)
            : base(
                string.Format(
                    "Collection {0} was already initialized with type {1}. An attempt was made to retrieve a collection of the same name, but with type {2}. The type of a collection cannot change after it is initialized",
                    collectionName, existingType.GenericTypeArguments.First().Name, targetType.GenericTypeArguments.First().Name
                    )
                )
        {

        }
    }
}
