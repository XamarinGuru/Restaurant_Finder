
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Restaurant_Finder
{
	[Activity(Label = "MapActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MapActivity : Activity, IOnMapReadyCallback

	{
		GoogleMap map;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Map);

			var mapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapFragment);
			mapFrag.GetMapAsync(this);

			//Button normalButton = FindViewById<Button>(Resource.Id.normalBtn);
			//Button satelliteButton = FindViewById<Button>(Resource.Id.satelliteBtn);
			//Button hybridButton = FindViewById<Button>(Resource.Id.hybridBtn);
			//TextView restaurantNameTextView = FindViewById<TextView>(Resource.Id.resName);

			//double lat = Intent.GetDoubleExtra("lat", 0.0);
			//double lng = Intent.GetDoubleExtra("lng", 0.0);
			//string restaurantName = Intent.GetStringExtra("name");
			//double userLat = MainActivity._currentLocation.Latitude;
			//double userLng = MainActivity._currentLocation.Longitude;

			//restaurantNameTextView.Text = restaurantName;

			//if (map != null)
			//{
			//	map.MapType = GoogleMap.MapTypeNormal;

			//	map.UiSettings.MyLocationButtonEnabled = true;
			//	map.UiSettings.ZoomControlsEnabled = true;
			//	map.UiSettings.CompassEnabled = true;
			//	map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(userLat, userLng), 15f));

			//	BitmapDescriptor currentLocationIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_currentLocation);

			//	MarkerOptions markerUser = new MarkerOptions();
			//	markerUser.SetPosition(new LatLng(userLat, userLng));
			//	markerUser.SetTitle("You");
			//	markerUser.SetIcon(currentLocationIcon);
			//	map.AddMarker(markerUser);

			//	MarkerOptions markerRestaurant = new MarkerOptions();
			//	markerRestaurant.SetPosition(new LatLng(lat, lng));
			//	markerRestaurant.SetTitle(restaurantName);
			//	map.AddMarker(markerRestaurant);

			//	normalButton.SetTextColor(Color.ParseColor("transparent"));
			//	normalButton.SetBackgroundResource(Resource.Drawable.button_border_clicked);
			//	satelliteButton.SetTextColor(Color.ParseColor("#ffe4b331"));
			//	satelliteButton.SetBackgroundResource(Resource.Drawable.button_border);
			//	hybridButton.SetTextColor(Color.ParseColor("#ffe4b331"));
			//	hybridButton.SetBackgroundResource(Resource.Drawable.button_border);

			//	normalButton.Click += (sender, e) =>
			//	{
			//		map.MapType = GoogleMap.MapTypeNormal;

			//		normalButton.SetTextColor(Color.ParseColor("transparent"));
			//		normalButton.SetBackgroundResource(Resource.Drawable.button_border_clicked);
			//		satelliteButton.SetTextColor(Color.ParseColor("#ffe4b331"));
			//		satelliteButton.SetBackgroundResource(Resource.Drawable.button_border);
			//		hybridButton.SetTextColor(Color.ParseColor("#ffe4b331"));
			//		hybridButton.SetBackgroundResource(Resource.Drawable.button_border);
			//	};

			//	satelliteButton.Click += (sender, e) =>
			//	{
			//		map.MapType = GoogleMap.MapTypeSatellite;

			//		satelliteButton.SetTextColor(Color.ParseColor("transparent"));
			//		satelliteButton.SetBackgroundResource(Resource.Drawable.button_border_clicked);
			//		normalButton.SetTextColor(Color.ParseColor("#ffe4b331"));
			//		normalButton.SetBackgroundResource(Resource.Drawable.button_border);
			//		hybridButton.SetTextColor(Color.ParseColor("#ffe4b331"));
			//		hybridButton.SetBackgroundResource(Resource.Drawable.button_border);
			//	};

			//	hybridButton.Click += (sender, e) =>
			//	{
			//		map.MapType = GoogleMap.MapTypeHybrid;

			//		hybridButton.SetTextColor(Color.ParseColor("transparent"));
			//		hybridButton.SetBackgroundResource(Resource.Drawable.button_border_clicked);
			//		normalButton.SetTextColor(Color.ParseColor("#ffe4b331"));
			//		normalButton.SetBackgroundResource(Resource.Drawable.button_border);
			//		satelliteButton.SetTextColor(Color.ParseColor("#ffe4b331"));
			//		satelliteButton.SetBackgroundResource(Resource.Drawable.button_border);
			//	};
			//}

		}

		public void OnMapReady(GoogleMap _map)
		{
			map = _map;

			Button normalButton = FindViewById<Button>(Resource.Id.normalBtn);
			Button satelliteButton = FindViewById<Button>(Resource.Id.satelliteBtn);
			Button hybridButton = FindViewById<Button>(Resource.Id.hybridBtn);
			TextView restaurantNameTextView = FindViewById<TextView>(Resource.Id.resName);

			double lat = Intent.GetDoubleExtra("lat", 0.0);
			double lng = Intent.GetDoubleExtra("lng", 0.0);
			string restaurantName = Intent.GetStringExtra("name");
			double userLat = MainActivity._currentLocation.Latitude;
			double userLng = MainActivity._currentLocation.Longitude;

			restaurantNameTextView.Text = restaurantName;

			if (map != null)
			{
				map.MapType = GoogleMap.MapTypeNormal;

				map.UiSettings.MyLocationButtonEnabled = true;
				map.UiSettings.ZoomControlsEnabled = true;
				map.UiSettings.CompassEnabled = true;
				map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(userLat, userLng), 15f));

				BitmapDescriptor currentLocationIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.ic_currentLocation);

				MarkerOptions markerUser = new MarkerOptions();
				markerUser.SetPosition(new LatLng(userLat, userLng));
				markerUser.SetTitle("You");
				markerUser.SetIcon(currentLocationIcon);
				map.AddMarker(markerUser);

				MarkerOptions markerRestaurant = new MarkerOptions();
				markerRestaurant.SetPosition(new LatLng(lat, lng));
				markerRestaurant.SetTitle(restaurantName);
				map.AddMarker(markerRestaurant);

				normalButton.SetTextColor(Color.Transparent);
				normalButton.SetBackgroundResource(Resource.Drawable.button_border_clicked);
				satelliteButton.SetTextColor(Color.ParseColor("#ffe4b331"));
				satelliteButton.SetBackgroundResource(Resource.Drawable.button_border);
				hybridButton.SetTextColor(Color.ParseColor("#ffe4b331"));
				hybridButton.SetBackgroundResource(Resource.Drawable.button_border);

				normalButton.Click += (sender, e) =>
				{
					map.MapType = GoogleMap.MapTypeNormal;

					normalButton.SetTextColor(Color.Transparent);
					normalButton.SetBackgroundResource(Resource.Drawable.button_border_clicked);
					satelliteButton.SetTextColor(Color.ParseColor("#ffe4b331"));
					satelliteButton.SetBackgroundResource(Resource.Drawable.button_border);
					hybridButton.SetTextColor(Color.ParseColor("#ffe4b331"));
					hybridButton.SetBackgroundResource(Resource.Drawable.button_border);
				};

				satelliteButton.Click += (sender, e) =>
				{
					map.MapType = GoogleMap.MapTypeSatellite;

					satelliteButton.SetTextColor(Color.Transparent);
					satelliteButton.SetBackgroundResource(Resource.Drawable.button_border_clicked);
					normalButton.SetTextColor(Color.ParseColor("#ffe4b331"));
					normalButton.SetBackgroundResource(Resource.Drawable.button_border);
					hybridButton.SetTextColor(Color.ParseColor("#ffe4b331"));
					hybridButton.SetBackgroundResource(Resource.Drawable.button_border);
				};

				hybridButton.Click += (sender, e) =>
				{
					map.MapType = GoogleMap.MapTypeHybrid;

					hybridButton.SetTextColor(Color.Transparent);
					hybridButton.SetBackgroundResource(Resource.Drawable.button_border_clicked);
					normalButton.SetTextColor(Color.ParseColor("#ffe4b331"));
					normalButton.SetBackgroundResource(Resource.Drawable.button_border);
					satelliteButton.SetTextColor(Color.ParseColor("#ffe4b331"));
					satelliteButton.SetBackgroundResource(Resource.Drawable.button_border);
				};
			}
		}
	}
		
}
