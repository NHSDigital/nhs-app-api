package com.nhs.online.nhsonline.network

import android.content.Context
import android.net.*
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhaarman.mockito_kotlin.*
import com.nhs.online.nhsonline.utils.SdkVersionHelper

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

    @Test
    fun connectionStateMonitorWithConnectedThenDisconnectedThenConnectedContext_PreSDK_23() {
        SdkVersionHelper.setSdkVersion(22)

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

    @Test
    fun connectionStateMonitorWithNullConnectionContext() {
        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockNullConnectionContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true with no connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithNullConnectionContext_PreSDK_23() {
        SdkVersionHelper.setSdkVersion(22)

        mockConnectionStateMonitor.mockNetworkCallback(ResourceMockingClass().mockNullConnectionContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true with no connected network"
        }
    }
}

class MockConnectionStateMonitor {

    fun mockNetworkCallback(context: Context) {
        val connectionStateMonitor = ConnectionStateMonitor(context)
        val connectivityManager = context.getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager

        if (connectivityManager.activeNetworkInfo != null) {
            when (connectivityManager.activeNetworkInfo.isConnected) {
                true -> connectionStateMonitor.onAvailable(mock())
                false -> connectionStateMonitor.onLost(mock())
            }
        } else {
            connectionStateMonitor.onLost(mock())
        }
    }
}