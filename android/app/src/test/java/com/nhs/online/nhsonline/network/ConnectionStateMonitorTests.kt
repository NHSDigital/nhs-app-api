package com.nhs.online.nhsonline.network

import android.net.Network
import com.nhaarman.mockitokotlin2.mock
import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.After
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class ConnectionStateMonitorTests {

    private var systemUnderTest: ConnectionStateMonitor = ConnectionStateMonitor(
            ResourceMockingClass().mockContext())
    private var network: Network = mock()

    @After
    fun teardown() {
        systemUnderTest.onLost(network)
    }

    @Test
    fun connectionStateMonitor_IsConnectedToNetworkIsTrue_WhenNetworkAvailable() {
        systemUnderTest.onAvailable(network)
        assert(systemUnderTest.isConnectedToNetwork) {
            "Failed: Expected isConnectedToNetwork to be true but was false"
        }
    }

    @Test
    fun connectionStateMonitor_IsConnectedToNetworkIsFalse_WhenNoNetworkAvailable() {
        assert(!systemUnderTest.isConnectedToNetwork) {
            "Failed: Expected isConnectedToNetwork to be false but was true"
        }
    }

    @Test
    fun connectionStateMonitor_IsConnectedIsTrueThenFalse_WhenAvailableNetworkIsLost() {
        systemUnderTest.onAvailable(network)
        assert(systemUnderTest.isConnectedToNetwork) {
            "Failed: Expected isConnectedToNetwork to be true but was false"
        }

        systemUnderTest.onLost(network)
        assert(!systemUnderTest.isConnectedToNetwork) {
            "Failed: Expected isConnectedToNetwork to be false but was true"
        }
    }

    @Test
    fun connectionStateMonitor_IsConnectedIsTrueThenFalseThenTrue_WhenAvailableNetworkIsLostAndAvailableAgain() {
        systemUnderTest.onAvailable(network)
        assert(systemUnderTest.isConnectedToNetwork) {
            "Failed: Expected isConnectedToNetwork to be true but was false"
        }

        systemUnderTest.onLost(network)
        assert(!systemUnderTest.isConnectedToNetwork) {
            "Failed: Expected isConnectedToNetwork to be false but was true"
        }

        systemUnderTest.onAvailable(network)
        assert(systemUnderTest.isConnectedToNetwork) {
            "Failed: Expected isConnectedToNetwork to be true but was false"
        }
    }
}
