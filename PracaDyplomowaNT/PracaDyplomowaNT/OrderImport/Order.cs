using Soneta.Towary;
using Soneta.Types;

namespace PracaDyplomowaNT.OrderImport
{
    public class Order
    {
        public string ItemId { get; set; }
        public Quantity Quantity { get; set; }
        public DoubleCy Price { get; set; }
        public bool Cod { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TargetPoint { get; set; }
    }
}