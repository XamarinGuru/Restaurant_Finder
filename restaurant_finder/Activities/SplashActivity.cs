using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Util;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Content.PM;

namespace Restaurant_Finder
{
	[Activity(Theme = "@android:style/Theme.NoTitleBar", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class SplashActivity : Activity
	{
		static readonly string TAG = "X:" + typeof(SplashActivity).Name;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Splash);

			await Task.Delay(2000);

			Log.Debug(TAG, "Work is finished - start Activity1.");
			StartActivity(new Intent(Application.Context, typeof(MainActivity)));

			Log.Debug(TAG, "SplashActivity.OnCreate");
		}
	}
}
