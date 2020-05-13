package mocking.stubs

import mocking.MockingClient
import models.Patient
import net.serenitybdd.core.Serenity
import utils.GlobalSerenityHelpers
import utils.set

class StubbedEnvironment {
    companion object {
        const val TIMEOUT_DELAY: Long = 71

        fun getPatientList(): List<Patient> =
                EmisStubsPatientFactory.EMISPatientList + TppStubsPatientFactory.TppPatientList
    }

    fun generateStubs()
    {
        val mockingClient = MockingClient.instance
        mockingClient.wiremockHelper.clearWiremock()
        mockingClient.favicon()

        resetSerenityVariables()
        EmisStubbedEnvironment(mockingClient).generateStubs()

        resetSerenityVariables()
        TppStubbedEnvironment(mockingClient).generateStubs()
    }

    private fun resetSerenityVariables() {
        Serenity.getCurrentSession().clear()
        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(false)
    }
}
