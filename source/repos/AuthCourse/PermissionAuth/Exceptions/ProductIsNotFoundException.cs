using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PermissionAuth.Exceptions
{
    public class ProductIsNotFoundException:Exception
    {
        public ProductIsNotFoundException():base()
        {

        }
        public ProductIsNotFoundException(string ErrorMsg) : base(ErrorMsg)
        {

        }

        public ProductIsNotFoundException(string ErrorMsg, Exception innerException) : base(ErrorMsg, innerException)
        {
        }
    }
}
