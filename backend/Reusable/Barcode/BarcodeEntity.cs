using Reusable.CRUD.Contract;
namespace Reusable.Barcode
{
    public partial class Barcode : BaseEntity
    {
        public string Label { get; set; }
        public string BarcodeImage { get; set; }
        public string BarcodeValue { get; set; }
    }
}
