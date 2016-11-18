using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using Android.Content.PM;
using Android.Util;
using Android.Locations;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Net;
using Java.Util;
using Android.Preferences;
using System.Json;
using System.Net;
using Android.Media;
using Android.Runtime;
using System.IO;
using Android.Graphics;
using Android.Support.V7.App;
using ToolBar = Android.Support.V7.Widget.Toolbar;
using Android.Views.InputMethods;
using System.Net.Http;

namespace Restaurant_Finder
{
	[Activity(Label = "Restaurant_Finder", Icon = "@mipmap/icon", ScreenOrientation = ScreenOrientation.Portrait, Theme="@style/MyTheme")]
	public class MainActivity : AppCompatActivity, ILocationListener
	{
		private Button advancedSearchButton;
		private Button hideButton;
		private EditText cityEditText;
		public static EditText restaurantTypeEditText;
		private Button invisibleButton;
		private Button restaurantSearchButton;
		private TextView noInternetConnectionTextView;
		private ImageView weatherIcon;
		public static TextView temperatureTextView;
		private TextView conditionTextView;
		private TextView humidityTextView;
		private TextView dateTextView;
		private ImageView loadingImageView;
		private LinearLayout weatherInfoLayout;
		private TextView toolBarTitle;

		private bool isNear;
		static readonly string TAG = "X:" + typeof(MainActivity).Name;
		public static Location _currentLocation;
		private LocationManager _locationManager;
		private string _locationProvider;
		private ConnectivityManager connectivityManager;
		public bool isOnline;
		private string openWeatherAPIKey = "af45f01782dbefa47363f11617de7d23";
		private string imageUrlString = "http://openweathermap.org/img/w/";
		private string baseUrl = "https://api.foursquare.com";
		private string clientId = "OHOIR43Q1EVS3OLORA1JDYDIW5LZ4JSJYLXQ2BUAWMSPVVMU";
		private string clientSecret = "CYFHYPGKNC4UBVNF4A1IEI10THYZ5MM30JEGSBZXJP1SEF5L";

		public static ISharedPreferences preferences;
		public static ISharedPreferencesEditor editor;
		public static bool isCelcius;
		public static int radius;
		public static bool isSortedByDistance;
		public static bool isKilometers;
		public static int numberOfResults;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			//InitializeLocationManager();

			connectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			isOnline = (activeConnection != null) && activeConnection.IsConnected;

			Log.Debug("DEBUG", "The value of isOnline is: {0}", isOnline);

			NetConnectionChangeReceiver.activity = this;

			/*NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
			if (wifiInfo.IsConnected)
			{
				Log.Debug("DEBUG", "Wifi connected.");

			}
			else
			{
				Log.Debug("DEBUG", "Wifi disconnected.");

			}

			NetworkInfo mobileInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Mobile);
			if (mobileInfo.IsRoaming && mobileInfo.IsConnected)
			{
				Log.Debug("DEBUG", "Roaming.");

			}
			else
			{
				Log.Debug("DEBUG", "Not roaming.");

			}*/

			CheckInternetConnection();

			preferences = PreferenceManager.GetDefaultSharedPreferences(this);
			editor = preferences.Edit();

			CheckIfFirstTime();

