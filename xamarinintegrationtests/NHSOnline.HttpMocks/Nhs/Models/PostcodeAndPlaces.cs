using System.Collections.Generic;

namespace NHSOnline.HttpMocks.Nhs.Models
{
    public class PostCodeAndPlacesItem
    {
        public string? Latitude { get; set; }

        public string? Longitude { get; set; }
    }

    public class PostCodeAndPlaces
    {
        public List<PostCodeAndPlacesItem>? value { get; set; }

        public int count { get; set; }
    }
}