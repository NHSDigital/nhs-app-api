package com.nhs.online.nhsonline.network

import android.content.Context
import android.net.ConnectivityManager
import android.net.NetworkInfo
import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig

class Reachability {
    companion object {
        fun isConnectedToNetwork(context: Context): Boolean {
            val cm = context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
            val activeNetworkInfo: NetworkInfo? = cm.activeNetworkInfo
            Log.d(Application.TAG, "${this::class.java.simpleName}: Entering isConnectedToNetwork > ${activeNetworkInfo?.isConnectedOrConnecting}")

            return activeNetworkInfo?.isConnectedOrConnecting == true
        }
    }
}