package mocking.stubs
import mocking.MockingClient

class StubbedEnvironment {
    companion object {
        const val TIMEOUT_DELAY: Long = 71
    }
    fun generateStubs()
    {
        EmisStubbedEnvironment(MockingClient.instance).generateStubs()
        TppStubbedEnvironment(MockingClient.instance).generateStubs()
    }
}
