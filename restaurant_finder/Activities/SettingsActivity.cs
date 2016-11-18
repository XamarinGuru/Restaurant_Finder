
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
using ToolBar = Android.Support.V7.Widget.Toolbar;

namespace Restaurant_Finder
{
	[Activity(Label = "SettingsActivity", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
	public class SettingsActivity : AppCompatActivity
	{
		private ListView settingsListView;
		private Button saveButton;
		public static ArrayAdapter spinnerAdapter;
		private SettingsListViewAdapter adapter;
		private TextView toolBarTitle;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Settings);

			toolBarTitle = FindViewById<TextView>(Resource.Id.toolbar_title);

			ToolBar toolBar = FindViewById<ToolBar>(Resource.Id.toolbarSettings);
			SetSupportActionBar(toolBar);
			SupportActionBar.SetDisplayShowTitleEnabled(false);
			toolBarTitle.Text = "Settings";
		
			saveButton = FindViewById<Button>(Resource.Id.buttonSave);
			saveButton.Click += saveButtonClicked;

			settingsListView = FindViewById<ListView>(Resource.Id.listView1);
			adapter = new SettingsListViewAdapter(this);
			settingsListView.Adapter = adapter;

			spinnerAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.DistanceUnitTypes, Resource.Layout.SpinnerCustom);
			spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

		}

		void saveButtonClicked(object sender, EventArgs eventArgs)
		{
			adapter.CheckRadiusBarValue();
			adapter.CheckNumberOfResultsBarValue();
			this.Finish();
		}
	}
}
