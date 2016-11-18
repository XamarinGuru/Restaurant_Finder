
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using ToolBar = Android.Support.V7.Widget.Toolbar;

namespace Restaurant_Finder
{
	[Activity(Label = "SearchResultsActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class SearchResultsActivity : AppCompatActivity
	{
		private ListView searchResultsListView;
		private List<RestaurantModel> searchResultsList;
		private ImageView loadingImageView;
		private LinearLayout linearLayoutBottom;
		private TextView toolBarTitle;
		private bool isNear;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.SearchResults);

			searchResultsListView = FindViewById<ListView>(Resource.Id.listViewResults);
			loadingImageView = FindViewById<ImageView>(Resource.Id.loadingImageView);
			linearLayoutBottom = FindViewById<LinearLayout>(Resource.Id.bottomBarLinearLayout);
			toolBarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);

			searchResultsListView.Visibility = ViewStates.Invisible;
			linearLayoutBottom.Visibility = ViewStates.Invisible;
			loadingImageView.Visibility = ViewStates.Visible;

			string baseUrl = Intent.GetStringExtra("baseUrl");
			string clientId = Intent.GetStringExtra("clientID");
			string clientSecret = Intent.GetStringExtra("clientSecret");
			string restaurantName = Intent.GetStringExtra("restaurantType");
			string cityName = Intent.GetStringExtra("city");
			bool sortByDistance = Intent.GetBooleanExtra("sortingType", true);
			double radius = Intent.GetIntExtra("radius", 20);
			int numberOfResults = Intent.GetIntExtra("numberOfResults", 20);
			double lat = Intent.GetDoubleExtra("latitude", 0.0);
			double lng = Intent.GetDoubleExtra("longitude", 0.0);
			isNear = Intent.GetBooleanExtra("isNear", true);

			ToolBar toolBar = FindViewById<ToolBar>(Resource.Id.toolbarSearchResults);
			SetSupportActionBar(toolBar);
			SupportActionBar.SetDisplayShowTitleEnabled(false);
			toolBarTitle.Text = restaurantName;


			SearchForRestaurant(baseUrl, clientId, clientSecret, restaurantName, cityName, sortByDistance, radius, numberOfResults, lat, lng, isNear);

			searchResultsList = new List<RestaurantModel>();
		}


		async void  SearchForRestaurant(string _baseUrl, string _clientId, string _clientSecret, string _restaurantName, string _cityName, bool _sortByDistance, double _radius, int _numberOfResults, double _lat, double _long, bool _isNear)
		{
			string url = "";
			double meterRadius = MainActivity.preferences.GetInt("radius", 20);
			int _sortByDistanceValue;

			if (MainActivity.preferences.GetBoolean("isKilometers", true) == true)
			{
				meterRadius = meterRadius * 1000;
			}
			else
			{
				meterRadius = meterRadius * 1609.34;
			}

			if (_restaurantName == "All")
			{
				_restaurantName = "Restaurant";
			}

			if (_sortByDistance == true)
			{
				_sortByDistanceValue = 1;
			}
			else
			{
				_sortByDistanceValue = 0;
			}

			if (_isNear == true)
			{
				url = _baseUrl +
					"/v2/venues/explore?client_id=" +
					_clientId +
					"&client_secret=" +
					_clientSecret +
					"&query=" +
					_restaurantName +
					"&v=20201212&sortByDistance=" +
					_sortByDistanceValue +
					"&venuePhotos=1&radius=" +
					meterRadius +
					"&limit=" +
					_numberOfResults +
					"&ll=" +
					_lat +
					"," +
					_long;
			}
			else
			{
				
				url = _baseUrl +
					"/v2/venues/explore?client_id=" +
					_clientId +
					"&client_secret=" +
					_clientSecret +
					"&query=" +
					_restaurantName +
					"&v=20201212&m=swarm&sortByDistance=" +
					_sortByDistanceValue +
					"&venuePhotos=1&radius=" +
					meterRadius +
					"&limit=" +
					_numberOfResults +
					"&near=" +
					_cityName;

			}

			Log.Debug("OUTPUT", url); 

			JsonValue json = await FetchRestaurantsAsync(url);

			InitializeRestaurantData(json, _restaurantName);

			loadingImageView.Visibility = ViewStates.Invisible;
			linearLayoutBottom.Visibility = ViewStates.Visible;
			searchResultsListView.Visibility = ViewStates.Visible;
		}


		//async Task<JsonValue> FetchRestaurantsAsync(string url)
		//{
		//	HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new System.Uri(url));
		//	request.ContentType = "application/json";
		//	request.Method = "GET";

