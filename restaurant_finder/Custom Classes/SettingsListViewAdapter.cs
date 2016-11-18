using System;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Restaurant_Finder
{
	public class SettingsListViewAdapter : BaseAdapter
	{
		Activity context;
		public SwitchCompat sortingSwitch;
		public SwitchCompat temperatureUnitsSwitch;
		public SeekBar radiusSeekBar;
		public SeekBar numberOfResultsSeekBar;
		public Spinner distanceUnitsSpinner;
		public TextView radiusTextView;


		public SettingsListViewAdapter(Activity _context)
		{
			context = _context;
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
				return 6;
			}
		}



		public override Android.Views.View GetView(int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{

			View view;

			if (position == 0)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.SortingSettingsRow, parent, false);
				sortingSwitch = view.FindViewById<SwitchCompat>(Resource.Id.sortingSwitch);
				CheckSortingSwitchState();

				sortingSwitch.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
				{
					if (e.IsChecked == true)
					{
						MainActivity.isSortedByDistance = true;
						MainActivity.editor.PutBoolean("isSortedByDistance", MainActivity.isSortedByDistance);
						MainActivity.editor.Apply();
					}
					else
					{
						MainActivity.isSortedByDistance = false;
						MainActivity.editor.PutBoolean("isSortedByDistance", MainActivity.isSortedByDistance);
						MainActivity.editor.Apply();
					}
				};

				return view;
			}
			else if (position == 1)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.RadiusSettingsRow, parent, false);
				radiusSeekBar = view.FindViewById<SeekBar>(Resource.Id.seekBarRadius);
				radiusTextView = view.FindViewById<TextView>(Resource.Id.textViewRadius);
				CheckRadiusBarValue();

				radiusSeekBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
				{
					if (e.FromUser)
					{
						SeekBar seekbar = (SeekBar)sender;

						if (seekbar.Progress == 0)
						{
							MainActivity.editor.PutInt("radius", 1);
							MainActivity.editor.Apply();
						}
						else 
						{
							MainActivity.editor.PutInt("radius", e.Progress);
							MainActivity.editor.Apply();
						}

					}
				};

				return view;
			}
			else if (position == 2)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.NumberOfResultsSettingsRow, parent, false);
				numberOfResultsSeekBar = view.FindViewById<SeekBar>(Resource.Id.seekBarResults);
				CheckNumberOfResultsBarValue();

				numberOfResultsSeekBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
				{
					if (e.FromUser)
					{
						MainActivity.editor.PutInt("numberOfResults", e.Progress + 10);
						MainActivity.editor.Apply();
					}
				};

				return view;
			}
			else if (position == 3)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.DistanceUnitsSettingsRow, parent, false);
				distanceUnitsSpinner = view.FindViewById<Spinner>(Resource.Id.spinnerDistanceUnits);
				distanceUnitsSpinner.Adapter = SettingsActivity.spinnerAdapter;
				CheckDistanceUnits();

				distanceUnitsSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(SpinnerItemSelected);

				return view;
			}
			else if (position == 4)
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.TemperatureSettingsRow, parent, false);
				temperatureUnitsSwitch = view.FindViewById<SwitchCompat>(Resource.Id.tempUnitsSwitch);
				CheckTemperatureUnitsSwitchState();

				temperatureUnitsSwitch.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
				{
					if (e.IsChecked == true)
					{
						MainActivity.isCelcius = false;
						MainActivity.editor.PutBoolean("isCelcius", MainActivity.isCelcius);
						MainActivity.editor.Apply();
						string currentTempString = "";
						for (int i = 0; i < MainActivity.temperatureTextView.Text.Length - 2; i++)
						{
							currentTempString += MainActivity.temperatureTextView.Text[i];
						}
						double currentTempValue = Convert.ToDouble(currentTempString);
						double newTempValue = currentTempValue * 1.8 + 32.0;
						var roundedNewTempValue = Math.Round(newTempValue, 0, MidpointRounding.AwayFromZero);
						MainActivity.temperatureTextView.Text = roundedNewTempValue.ToString() + "°F";
					}
					else 
					{
						MainActivity.isCelcius = true;
						MainActivity.editor.PutBoolean("isCelcius", MainActivity.isCelcius);
						MainActivity.editor.Apply();
						string currentTempString = "";
						for (int i = 0; i < MainActivity.temperatureTextView.Text.Length - 2; i++)
						{
							currentTempString += MainActivity.temperatureTextView.Text[i];
						}
						double currentTempValue = Convert.ToDouble(currentTempString);
						double newTempValue = (currentTempValue - 32) * 5 / 9;
						var roundedNewTempValue = Math.Round(newTempValue, 0, MidpointRounding.AwayFromZero);
						MainActivity.temperatureTextView.Text = roundedNewTempValue.ToString() + "°C";
					}

				};

				return view;
			}
			else
			{
				view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);
				return view;
			}

		}

		void CheckSortingSwitchState()
		{
			if (MainActivity.preferences.GetBoolean("isSortedByDistance", true) == true)
			{
				sortingSwitch.Checked = true;
			}
			else 
			{
				sortingSwitch.Checked = false;
			}
		}


		void CheckTemperatureUnitsSwitchState()
		{
			if (MainActivity.preferences.GetBoolean("isCelcius", true) == true)
			{
				temperatureUnitsSwitch.Checked = false;
			}
			else 
			{
				temperatureUnitsSwitch.Checked = true;
			}
		}


		public void CheckRadiusBarValue()
		{
			radiusSeekBar.Progress = MainActivity.preferences.GetInt("radius", 20);
		}


		public void CheckNumberOfResultsBarValue()
		{
			int number = MainActivity.preferences.GetInt("numberOfResults", 20);

			numberOfResultsSeekBar.Progress = number - 10;
		}


		void CheckDistanceUnits()
		{
			if (MainActivity.preferences.GetBoolean("isKilometers", false) == true)
			{
				distanceUnitsSpinner.SetSelection(0);
				radiusTextView.Text = "  Radius(Kms):";
			}
			else
			{
				distanceUnitsSpinner.SetSelection(1);
				radiusTextView.Text = "   Radius(Miles):";
			}
		}


		void SpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;

			if (spinner.SelectedItem.ToString() == "Kms")
			{
				MainActivity.isKilometers = true;
				MainActivity.editor.PutBoolean("isKilometers", MainActivity.isKilometers);
				MainActivity.editor.Apply();
				radiusTextView.Text = "  Radius(Kms):";
			}
			else
			{
				MainActivity.isKilometers = false;
				MainActivity.editor.PutBoolean("isKilometers", MainActivity.isKilometers);
				MainActivity.editor.Apply();
				radiusTextView.Text = "  Radius(Miles):";
			}
		}
	}
}
