using Volo.Abp;

namespace Syrna.FileManagement.Files
{
    public class UploadQuantityExceededLimitException : BusinessException
    {
        public UploadQuantityExceededLimitException(long uploadQuantity, long maxQuantity) : base(
            "UploadQuantityExceededLimit",
            $"The quantity of the files ({uploadQuantity}) exceeded the limit: {maxQuantity}.")
        {
            Data.Add("maxQuantity", maxQuantity);
            Data.Add("uploadQuantity", uploadQuantity);
        }
    }
}