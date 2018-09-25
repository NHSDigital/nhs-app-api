package com.nhs.online.nhsonline.network

import android.content.Context
import android.net.ConnectivityManager
import android.net.NetworkInfo

class Reachability {
    companion object {
        fun isConnectedToNetwork(context: Context): Boolean {
            val cm = context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
            val activeNetworkInfo: NetworkInfo? = cm.activeNetworkInfo
            return activeNetworkInfo?.isConnectedOrConnecting == true
        }
    }
}