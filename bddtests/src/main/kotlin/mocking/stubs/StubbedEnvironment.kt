package mocking.stubs
import mocking.MockingClient

class StubbedEnvironment {
    companion object {
        const val TIMEOUT_DELAY: Long = 71
    }

    fun generateStubs()
    {
        val mockingClient = MockingClient.instance
        mockingClient.clearWiremock()
        mockingClient.favicon()
        EmisStubbedEnvironment(mockingClient).generateStubs()
        TppStubbedEnvironment(mockingClient).generateStubs()
    }
}
