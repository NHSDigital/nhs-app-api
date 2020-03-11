package com.nhs.online.nhsonline.network

import android.content.Context
import android.net.ConnectivityManager
import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import com.nhs.online.nhsonline.utils.SdkVersionHelper
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class ConnectionStateMonitorTests {

    private val connectionStateMonitorMock = MockConnectionStateMonitor()

    @Test
    fun connectionStateMonitorWithConnectedContext() {
        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithDisconnectedContext() {
        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true for a connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithConnectedThenDisconnectedContext() {
        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }

        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true for a connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithConnectedThenDisconnectedThenConnectedContext() {
        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }

        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true for a connected network"
        }

        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithConnectedThenDisconnectedThenConnectedContext_PreSDK_23() {
        SdkVersionHelper.setSdkVersion(22)

        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }

        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockDisconnectedContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true for a connected network"
        }

        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockConnectedContext())
        assert(isConnectedToNetwork) {
            "Failed: Returns false for a connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithNullConnectionContext() {
        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockNullConnectionContext())
        assert(!isConnectedToNetwork) {
            "Failed: Returns true with no connected network"
        }
    }

    @Test
    fun connectionStateMonitorWithNullConnectionContext_PreSDK_23() {
        SdkVersionHelper.setSdkVersion(22)

        connectionStateMonitorMock.mockNetworkCallback(ResourceMockingClass().mockNullConnectionContext())
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