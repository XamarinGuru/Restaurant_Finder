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
using Android.Content.Res;

namespace Restaurant_Finder
{
	public class SearchResultsListViewAdapter : BaseAdapter
	{
		Activity context;
		List<RestaurantModel> data;
		bool isNear;

		public SearchResultsListViewAdapter(Activity _context, List<RestaurantModel> _data, bool _isNear)
		{
			context = _context;
			data = _data;
			isNear = _isNear;
		}


		public override long GetItemId(int position)
		{
			return position;
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}


		public override int Count
		{
			get
			{
				return data.Count;
			}
		}



		public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View view = convertView;

			if (view == null)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.RestaurantRow, parent, false);
			}

			RestaurantModel restaurantObject = data[position];

			ImageView restaurantImage = view.FindViewById<Refractored.Controls.CircleImageView>(Resource.Id.imageViewRestaurantPicture);
			ImageView distanceImageView = view.FindViewById<ImageView>(Resource.Id.imageViewDistance);
			TextView distanceValueTextView = view.FindViewById<TextView>(Resource.Id.textViewDistance);
			TextView distanceUnitsTextView = view.FindViewById<TextView>(Resource.Id.textViewDistanceUnits);
			TextView restaurantNameTextView = view.FindViewById<TextView>(Resource.Id.textViewRestaurantName);
			TextView restaurantAddressTextView = view.FindViewById<TextView>(Resource.Id.textViewRestaurantAddress);
			TextView restaurantNumberTextView = view.FindViewById<TextView>(Resource.Id.textViewRestaurantPhoneNumber);
			TextView noRatingTextView = view.FindViewById<TextView>(Resource.Id.textViewNoRatingAvailable);
			Button callButton = view.FindViewById<Button>(Resource.Id.ButtonCall);
			var callBtnContent = view.FindViewById<RelativeLayout>(Resource.Id.callBtnContent);
			Button urlButton = view.FindViewById<Button>(Resource.Id.ButtonURL);
			var urlBtnContent = view.FindViewById<RelativeLayout>(Resource.Id.urlBtnContent);
			Button mapButton = view.FindViewById<Button>(Resource.Id.ButtonMap);
			RatingBar restauarantRatingBar = view.FindViewById<RatingBar>(Resource.Id.ratingRestaurant);


			if (restaurantObject.imageString != null)
			{
				/*var imageBitmap = GetImageBitmapFromUrl(restaurantObject.imageString);
				restaurantImage.SetImageBitmap(imageBitmap);*/

				Koush.UrlImageViewHelper.SetUrlDrawable(restaurantImage, restaurantObject.imageString, Resource.Drawable.ic_restaurantPlaceholder);
			}
			else
			{
				restaurantImage.SetImageResource(Resource.Drawable.ic_restaurantPlaceholder);
			}


			if (restaurantObject.url == null)
			{
				urlButton.Visibility = ViewStates.Invisible;
				urlBtnContent.Visibility = ViewStates.Invisible;
			}
			else
			{
				urlButton.Visibility = ViewStates.Visible;
				urlBtnContent.Visibility = ViewStates.Visible;
			}


			restaurantNameTextView.Text = restaurantObject.restaurantName;

			if (isNear == true)
			{
				if (restaurantObject.streetAddress != null)
				{
					restaurantAddressTextView.Text = restaurantObject.streetAddress + ", " + restaurantObject.cityAdress + ", " + restaurantObject.stateAdress;
				}
				else
				{
					restaurantAddressTextView.Text = restaurantObject.cityAdress + ", " + restaurantObject.stateAdress + ", " + restaurantObject.ccAdress;
				}
			}
			else
			{
				if (restaurantObject.streetAddress != null)
				{
					restaurantAddressTextView.Text = restaurantObject.streetAddress + ", " + restaurantObject.cityAdress + ", " + restaurantObject.stateAdress;
				}
				else
				{
					restaurantAddressTextView.Text = restaurantObject.cityAdress + ", " + restaurantObject.stateAdress + ", " + restaurantObject.ccAdress;
				}
			}

			if (isNear == true)
			{
				if (MainActivity.preferences.GetBoolean("isKilometers", false) == false)
				{
					string meterDistanceString = restaurantObject.distance.ToString();
					double meterDistance = Convert.ToDouble(meterDistanceString);
					double mileDistance = meterDistance / 1609.34;
					double roundedMileDistance = Math.Round(mileDistance, 2);
					distanceValueTextView.Text = roundedMileDistance.ToString();
					distanceUnitsTextView.Text = "miles";
				}
				else
				{
					string meterDistanceString = restaurantObject.distance.ToString();
					double meterDistance = Convert.ToDouble(meterDistanceString);
					double kmsDistance = meterDistance / 1000;
					double roundedkmsDistance = Math.Round(kmsDistance, 2);
					distanceValueTextView.Text = roundedkmsDistance.ToString();
					distanceUnitsTextView.Text = "kms";
				}
			}
			else
			{
				distanceImageView.Visibility = ViewStates.Gone;
				distanceValueTextView.Visibility = ViewStates.Gone;
				distanceUnitsTextView.Visibility = ViewStates.Gone;
			}

			if (restaurantObject.phoneNumber != null)
			{
				restaurantNumberTextView.Text = "Ph : " + restaurantObject.phoneNumber;
			}
			else if (restaurantObject.nonFormattedPhoneNumber != null)
			{
				restaurantNumberTextView.Text = "Ph : " + restaurantObject.nonFormattedPhoneNumber;
			}
			else
			{
				callButton.Visibility = ViewStates.Invisible;
				callBtnContent.Visibility = ViewStates.Invisible;
				restaurantNumberTextView.Text = "";
			}


			if (restaurantObject.ratingColor != null)
			{
				float rating = Convert.ToSingle(restaurantObject.rating);
				float halfRating = rating / 2;
				Console.WriteLine("The rating of the restaurant is:D {0}", halfRating);
				restauarantRatingBar.Rating = 3.5f;
				/*Drawable drawable = restauarantRatingBar.ProgressDrawable;
				drawable.SetColorFilter(Color.ParseColor("#" + ""), PorterDuff.Mode.SrcAtop);*/
				restauarantRatingBar.Visibility = ViewStates.Visible;
				noRatingTextView.Visibility = ViewStates.Invisible;
			}
			else
			{
				restauarantRatingBar.Visibility = ViewStates.Invisible;
				noRatingTextView.Visibility = ViewStates.Visible;
			}



			callButton.Click += (sender, e) =>
			{
				var uri = Android.Net.Uri.Parse("tel:" + restaurantObject.nonFormattedPhoneNumber);
				var intent = new Intent(Intent.ActionDial, uri);
				context.StartActivity(intent);
			};

			urlButton.Click += (sender, e) =>
			{
				var intent = new Intent(context, typeof(RestaurantMainPageActivity));
				intent.PutExtra("url", restaurantObject.url);
				intent.PutExtra("resName", restaurantObject.restaurantName);
				context.StartActivity(intent);
			};

			mapButton.Click += (sender, e) =>
			{
				var intent = new Intent(context, typeof(MapActivity));
				intent.PutExtra("lat", restaurantObject.lat);
				intent.PutExtra("lng", restaurantObject.lng);
				intent.PutExtra("name", restaurantObject.restaurantName);
				context.StartActivity(intent);
			};

			return view;
			
		}


		private Bitmap GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;

			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}
	}
}
