
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Webkit;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using ToolBar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using System.Threading;
using System.Threading.Tasks;

namespace Restaurant_Finder
{
	[Activity(Label = "RestaurantMainPageActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class RestaurantMainPageActivity : AppCompatActivity
	{

		static WebView web_view;
		static ImageView loadingImageView;
		private TextView toolBarTitle;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.RestaurantMainPage);

			string restaurantName = Intent.GetStringExtra("resName");

			toolBarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);

			ToolBar toolBar = FindViewById<ToolBar>(Resource.Id.toolbarRestaurantMenu);
			SetSupportActionBar(toolBar);
			SupportActionBar.SetDisplayShowTitleEnabled(false);
			toolBarTitle.Text = restaurantName;

			web_view = FindViewById<WebView>(Resource.Id.webView1);
			loadingImageView = FindViewById<ImageView>(Resource.Id.loadingImageViewMenu);

			//web_view.Visibility = ViewStates.Invisible;
			//loadingImageView.Visibility = ViewStates.Visible;

			string urlString = Intent.GetStringExtra("url");

			web_view.Settings.JavaScriptEnabled = true;
			web_view.SetWebViewClient(new HelloWebViewClient(this, loadingImageView));
			web_view.LoadUrl(urlString);
			//RestaurantMainPageActivity.loadingImageView.Visibility = ViewStates.Invisible;
			//RestaurantMainPageActivity.web_view.Visibility = ViewStates.Visible;

		}


		public class HelloWebViewClient : WebViewClient
		{
			private ImageView loadingView;

			bool loadingFinished = true;
			bool redirect = false;

			public HelloWebViewClient(Activity mActivity, ImageView loadingImageView)
			{
				this.loadingView = loadingImageView;
			}

			public override bool ShouldOverrideUrlLoading(WebView view, string url)
			{
				if (!loadingFinished) redirect = true;

				loadingFinished = false;
				loadingView.Visibility = ViewStates.Visible;
				view.LoadUrl(url);

				return true;
			}

			public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
			{
				base.OnPageStarted(view, url, favicon);
				loadingFinished = false;
			}
			public override void OnPageFinished(WebView view, string url)
			{
				base.OnPageFinished(view, url);

				if (!redirect)
				{
					loadingFinished = true;
				}

				if (loadingFinished && !redirect)
				{
					loadingView.Visibility = ViewStates.Invisible;
				}
				else {
					redirect = false;
				}

			}
		}


		public override bool OnKeyDown(Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
		{
			if (keyCode == Keycode.Back && web_view.CanGoBack())
			{
				web_view.GoBack();

				return true;
			}

			return base.OnKeyDown(keyCode, e);
		}
	}
}
