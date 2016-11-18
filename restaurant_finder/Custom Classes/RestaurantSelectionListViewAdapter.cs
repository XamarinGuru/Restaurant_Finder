using System;
using Android.App;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Graphics;
using System.Net;
using Android.Graphics.Drawables;
using Android.Content;

namespace Restaurant_Finder
{
	public class RestaurantSelectionListViewAdapter : BaseAdapter<string>
	{
		Activity context;
		string[] restaurantTypes;

		public RestaurantSelectionListViewAdapter(Activity _context, string[] _restaurantTypes)
		{
			context = _context;
			restaurantTypes = _restaurantTypes;
		}


		public override long GetItemId(int position)
		{
			return position;
		}


		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		public override string this[int position]
		{
			get
			{
				return restaurantTypes[position];
			}
		}


		public override int Count
		{
			get
			{
				return restaurantTypes.Length;
			}
		} 


		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView;

			if (view == null)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.RestaurantTypeRow, null);
			}

			TextView restaurantNameTextView = view.FindViewById<TextView>(Resource.Id.restaurantNameTextView);

			restaurantNameTextView.Text = restaurantTypes[position];

			return view;
		}
	}
}
