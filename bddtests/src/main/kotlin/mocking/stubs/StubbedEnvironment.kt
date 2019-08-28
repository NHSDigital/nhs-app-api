package mocking.stubs
import mocking.MockingClient

const val NUMBER_OF_PRESCRIPTIONS = 5
const val NUMBER_OF_COURSES = 5
const val NUMBER_OF_REPEAT_PRESCRIPTIONS = 5

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
