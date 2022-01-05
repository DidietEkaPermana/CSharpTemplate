using Service.Domain.Base;
using System.Collections.Generic;

namespace Service.Domain.Entities
{
    public class Property : BaseModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public PropertyType PropertyType { get; set; }
        /// address of hotels
        public string Address { get; set; }
        /// Location should consist of "lat, long" format 
        public string Location { get; set; }
        /// current hotel rate star, example 3 star hotel, 4 star hotel etc
        public int Star { get; set; }
        /// rating from user, this got from ratingreview service
        public string Rating { get; set; }
        /// number of reviewer, got it from ratingreview service
        public int NumReviews { get; set; }
        /// string format of ex "wifi, breakfast, rental" etc
        public string Facilities { get; set; }
        public string Telephone { get; set; }
        public string Facsimile { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public bool Display { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string CityLocation { get; set; }
        public string CityLocationCode { get; set; }

        // [Column("Image", TypeName = "string[]")]
        public string[] Image { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