			restaurantSearchButton = FindViewById<Button>(Resource.Id.button1);
			advancedSearchButton = FindViewById<Button>(Resource.Id.button2);
			hideButton = FindViewById<Button>(Resource.Id.buttonHide);
			cityEditText = FindViewById<EditText>(Resource.Id.editText2);
			restaurantTypeEditText = FindViewById<EditText>(Resource.Id.editText1);
			invisibleButton = FindViewById<Button>(Resource.Id.invisibleButton);
			noInternetConnectionTextView = FindViewById<TextView>(Resource.Id.noInternetConnectionTextView);
			weatherIcon = FindViewById<ImageView>(Resource.Id.weatherImageView);
			temperatureTextView = FindViewById<TextView>(Resource.Id.temperatureTextView);
			conditionTextView = FindViewById<TextView>(Resource.Id.conditionTextView);
			humidityTextView = FindViewById<TextView>(Resource.Id.humidityTextView);
			dateTextView = FindViewById<TextView>(Resource.Id.dateTextView);
			loadingImageView = FindViewById<ImageView>(Resource.Id.ImageViewLoading);
			weatherInfoLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutWeather);
			toolBarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);

			ToolBar toolBar = FindViewById<ToolBar>(Resource.Id.toolbar);
			SetSupportActionBar(toolBar);
			SupportActionBar.SetDisplayShowTitleEnabled(false);
			toolBarTitle.Text = "Restaurant Finder";

			loadingImageView.Visibility = ViewStates.Visible;
			weatherInfoLayout.Visibility = ViewStates.Gone;

			noInternetConnectionTextView.Visibility = ViewStates.Gone;

			cityEditText.Visibility = ViewStates.Invisible;
			isNear = true;

			hideButton.Visibility = ViewStates.Gone;

			restaurantTypeEditText.SetCursorVisible(false);

			restaurantSearchButton.Click += RestaurantSearchButtonClicked;

			advancedSearchButton.Click += AdvancedButtonClicked;

			hideButton.Click += HideButtonClicked;

			invisibleButton.Click += InvisibleButtonClicked;

			cityEditText.KeyPress += (object sender, View.KeyEventArgs e) =>
			{
				if ((e.Event.Action == KeyEventActions.Down) && (e.KeyCode == Keycode.Enter))
				{
					InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
					imm.HideSoftInputFromWindow(cityEditText.WindowToken, 0);
					Console.WriteLine("Enter key pressed");
				}
			};

			//await InitializeWeatherData();
		}


		protected async override void OnResume()
		{
			base.OnResume();

			InitializeLocationManager();

			try
			{
				if (_locationManager.IsProviderEnabled(_locationProvider) == true)
				{
					_locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
				}
				else
				{

					Android.Support.V7.App.AlertDialog.Builder firstTimeNotfication = new Android.Support.V7.App.AlertDialog.Builder(this);
					firstTimeNotfication.SetTitle("Location services not enabled");
					firstTimeNotfication.SetMessage("Your device's location service is not enabled. In order to use this application you must turn on location services in Settings");
					firstTimeNotfication.SetPositiveButton("OK", (sender, e) =>
																{
																	//Alert Dialog closes.
																});

					var alert = firstTimeNotfication.Create();
					alert.Show();
				}
			}
			catch
			{

			}

			await InitializeWeatherData();
		}


		protected override void OnPause()
		{
			base.OnPause();
			_locationManager.RemoveUpdates(this);
		}


		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu, menu);

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var intent = new Intent(this, typeof(SettingsActivity));
			StartActivity(intent);
			return base.OnOptionsItemSelected(item);
		}

		void CheckIfFirstTime()
		{
			if (preferences.GetBoolean("first_time", true))
			{
				Log.Debug("DEBUG", "This is the first time the app is being launched");
				editor.PutBoolean("first_time", false);
				editor.Apply();

				isCelcius = false;
				radius = 20;
				isSortedByDistance = true;
				isKilometers = false;
				numberOfResults = 20;

				editor.PutBoolean("isCelcius", isCelcius);
				editor.PutInt("radius", radius);
				editor.PutBoolean("isSortedByDistance", isSortedByDistance);
				editor.PutBoolean("isKilometers", isKilometers);
				editor.PutInt("numberOfResults", numberOfResults);

				editor.Apply();

				Android.Support.V7.App.AlertDialog.Builder firstTimeNotfication = new Android.Support.V7.App.AlertDialog.Builder(this);
				firstTimeNotfication.SetTitle("Alert");
				firstTimeNotfication.SetMessage("This application needs to use your device's location. Please turn on location under settings if you haven't done so.");
				firstTimeNotfication.SetPositiveButton("OK", (sender, e) =>
															{
																//Alert Dialog closes.
															});

				var alert = firstTimeNotfication.Create();
				alert.Show();

			}
		}

		void CheckInternetConnection()
		{
			if (isOnline == false)
			{
				Android.Support.V7.App.AlertDialog.Builder noInternetConnectionAlert = new Android.Support.V7.App.AlertDialog.Builder(this);
				noInternetConnectionAlert.SetTitle("No Network Connection");
				noInternetConnectionAlert.SetMessage("Network Connection is needed in order to use this application. Please connect to the internet and restart the application.");
				noInternetConnectionAlert.SetPositiveButton("OK", (sender, e) =>
															{
																//Alert Dialog closes.
															});

				var alert = noInternetConnectionAlert.Create();
				alert.Show();

			}
		}


		void InitializeLocationManager()
		{
			_locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

			if (acceptableLocationProviders.Any())
			{
				_locationProvider = acceptableLocationProviders.First();
			}
			else
			{
				_locationProvider = string.Empty;
			}
			Log.Debug(TAG, "Using " + _locationProvider + ".");
		}


		public void OnLocationChanged(Location location)
		{
			_currentLocation = location;

			if (_currentLocation == null)
			{
				Log.Debug("DEBUG", "Cannot get location");
			}
			else
			{
				//Log.Debug("DEBUG", "Current lat and long are: " + _currentLocation.Latitude + " and " + _currentLocation.Longitude);
			}
		}


		public void OnProviderDisabled(string provider) { }



		public void OnProviderEnabled(string provider) { }



		public void OnStatusChanged(string provider, Availability status, Bundle extras) { }


		public async Task InitializeWeatherData()
		{
			await Task.Delay(2000);

			string url;

			if (isOnline == false)
			{
				noInternetConnectionTextView.Visibility = ViewStates.Visible;
				loadingImageView.Visibility = ViewStates.Gone;
				noInternetConnectionTextView.Text = "No Network Connection";
				weatherIcon.Visibility = ViewStates.Invisible;
				temperatureTextView.Visibility = ViewStates.Invisible;
				conditionTextView.Visibility = ViewStates.Invisible;
				humidityTextView.Visibility = ViewStates.Invisible;
				dateTextView.Visibility = ViewStates.Invisible;
			}
			else
			{
				if (_currentLocation == null)
				{
					noInternetConnectionTextView.Visibility = ViewStates.Visible;
					loadingImageView.Visibility = ViewStates.Gone;
					noInternetConnectionTextView.Text = "Could not access device's location";
					weatherIcon.Visibility = ViewStates.Invisible;
					temperatureTextView.Visibility = ViewStates.Invisible;
					conditionTextView.Visibility = ViewStates.Invisible;
					humidityTextView.Visibility = ViewStates.Invisible;
					dateTextView.Visibility = ViewStates.Invisible;

				}
				else
				{
					noInternetConnectionTextView.Visibility = ViewStates.Gone;
					loadingImageView.Visibility = ViewStates.Visible;
					noInternetConnectionTextView.Text = "";
					weatherIcon.Visibility = ViewStates.Visible;
					temperatureTextView.Visibility = ViewStates.Visible;
					conditionTextView.Visibility = ViewStates.Visible;
					humidityTextView.Visibility = ViewStates.Visible;
					dateTextView.Visibility = ViewStates.Visible;

					if (preferences.GetBoolean("isCelcius", false) == true)
					{
						url = "http://api.openweathermap.org/data/2.5/weather?lat=" +
									_currentLocation.Latitude +
									"&lon=" +
									_currentLocation.Longitude +
									"&APPID=" + openWeatherAPIKey
													+ "&units=metric";

						Log.Debug("DEBUG", "This is the weather url: " + url);



					}
					else
					{
						url = "http://api.openweathermap.org/data/2.5/weather?lat=" +
									_currentLocation.Latitude +
									"&lon=" +
									_currentLocation.Longitude +
									"&APPID=" + openWeatherAPIKey
													+ "&units=imperial";

						Log.Debug("DEBUG", "This is the weather url: " + url);


					}

					JsonValue json = await FetchUrlAsync(url);

					loadingImageView.Visibility = ViewStates.Gone;
					weatherInfoLayout.Visibility = ViewStates.Visible;

					DisplayWeather(json);

				}
			}
		}


		//async Task<JsonValue> FetchUrlAsync(string url)
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
		private async Task<JsonValue> FetchUrlAsync(string url)
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


		async void DisplayWeather(JsonValue json)
		{
			var main = json["main"];

			if (preferences.GetBoolean("isCelcius", false) == true)
			{
				var roundedTemperature = Math.Round(Convert.ToDouble(main["temp"].ToString()), 0, MidpointRounding.AwayFromZero);
				temperatureTextView.Text = roundedTemperature.ToString() + "°C";
			}
			else
			{
				var roundedTemperature = Math.Round(Convert.ToDouble(main["temp"].ToString()), 0, MidpointRounding.AwayFromZero);
				temperatureTextView.Text = roundedTemperature.ToString() + "°F";
			}

			humidityTextView.Text = "Humidity: " + main["humidity"].ToString();

			var weather = json["weather"];
			var firstObject = weather[0];
			conditionTextView.Text = FirstCharToUpper(firstObject["description"]);

			DateTime today = DateTime.Today;
			string currentDate = today.ToString("dd/MM/yyyy");
			string month = currentDate[3].ToString() + currentDate[4].ToString();
			string day = currentDate[0].ToString() + currentDate[1].ToString();

			if (day[0].ToString() == "0")
			{
				day = currentDate[1].ToString();
			}

			string year = currentDate[6].ToString() + currentDate[7].ToString() + currentDate[8].ToString() + currentDate[9].ToString();

			dateTextView.Text = ConvertNumberToMonth(month) + ", " + day + ", " + year;

			var imageBitmap = await GetImageBitmapFromUrl(imageUrlString + firstObject["icon"] + ".png");
			weatherIcon.SetImageBitmap(imageBitmap);
			weatherIcon.SetScaleType(ImageView.ScaleType.FitCenter);
		}


		public static string FirstCharToUpper(string input)
		{
			if (String.IsNullOrEmpty(input))
				throw new ArgumentException("ARGH!");
			return input.First().ToString().ToUpper() + input.Substring(1);
		}

		string ConvertNumberToMonth(string monthDigitString)
		{
			if (monthDigitString == "01")
			{
				return "January";
			}
			else if (monthDigitString == "02")
			{
				return "February";
			}
			else if (monthDigitString == "03")
			{
				return "March";
			}
			else if (monthDigitString == "04")
			{
				return "April";
			}
			else if (monthDigitString == "05")
			{
				return "May";
			}
			else if (monthDigitString == "06")
			{
				return "June";
			}
			else if (monthDigitString == "07")
			{
				return "July";
			}
			else if (monthDigitString == "08")
			{
				return "August";
			}
			else if (monthDigitString == "09")
			{
				return "September";
			}
			else if (monthDigitString == "10")
			{
				return "October";
			}
			else if (monthDigitString == "11")
			{
				return "November";
			}
			else if (monthDigitString == "12")
			{
				return "December";
			}
			else
			{
				return "blah";
			}

		}


		//private Bitmap GetImageBitmapFromUrl(string url)
		//{
		//	Bitmap imageBitmap = null;

		//	using (var webClient = new WebClient())
		//	{
		//		var imageBytes = webClient.DownloadData(url);
		//		if (imageBytes != null && imageBytes.Length > 0)
		//		{
		//			imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
		//		}
		//	}

		//	return imageBitmap;
		//}
		private async Task<Bitmap> GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;

			using (var httpClient = new HttpClient())
			{
				var imageBytes = await httpClient.GetByteArrayAsync(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}


		void RestaurantSearchButtonClicked(object sender, EventArgs eventArgs)
		{

			if (isOnline == false)
			{
				Android.Support.V7.App.AlertDialog.Builder noInternetConnectionAlert = new Android.Support.V7.App.AlertDialog.Builder(this);
				noInternetConnectionAlert.SetTitle("No Network Connection");
				noInternetConnectionAlert.SetMessage("Network Connection is needed in order to use this application. Please connect to the internet and restart the application.");
				noInternetConnectionAlert.SetPositiveButton("OK", (s, e) =>
															{
																//Alert Dialog closes.
															});

				var alert = noInternetConnectionAlert.Create();
				alert.Show();
			}
			else
			{

			if (isNear == true)
				{
					if (restaurantTypeEditText.Text != "")
					{
						var intent = new Intent(this, typeof(SearchResultsActivity));
						intent.PutExtra("baseUrl", baseUrl);
						intent.PutExtra("clientID", clientId);
						intent.PutExtra("clientSecret", clientSecret);
						intent.PutExtra("restaurantType", restaurantTypeEditText.Text);
						intent.PutExtra("city", cityEditText.Text);
						intent.PutExtra("sortingType", preferences.GetBoolean("isSortedByDistance", true));
						intent.PutExtra("radius", preferences.GetInt("radius", 20));
						intent.PutExtra("numberOfResults", preferences.GetInt("numberOfResults", 20));
						intent.PutExtra("latitude", _currentLocation.Latitude);
						intent.PutExtra("longitude", _currentLocation.Longitude);
						intent.PutExtra("isNear", isNear);

						StartActivity(intent);
					}
					else
					{
						Android.Support.V7.App.AlertDialog.Builder noEntryNotification = new Android.Support.V7.App.AlertDialog.Builder(this);
						noEntryNotification.SetTitle("Necessary Field");
						noEntryNotification.SetMessage("Please select Restaurant Type");
						noEntryNotification.SetPositiveButton("OK", (s, e) =>
																{
																//Alert Dialog closes.
															});

						var alert = noEntryNotification.Create();
						alert.Show();
					}
				}
				else
				{
					if (restaurantTypeEditText.Text == "")
					{
						Android.Support.V7.App.AlertDialog.Builder noEntryNotification = new Android.Support.V7.App.AlertDialog.Builder(this);
						noEntryNotification.SetTitle("Necessary Field");
						noEntryNotification.SetMessage("Please select Restaurant Type");
						noEntryNotification.SetPositiveButton("OK", (s, e) =>
																{
																//Alert Dialog closes.
															});

						var alert = noEntryNotification.Create();
						alert.Show();
					}
					else if (cityEditText.Text == "")
					{
						Android.Support.V7.App.AlertDialog.Builder noEntryNotification = new Android.Support.V7.App.AlertDialog.Builder(this);
						noEntryNotification.SetTitle("Necessary Field");
						noEntryNotification.SetMessage("Please enter city and state");
						noEntryNotification.SetPositiveButton("OK", (s, e) =>
																{
																//Alert Dialog closes.
															});

						var alert = noEntryNotification.Create();
						alert.Show();
					}
					else
					{
						var intent = new Intent(this, typeof(SearchResultsActivity));
						intent.PutExtra("baseUrl", baseUrl);
						intent.PutExtra("clientID", clientId);
						intent.PutExtra("clientSecret", clientSecret);
						intent.PutExtra("restaurantType", restaurantTypeEditText.Text);
						intent.PutExtra("city", cityEditText.Text);
						intent.PutExtra("sortingType", preferences.GetBoolean("isSortedByDistance", true));
						intent.PutExtra("radius", preferences.GetInt("radius", 20));
						intent.PutExtra("numberOfResults", preferences.GetInt("numberOfResults", 20));
						intent.PutExtra("latitude", _currentLocation.Latitude);
						intent.PutExtra("longitude", _currentLocation.Longitude);
						intent.PutExtra("isNear", isNear);

						StartActivity(intent);
					}
				}
			}

		}


		void AdvancedButtonClicked(object sender, EventArgs eventArgs)
		{
			cityEditText.Visibility = ViewStates.Visible;
			isNear = false;
			restaurantSearchButton.Text = "SEARCH";
			advancedSearchButton.Visibility = ViewStates.Gone;
			hideButton.Visibility = ViewStates.Visible;
		}


		void HideButtonClicked(object sender, EventArgs eventArgs)
		{
			cityEditText.Visibility = ViewStates.Invisible;
			isNear = true;
			restaurantSearchButton.Text = "RESTAURANT NEAR ME";
			advancedSearchButton.Visibility = ViewStates.Visible;
			hideButton.Visibility = ViewStates.Gone;
		}


		void InvisibleButtonClicked(object sender, EventArgs eventArgs)
		{
			var intent = new Intent(this, typeof(RestaurantSelectionActivity));
			StartActivity(intent);
		}

		public void wifiStatusChangedFromBroadcastReceiver(bool status)
		{
			isOnline = status;
			//if (status)
			//{
			//	Toast.MakeText(this, status.ToString(), ToastLength.Short).Show();
			//}
			//else {
			//	Toast.MakeText(this, status.ToString(), ToastLength.Short).Show();
			//}
		}

	}
}

