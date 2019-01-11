package features.organDonation.stepDefinitions

import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.data.organDonation.OrganDonationReferenceData
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.stubs.StubbedEnvironment
import models.Patient
import utils.SerenityHelpers
import java.time.Duration

class OrganDonationFactory(val gpSystem: String) {

    val mockingClient = MockingClient.instance

    val patient = setupPatient()

    fun setupPatientForAppUse() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        mockingClient.forOrganDonation {
            referenceData().respondWithSuccess(OrganDonationReferenceData.getOrganDonationReferenceData())
        }
    }

    fun registeredUser() {
        val patient = setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun unregisteredUser(patient: Patient? = null) {
        val patientToUse = patient ?: setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patientToUse).respondWithNotFoundError()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patientToUse)
    }

    fun conflict() {
        val patient = setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithConflictError()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun organDonationTimeout() {
        val patient = setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithTimeoutError()
                    .delayedBy(Duration.ofSeconds(StubbedEnvironment.TIMEOUT_DELAY))
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun organDonationInternalError() {
        val patient = setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithInternalError()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabled(patient)
    }

    fun demographicsTimeout() {
        val patient = setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess()
        }
        DemographicsFactory.getForSupplier(gpSystem).enabledButTimesOut(patient)
    }

    private fun setupPatient(): Patient {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)
        return patient
    }

    fun demographicsInternalError() {
        val patient = setupPatient()
        mockingClient.forOrganDonation {
            lookupOrganDonationRegistration(patient).respondWithSuccess()
        }
        DemographicsFactory.getForSupplier(gpSystem).throwInternalError(patient)
    }
}
