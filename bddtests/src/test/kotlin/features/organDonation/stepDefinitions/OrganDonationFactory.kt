package features.organDonation.stepDefinitions

import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.stubs.StubbedEnvironment
import models.Patient
import utils.SerenityHelpers
import java.time.Duration

class OrganDonationFactory {

    val mockingClient = MockingClient.instance

    fun registeredUser(gpSystem: String){
        val patient = setupPatient(gpSystem)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun unregisteredUser(gpSystem: String) {
        val patient = setupPatient(gpSystem)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithNotFoundError()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun conflict(gpSystem: String) {
        val patient = setupPatient(gpSystem)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithConflictError()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun organDonationTimeout(gpSystem: String) {
        val patient = setupPatient(gpSystem)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithTimeoutError()
                    .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun demographicsTimeout(gpSystem: String) {
        val patient = setupPatient(gpSystem)
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabledButTimesOut(patient)
    }

    private fun setupPatient(gpSystem: String) : Patient{
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)
        return patient
    }
}
