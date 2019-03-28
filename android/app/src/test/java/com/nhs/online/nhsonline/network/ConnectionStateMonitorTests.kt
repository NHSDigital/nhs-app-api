package com.nhs.online.nhsonline.network

import android.content.Context
import android.net.*
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhaarman.mockito_kotlin.*
@RunWith(RobolectricTestRunner::class)
class ConnectionStateMonitorTests {

    private val mockConnectionStateMonitor = MockConnectionStateMonitor()

    @Test
    fun connectionStateMonitorWithConnectedContext() {
        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithDisconnectedContext() {
        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true for a connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithConnectedThenDisconnectedContext() {
        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }

        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true for a connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithConnectedThenDisconnectedThenConnectedContext() {
        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }

        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true for a connected network"
        }

        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }
    }
}

class MockConnectionStateMonitor {

    fun mockNetworkCallback(context: Context) {
        val connectionStateMonitor = ConnectionStateMonitor(context)
        val connectivityManager = context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager

        when(connectivityManager.activeNetworkInfo.isConnected) {
            true -> connectionStateMonitor.onAvailable(mock())
            false -> connectionStateMonitor.onLost(mock())
        }
    }
}