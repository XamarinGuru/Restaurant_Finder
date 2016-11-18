
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Restaurant_Finder
{
	//[BroadcastReceiver]
	//public class NetConnectionChangeReceiver : BroadcastReceiver
	//{
	//	public override void OnReceive(Context context, Intent intent)
	//	{
	//		//Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
	//		ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
	//		NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
	//		bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
	//		Toast.MakeText(context, isOnline.ToString(), ToastLength.Short).Show();
	//		//ConnectivityManager connectivityManager = (ConnectivityManager)context.getSystemService(Context.CONNECTIVITY_SERVICE);
	//		//NetworkInfo activeNetInfo = connectivityManager.getActiveNetworkInfo();
	//		//NetworkInfo mobNetInfo = connectivityManager.getNetworkInfo(ConnectivityManager.TYPE_MOBILE);
	//		//if (activeNetInfo != null)
	//		//{
	//		//	Toast.makeText(context, "Active Network Type : " + activeNetInfo.getTypeName(), Toast.LENGTH_SHORT).show();
	//		//}
	//		//if (mobNetInfo != null)
	//		//{
	//		//	Toast.makeText(context, "Mobile Network Type : " + mobNetInfo.getTypeName(), Toast.LENGTH_SHORT).show();
	//		//}
	//	}
	//}
	[BroadcastReceiver]
	[IntentFilter(new string[] { "android.net.conn.CONNECTIVITY_CHANGE" })]

	public class NetConnectionChangeReceiver : BroadcastReceiver
	{
		public static MainActivity activity;

		public override void OnReceive(Context context, Intent intent)
		{
			var connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
			var mobileState = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi).GetState();
			var noConnection = mobileState == NetworkInfo.State.Connected;

			if (noConnection)
			{

				if (activity != null)
				{
					activity.wifiStatusChangedFromBroadcastReceiver(true);
				}

			}
			else {

				if (activity != null)
				{
					activity.wifiStatusChangedFromBroadcastReceiver(false);
				}
			}

			if (NetworkStatusChanged != null)
			{
				NetworkStatusChanged(this, EventArgs.Empty);
			}
		}

		public EventHandler NetworkStatusChanged;
	}
}
