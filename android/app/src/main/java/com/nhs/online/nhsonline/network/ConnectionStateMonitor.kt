package com.nhs.online.nhsonline.network

import android.content.Context
import android.net.ConnectivityManager
import android.net.ConnectivityManager.NetworkCallback
import android.net.Network
import android.net.NetworkCapabilities
import android.net.NetworkRequest
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

        val activeNetwork = connectivityManager.getActiveNetwork()

        if (activeNetwork != null) {
            val capabilities = connectivityManager.getNetworkCapabilities(activeNetwork)

            if (capabilities != null) {
                isConnectedToNetwork =
                        (capabilities.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR) ||
                                capabilities.hasTransport(NetworkCapabilities.TRANSPORT_WIFI) ||
                                capabilities.hasTransport(NetworkCapabilities.TRANSPORT_VPN))
            }
        } else {
            isConnectedToNetwork = false
        }

        Log.d(Application.TAG, "${this::class.java.simpleName}: Exiting onLost, Is connected to network: " + isConnectedToNetwork)
    }

    override fun onAvailable(network: Network) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onAvailable, Is connected to network")
        isConnectedToNetwork = true
    }

}