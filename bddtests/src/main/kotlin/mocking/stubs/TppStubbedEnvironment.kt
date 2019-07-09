package mocking.stubs

import mocking.MockingClient
import mocking.stubs.TppStubsPatientFactory.Companion.TppPatientList

class TppStubbedEnvironment(private val mockingClient: MockingClient){
    fun generateStubs() {
        PatientDataGenerator.generatePatientData(TppPatientList, "TPP")
    }
}
