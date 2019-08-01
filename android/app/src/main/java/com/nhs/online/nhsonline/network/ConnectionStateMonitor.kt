package com.nhs.online.nhsonline.network

import android.content.Context
import android.net.*
import android.net.ConnectivityManager.NetworkCallback
import android.os.Build
import android.util.Log
import com.nhs.online.nhsonline.Application

class ConnectionStateMonitor(val context: Context) : NetworkCallback() {
    companion object {
        var isConnectedToNetwork: Boolean = false
            private set
    }
    private val networkRequest = NetworkRequest.Builder()

    private val connectivityManager = context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager

    fun registerNetworkCallback() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering registerNetworkCallback")
        connectivityManager.registerNetworkCallback(networkRequest.build(), this)
    }

    fun deregisterNetworkCallback() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering deregisterNetworkCallback")
        connectivityManager.unregisterNetworkCallback(this)
    }

    override fun onLost(network: Network?) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLost, Is connected to network: " + isConnectedToNetwork)

        if (Build.VERSION.SDK_INT < 23) {
            val networkInfo = connectivityManager.getActiveNetworkInfo()

            if (networkInfo != null) {
                isConnectedToNetwork = (networkInfo.isConnected() &&
                        (networkInfo.getType() == ConnectivityManager.TYPE_WIFI ||
                                networkInfo.getType() == ConnectivityManager.TYPE_MOBILE))
            }
        } else {
            val activeNetwork = connectivityManager.getActiveNetwork()

            if (activeNetwork != null) {
                val capabilities = connectivityManager.getNetworkCapabilities(activeNetwork)

                if (capabilities != null) {
                    isConnectedToNetwork =
                            (capabilities.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR) ||
                                    capabilities.hasTransport(NetworkCapabilities.TRANSPORT_WIFI) ||
                                    capabilities.hasTransport(NetworkCapabilities.TRANSPORT_VPN))
                }
            }
        }

        Log.d(Application.TAG, "${this::class.java.simpleName}: Exiting onLost, Is connected to network: " + isConnectedToNetwork)
    }

    override fun onAvailable(network: Network) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onAvailable, Is connected to network")
        isConnectedToNetwork = true
    }

}