using System;
namespace Restaurant_Finder
{
	public class RestaurantModel
	{
		public double rating { get; set; }
		public string ratingColor { get; set; }
		public string restaurantName { get; set; }
		public string streetAddress { get; set; }
		public string countryAdress { get; set; }
		public double distance { get; set; }
		public string phoneNumber { get; set; }
		public string nonFormattedPhoneNumber { get; set; }
		public string url { get; set; }
		public string cityAdress { get; set; }
		public string stateAdress { get; set; }
		public string ccAdress { get; set; }
		public string imageString { get; set; }
		public string featuredImageString { get; set;}
		public double lat { get; set; }
		public double lng { get; set; }
		public Array tempArray { get; set; }

		public RestaurantModel()
		{
		}
	}
}
