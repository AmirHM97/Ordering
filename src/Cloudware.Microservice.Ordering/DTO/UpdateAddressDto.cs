namespace Cloudware.Microservice.Ordering.DTO
{
    public class UpdateAddressDto
    {
        public long OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long CityId { get; set; }
        public string City { get; set; }
        public long ProvinceId { get; set; }
        public string Province { get; set; }
        public string LandlinePhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}