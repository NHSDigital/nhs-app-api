package features.prescriptionsSubmission.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.courses.stepDefinitions.CoursesStepDefinitions
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import features.prescriptions.factories.PrescriptionsFactory
import features.prescriptions.mappers.EmisPrescriptionMapper
import features.prescriptions.mappers.MicrotestPrescriptionMapper
import features.prescriptions.mappers.TppPrescriptionMapper
import features.prescriptions.mappers.VisionPrescriptionMapper
import features.prescriptions.stepDefinitions.PrescriptionsSerenityHelpers
import features.prescriptions.stepDefinitions.ProviderTypes
import features.prescriptions.steps.PrescriptionsSteps
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.defaults.VisionMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.microtest.prescriptions.PrescriptionHistoryGetResponse
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import mocking.stubs.pds.ViewSpinePdsStubs
import mocking.vision.models.PrescriptionHistory
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.ErrorPage
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.PrescriptionsPage
import pages.text
import utils.SerenityHelpers
import utils.assertTrueWithRetry
import utils.getOrNull
import utils.getOrFail
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.Duration
import java.util.*

private const val WAIT_TIME_GREATER_THAN_THIRTY_SECS = 31L

open class PrescriptionsSubmissionStepDefinitions {

    @Steps
    lateinit var coursesStepDefinitions: CoursesStepDefinitions
    @Steps
    lateinit var prescriptionSteps: PrescriptionsSteps

    val mockingClient = MockingClient.instance

    var prescriptionSubmissionRequest: PrescriptionSubmissionRequest? = null

    lateinit var prescriptionLoader: IPrescriptionLoader<*>

    lateinit var scenarioTitle: String

    var currentScenarioState: String = Scenario.STARTED

    private lateinit var prescriptionPage: PrescriptionsPage
    private lateinit var confirmRepeatPrescriptionsOrderPage : ConfirmRepeatPrescriptionsOrderPage
    private lateinit var errorPage: ErrorPage

    private var initialHistoricPrescriptionsCount = 0

    val statusSubmitted = "Submitted"

    @Given("^I have an empty repeat prescription request")
    fun iHaveAnEmptyRepeatPrescriptionRequest() {
        prescriptionSubmissionRequest = null
    }

    @Given("^I have a repeat prescription request with (\\d+) courses")
    fun iHaveARepeatPrescriptionRequestWithXCourses(numOfCourses: Int) {
        SerenityHelpers.setPatient(Patient.getDefault("EMIS"))

        val uuids: MutableList<String> = mutableListOf()

        for (i in 0 until numOfCourses) {
            uuids.add(UUID.randomUUID().toString())
        }

        prescriptionSubmissionRequest = PrescriptionSubmissionRequest(uuids, "")
    }

    @Given("^(\\d+) invalid courses")
    fun xInvalidCourses(numOfCourses: Int) {
        val uuids: MutableList<String> = mutableListOf()

        for (i in 0 until numOfCourses) {
            uuids.add("invalidCourse-$i")
        }

        prescriptionSubmissionRequest!!.courseIds.addAll(uuids)
    }

