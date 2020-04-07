package mocking.stubs
import mocking.MockingClient
import models.Patient

class StubbedEnvironment {
    companion object {
        const val TIMEOUT_DELAY: Long = 71

        fun getPatientList(): List<Patient> =
                EmisStubsPatientFactory.EMISPatientList + TppStubsPatientFactory.TppPatientList
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
