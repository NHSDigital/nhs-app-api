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
    val isConnectedToNetwork: Boolean
        get() = this.availableNetworkIds.size > 0
    private val availableNetworkIds = mutableSetOf<String>()

    private val connectivityManager = context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager

    private val networkRequest = NetworkRequest.Builder()
            .addTransportType(NetworkCapabilities.TRANSPORT_WIFI)
            .addTransportType(NetworkCapabilities.TRANSPORT_CELLULAR)
            .addTransportType(NetworkCapabilities.TRANSPORT_VPN).build()!!

    fun registerNetworkCallback() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering registerNetworkCallback")
        connectivityManager.registerNetworkCallback(networkRequest, this)
    }

    fun deregisterNetworkCallback() {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering deregisterNetworkCallback")
        connectivityManager.unregisterNetworkCallback(this)
    }

    override fun onAvailable(network: Network) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onAvailable")
        availableNetworkIds.add(network.toString())
    }

    override fun onLost(network: Network) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering onLost")
        availableNetworkIds.remove(network.toString())
    }
}