    @Given("^EMIS responds with an error indicating an included course has already been ordered in the last 30 days " +
            "when submitting the repeat prescription")
    fun emisRespondsWithErrorIndicatingAnIncludedCourseHasAlreadyBeenOrderedWhenSubmittingRepeatPrescription() {
        val currentPatient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithAlreadyAPendingRequestInTheLast30Days()
        }
    }

    @Given("^Emis responds with an error indicating a course is invalid")
    fun emisRespondsWithAnErrorIndicatingACourseIsInvalid() {
        val currentPatient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithBadRequestErrorIndicatingACourseIsInvalid()
        }
    }

    @Given("^EMIS responds with a Created success code when submitting the repeat prescription")
    fun emisRespondsWithACreatedSuccessCodeWhenSubmittingRepeatPrescription() {
        val currentPatient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithCreated()
        }
    }

    @Given("^EMIS responds with an error indicating prescriptions is not enabled when submitting the repeat " +
            "prescription")
    fun emisRespondsWithAnErrorIndicatingPrescriptionsIsNotEnabled() {
        val currentPatient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithPrescriptionsNotEnabled()
        }
    }

    @Given("^EMIS takes longer than 30 seconds to respond when a repeat prescription is submitted")
    fun emisTakesTooLongToRespondWhenARepeatPrescriptionIsSubmitted() {
        val currentPatient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithCreated().delayedBy(Duration.ofSeconds(WAIT_TIME_GREATER_THAN_THIRTY_SECS))
        }
    }

    @Given("^I select (\\d+) repeatable prescriptions to order$")
    fun iSelectXRepeatablePrescriptionsToOrder(amount: Int) {
        prescriptionSubmissionWireMockAndDataSetup(amount, SerenityHelpers.getGpSupplier())
    }

    private fun prescriptionSubmissionWireMockAndDataSetup(amount: Int, gpSystem: String) {
        coursesStepDefinitions.thereAreXXRepeatablePrescriptionsAvailable(amount)
        coursesStepDefinitions.iSelectXRepeatablePrescriptions(amount)
        currentScenarioState = PrescriptionsFactory.getForSupplier(gpSystem)
                .setupWireMockAndDataSetup(
                        scenarioTitle,
                        currentScenarioState,
                        statusSubmitted,
                        initialHistoricPrescriptionsCount,
                        amount)

        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(null
            ).respondWithSuccess(NhsAzureSearchData.getOrganisationWithinRange(true))
        }
    }

        @Given("^the scenario is (.*)$")
        fun theScenarioIsX(title: String) {
            scenarioTitle = title
        }


        @Given("^I am using (.*) GP System to submit my prescription$")
        fun givenIHaveXPastRepeatPrescriptions(gpSystem: String) {
            SerenityHelpers.setGpSupplier(gpSystem)
            val currentPatient = Patient.getDefault(gpSystem)
            SerenityHelpers.setPatient(currentPatient)
            CitizenIdSessionCreateJourney(mockingClient).createFor(currentPatient)
            SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(currentPatient)
            PrescriptionsSerenityHelpers.PROVIDER.set(ProviderTypes.valueOf(gpSystem))
            prescriptionLoader = PrescriptionsFactory.getForSupplier(gpSystem).getPrescriptionsLoader
            val emisPrescriptionMap = mutableMapOf<String, PrescriptionRequestsGetResponse>()
            Serenity.setSessionVariable("EmisPrescriptionsMap").to(emisPrescriptionMap)
            val microtestPrescriptionMap = mutableMapOf<String, PrescriptionHistoryGetResponse>()
            Serenity.setSessionVariable("MicrotestPrescriptionsMap").to(microtestPrescriptionMap)
        }

        @Given("^I have (\\d+) historic prescriptions in this scenario$")
        fun iHaveXHistoricPrescriptionsInThisScenario(amount: Int) {
            ViewSpinePdsStubs(mockingClient).generateSpineStubs()
            initialHistoricPrescriptionsCount = amount
            prescriptionLoader.loadData(amount, amount, amount)
            val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<ProviderTypes>()

            val currentPatient = SerenityHelpers.getPatient()
            when (currentProvider) {
                ProviderTypes.EMIS -> {
                    val data = prescriptionLoader.data as PrescriptionRequestsGetResponse
                    mockingClient.forEmis {
                        prescriptions.prescriptionsRequest(currentPatient)
                                .respondWithSuccess(data)
                                .inScenario(scenarioTitle)
                                .whenScenarioStateIs(currentScenarioState)
                    }

                    val map =
                            Serenity.sessionVariableCalled<MutableMap<String,
                                    PrescriptionRequestsGetResponse>>("EmisPrescriptionsMap")
                    map[Scenario.STARTED] = data
                }
                ProviderTypes.TPP -> {
                    mockingClient.forTpp {
                        prescriptions.listRepeatMedication(currentPatient)
                                .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply)
                    }
                }
                ProviderTypes.VISION -> {
                    mockingClient.forVision {
                        prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults.visionUserSession)
                                .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                    }
                }
                ProviderTypes.MICROTEST -> {
                    val data = prescriptionLoader.data as PrescriptionHistoryGetResponse
                    mockingClient.forMicrotest {
                        prescriptions.getPrescriptionHistoryRequest(currentPatient)
                                .respondWithSuccess(data)
                                .inScenario(scenarioTitle)
                                .whenScenarioStateIs(currentScenarioState)
                    }

                    val map =
                            Serenity.sessionVariableCalled<MutableMap<String,
                                    PrescriptionHistoryGetResponse>>("MicrotestPrescriptionsMap")
                    map[Scenario.STARTED] = data
                }

            }
        }

        @When("I submit the repeat prescription")
        fun whenISubmitTheRepeatPrescription() {
            try {
                val response = Serenity
                        .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .prescriptions.postPrescriptionsConnection(prescriptionSubmissionRequest)
                SerenityHelpers.setHttpResponse(response)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }

        @When("I click Confirm and order repeat prescription")
        fun iClickConfirmAndOrderRepeatPrescription() {
            confirmRepeatPrescriptionsOrderPage
                    .clickConfirmAndOrderRepeatPrescriptionButton()
        }

        @Then("EMIS responds with an unknown internal server error when a repeat prescription is submitted")
        fun emisRespondsWithAnUnknownInternalServerErrorWhenARepeatPrescriptionIsSubmitted() {
            val currentPatient = SerenityHelpers.getPatient()
            mockingClient.forEmis {
                prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                        .respondWithGenericInternalServerError()
            }
        }


        @Then("I see a order successful message on the Repeat prescription page with (\\d+) prescriptions")
        fun iSeeAOrderSuccessfulMessageOnTheRequestPrescriptionPageWithXPrescriptions(amount: Int) {
            assertTrueWithRetry(prescriptionPage.isOrderSuccessfullTextVisible(),
                    "Expected order success text to be visible")
            val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<ProviderTypes>()

            when (currentProvider) {
                ProviderTypes.TPP -> {
                    prescriptionSteps.assertPrescriptionsMatch(
                            TppPrescriptionMapper.map(prescriptionLoader.data as ListRepeatMedicationReply),
                            amount,
                            false)
                }
                ProviderTypes.EMIS -> {
                    val map =
                            Serenity.sessionVariableCalled<MutableMap<String,
                                    PrescriptionRequestsGetResponse>>("EmisPrescriptionsMap")

                    prescriptionSteps.assertPrescriptionsMatch(EmisPrescriptionMapper.map(
                            map[currentScenarioState]!!), amount)
                }
                ProviderTypes.VISION -> {
                    prescriptionSteps.assertPrescriptionsMatch(VisionPrescriptionMapper
                            .map(prescriptionLoader.data as PrescriptionHistory), amount)
                }
                ProviderTypes.MICROTEST -> {
                    val map =
                            Serenity.sessionVariableCalled<MutableMap<String,
                                    PrescriptionHistoryGetResponse>>("MicrotestPrescriptionsMap")

                    prescriptionSteps.assertPrescriptionsMatch(MicrotestPrescriptionMapper.map(
                            map[currentScenarioState]!!), amount)
                }
            }
        }

        @Then("I see a message indicating there was an error sending my order")
        fun iSeeAMessageOrderNotSuccessful() {
            Assert.assertTrue("Expected error to be visible",
                    confirmRepeatPrescriptionsOrderPage.errorSendingOrderErrorIsVisible())
        }

        @Then("I see a message indicating I've previously ordered " +
                "one of the selected medications within the last 30 days")
        fun iSeeAMessageIndicatingIvePreviouslyOrderedOneOfTheSelectedMedicationsWithinTheLast30days() {
            val expectedHeader = "We cannot complete this order"
            val expectedSubHeader = "You previously ordered at least one of these medications in the last 30 days."
            val expectedText = "If you need more medication sooner, contact your GP."
            Assert.assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.text}",
                    expectedHeader, errorPage.heading.text)
            Assert.assertEquals("expected error text $expectedSubHeader but found ${errorPage.subHeading.text}",
                    expectedSubHeader, errorPage.subHeading.text)
            Assert.assertEquals("expected error text $expectedText but found ${errorPage.errorText1.text}",
                    expectedText, errorPage.errorText1.text)
        }

        @When("I see nominated pharmacy information is shown and correct")
        fun iSeeNominatedPharmacyInformationIsCorrect() {

            val component = confirmRepeatPrescriptionsOrderPage.pharmacyDetailComponent
            Assert.assertTrue("PharmacyDetailComponent is not visible", component.isVisible())

            val actualPharmacyName= component.pharmacyName.text
            val actualPharmacyAddress= component.pharmacyAddress.text
            val actualPhoneNumber= component.pharmacyPhoneNumber.text

            val myNominatedPharmacy =
                    NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.getOrFail<NhsAzureSearchOrganisationItem>()

            Assert.assertEquals(
                    "Nominated Pharmacy name is not correct",
                    myNominatedPharmacy.OrganisationName, actualPharmacyName)

            Assert.assertEquals(
                    "Nominated Pharmacy address is not correct",
                    myNominatedPharmacy.addressFormatted(), actualPharmacyAddress)

            val expectedPhoneNumber = myNominatedPharmacy.primaryPhone()
            if (expectedPhoneNumber != null) {
                Assert.assertEquals(
                        "Nominated Pharmacy phone number is not correct",
                        expectedPhoneNumber, actualPhoneNumber)
            }
        }

        @Then("^I cannot see any nominated pharmacy information$")
        fun iCannotSeeNominatedPharmacyInformation() {
            Assert.assertNotNull(
                    "PharmacyDetailComponent is null",
                    confirmRepeatPrescriptionsOrderPage.pharmacyDetailComponent)

            Assert.assertFalse(
                    "PharmacyDetailComponent is visible",
                    confirmRepeatPrescriptionsOrderPage.pharmacyDetailComponent.isVisible())
        }
    }
