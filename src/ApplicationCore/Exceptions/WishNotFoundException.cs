using System;

namespace Microsoft.eShopWeb.ApplicationCore.Exceptions
{

    public class WishNotFoundException : Exception
    {
        public WishNotFoundException(int wishId) : base($"No wish found with id {wishId}")
        {
        }

        protected WishNotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public WishNotFoundException(string message) : base(message)
        {
        }

        public WishNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