		//	using (WebResponse response = await request.GetResponseAsync())
		//	{
		//		// Get a stream representation of the HTTP web response:
		//		using (System.IO.Stream stream = response.GetResponseStream())
		//		{
		//			// Use this stream to build a JSON document object:
		//			JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
		//			Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

		//			stream.Close();

		//			// Return the JSON document:
		//			return jsonDoc;
		//		}
		//	}
		//}
		private async Task<JsonValue> FetchRestaurantsAsync(string url)
		{
			JsonValue jsonValue = null;

			using (var httpClient = new HttpClient())
			{
				var jsonString = await httpClient.GetStringAsync(url);
				if (jsonString != null && jsonString.Length > 0)
				{
					jsonValue = JsonObject.Parse(jsonString);
				}
			}

			return jsonValue;
		}

		void InitializeRestaurantData(JsonValue responseJson, string resName)
		{
			try
			{
				JsonValue response = responseJson["response"]["groups"][0]["items"];

				int counter = 0;

				for (int i = 0; i < response.Count; i++)
				{
					RestaurantModel restaurantObject = new RestaurantModel();

					if (response[i]["venue"].ContainsKey("rating"))
					{
						restaurantObject.rating = response[i]["venue"]["rating"];
						restaurantObject.ratingColor = response[i]["venue"]["ratingColor"];
					}
					else
					{
						restaurantObject.ratingColor = null;
					}

					restaurantObject.restaurantName = response[i]["venue"]["name"];

					if (response[i]["venue"]["location"].ContainsKey("address"))
					{
						restaurantObject.streetAddress = response[i]["venue"]["location"]["address"];
					}
					else
					{
						restaurantObject.streetAddress = null;
					}

					if (isNear == true)
					{
						int lastIndex = (response[i]["venue"]["location"]["formattedAddress"]).Count - 1;
						restaurantObject.countryAdress = response[i]["venue"]["location"]["formattedAddress"][lastIndex];
					}

					if (isNear == true)
					{
						restaurantObject.distance = response[i]["venue"]["location"]["distance"];
					}

					if (response[i]["venue"].ContainsKey("url"))
					{
						restaurantObject.url = response[i]["venue"]["url"];
					}
					else
					{
						restaurantObject.url = null;
					}

					if (response[i]["venue"]["location"].ContainsKey("city"))
					{
						restaurantObject.cityAdress = response[i]["venue"]["location"]["city"];
					}
					else
					{
						restaurantObject.cityAdress = null;
					}

					if (response[i]["venue"]["location"].ContainsKey("state"))
					{
						restaurantObject.stateAdress = response[i]["venue"]["location"]["state"];
					}
					else
					{
						restaurantObject.stateAdress = null;
					}

					if (response[i]["venue"]["location"].ContainsKey("country"))
					{
						restaurantObject.ccAdress = response[i]["venue"]["location"]["country"];
					}
					else
					{
						restaurantObject.ccAdress = null;
					}

					if ((response[i]["venue"]["photos"]["count"]).ToString() == "0")
					{
						restaurantObject.imageString = null;
					}
					else
					{
						JsonValue image = response[i]["venue"]["photos"]["groups"][0]["items"][0];
						restaurantObject.imageString = image["prefix"] + "100x100" + image["suffix"];
					}

					if (isNear == true)
					{
						if (response[i]["venue"]["contact"].ContainsKey("formattedPhone"))
						{
							restaurantObject.phoneNumber = response[i]["venue"]["contact"]["formattedPhone"];
						}
						else
						{
							restaurantObject.phoneNumber = null;
						}
						if (response[i]["venue"]["contact"].ContainsKey("phone"))
						{
							restaurantObject.nonFormattedPhoneNumber = response[i]["venue"]["contact"]["phone"];
						}
						else
						{
							restaurantObject.nonFormattedPhoneNumber = null;
						}
					}



					restaurantObject.lat = response[i]["venue"]["location"]["lat"];
					restaurantObject.lng = response[i]["venue"]["location"]["lng"];

					searchResultsList.Add(restaurantObject);

					restaurantObject = null;

					Console.WriteLine("The value of the counter is: {0}", counter);

					counter++;
				}

				if (response.Count == 0 || response[0] == null)
				{
					Android.Support.V7.App.AlertDialog.Builder errorNotification = new Android.Support.V7.App.AlertDialog.Builder(this);
					errorNotification.SetTitle("No Search Results");
					errorNotification.SetMessage("There are no " + resName + " within the specified radius");
					errorNotification.SetPositiveButton("OK", (sender, e) =>
															{
																//Alert Dialog closes.
															});

					var alert = errorNotification.Create();
					alert.Show();
				}
				else
				{
					var adapter = new SearchResultsListViewAdapter(this, searchResultsList, isNear);
					searchResultsListView.Adapter = adapter;
				}
			}
			catch(Exception e)
			{
			}

		}

	}

}
