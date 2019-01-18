package com.nhs.online.nhsonline.network

import com.nhs.online.nhsonline.resources.ResourceMockingClass
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)
class ReachabilityTest {

    @Test
    fun reachabilityWithConnectedContext() {
        val connectedContext = ResourceMockingClass().mockConnectedContext()

        assert(Reachability.isConnectedToNetwork(connectedContext)) {
            "Reachability - Failed: Returns false for a connected network"
        }
    }


    @Test
    fun reachabilityWithDisconnectedContext() {
        val connectedContext = ResourceMockingClass().mockDisconnectedContext()

        assert(!Reachability.isConnectedToNetwork(connectedContext)) {
            "Reachability - Failed: Returns true for a disconnected network"
        }
    }
}