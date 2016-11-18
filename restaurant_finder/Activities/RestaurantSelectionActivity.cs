
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace Restaurant_Finder
{
	[Activity(Label = "RestaurantSelectionActivity", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
	public class RestaurantSelectionActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.RestaurantSelection);

			ListView restaurantTypesListView = FindViewById<ListView>(Resource.Id.listViewSelection);

			string[] restaurantTypesArray = {"All",
											"African Restaurant",
											"Afghan Restaurant",
											"American Restaurant",
											"Asian Restaurant",
											"BBQ Joint",
											"Bakery",
											"Bar",
											"Belgian Restaurant",
											"Bistro",
											"Brazilian Restaurant",
											"Breakfast Spot",
											"Bubble Tea Shop",
											"Burger Joint",
											"Burmese Restaurant",
											"Cafe",
											"Caribbean Restaurant",
											"Chinese Restaurant",
											"Coffee Shop",
											"Creperie",
											"Curban Restaurant",
											"Deli/Bodega",
											"Fast Food Restaurant",
											"Filipino Restaurant",
											"French Restaurant",
											"German Restaurant",
											"Greek Restaurant",
											"Hot Dog Joint",
											"Indian Restaurant",
											"Indonesian Restaurant",
											"Italian Restaurant",
											"Japanese Restauran",
											"Korean Restaurant",
											"Latin American Restaurant",
											"Malaysian Restaurant",
											"Mediterranean Restaurant",
											"Mexican Restaurant",
											"Nightclub",
											"Pizza Place",
											"Portuguese Restaurant",
											"Poutine Place",
											"Pub",
											"Sandwich Place",
											"Seafood Restaurant",
											"Soup Place",
											"South American Restaurant",
											"Spanish Restaurant",
											"Sushi Restaurant",
											"Thai Restaurant",
											"Turkish Restaurant",
											"Vegetarian/Vegan Restaurant",
											"Vietnamese Restaurant"};

			var adapter = new RestaurantSelectionListViewAdapter(this, restaurantTypesArray);
			restaurantTypesListView.Adapter = adapter;

			restaurantTypesListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				MainActivity.restaurantTypeEditText.Text = restaurantTypesArray[e.Position];
				this.Finish();
			};


		}
	}
}
