package features.prescriptionsSubmission.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import constants.Supplier
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.courses.stepDefinitions.CoursesStepDefinitions
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import features.prescriptions.PrescriptionSerenityHelpers
import mocking.stubs.prescriptions.factories.PrescriptionsFactory
import features.prescriptions.mappers.EmisPrescriptionMapper
import features.prescriptions.mappers.MicrotestPrescriptionMapper
import features.prescriptions.mappers.TppPrescriptionMapper
import features.prescriptions.mappers.VisionPrescriptionMapper
import features.prescriptions.stepDefinitions.PrescriptionsSerenityHelpers
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.defaults.VisionMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.microtest.prescriptions.PrescriptionHistoryGetResponse
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import mocking.stubs.pds.ViewSpinePdsStubs
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.vision.models.PrescriptionHistory
import mockingFacade.prescriptions.PartialSuccessFacade
import models.Patient
import models.prescriptions.PrescriptionLoaderConfiguration
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.ErrorPage
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.PartiallySuccessfulRepeatPrescriptionsOrderPage
import pages.prescription.PrescriptionsPage
import pages.text
import utils.LinkedProfilesSerenityHelpers
import utils.ProxySerenityHelpers
import utils.SerenityHelpers
import utils.assertTrueWithRetry
import utils.getOrFail
import utils.getOrNull
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

    lateinit var prescriptions : PrescriptionsPage

    private val mockingClient = MockingClient.instance

    var prescriptionSubmissionRequest: PrescriptionSubmissionRequest? = null

    lateinit var prescriptionLoader: IPrescriptionLoader<*>

    lateinit var scenarioTitle: String

    var currentScenarioState: String = Scenario.STARTED

    private lateinit var prescriptionPage: PrescriptionsPage
    private lateinit var confirmRepeatPrescriptionsOrderPage: ConfirmRepeatPrescriptionsOrderPage
    private lateinit var partiallySuccessfulPrescriptionsOrderPage: PartiallySuccessfulRepeatPrescriptionsOrderPage
    private lateinit var errorPage: ErrorPage

    private var initialHistoricPrescriptionsCount = 0

    val statusSubmitted = "Submitted"

    @Given("^I have an empty repeat prescription request")
    fun iHaveAnEmptyRepeatPrescriptionRequest() {
        prescriptionSubmissionRequest = null
    }

    @Given("^I have a repeat prescription request with (\\d+) courses")
    fun iHaveARepeatPrescriptionRequestWithXCourses(numOfCourses: Int) {
        SerenityHelpers.setPatient(Patient.getDefault(Supplier.EMIS))

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

    @Given("^the GP system responds with an error indicating the order was partially successful")
    fun theGpSystemRespondsWithErrorIndicatingTheOrderWasPartiallySuccessful() {
        val gpSystem = SerenityHelpers.getGpSupplier()

        val successfulMedicationOrders = listOf("Amoxicillin", "Ibuprofen")
        val unsuccessfulMedicationOrders = listOf("Tramadol", "Oxycodone", "Xanax")

        val partialSuccessData = PartialSuccessFacade(
                unsuccessfulMedications = unsuccessfulMedicationOrders,
                successfulMedications = successfulMedicationOrders
        )

        PrescriptionsFactory.getForSupplier(gpSystem)
                .prescriptionsOrderEndpointPartiallySuccessful(partialSuccessData)

        PrescriptionSerenityHelpers.PARTIAL_SUCCESS_RESULT.set(partialSuccessData)
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

    private fun prescriptionSubmissionWireMockAndDataSetup(amount: Int, gpSystem: Supplier) {
        coursesStepDefinitions.thereAreXXRepeatablePrescriptionsAvailable(amount)
        coursesStepDefinitions.iSelectXRepeatablePrescriptions(amount)

        currentScenarioState = PrescriptionsFactory.getForSupplier(gpSystem)
                .setupWireMockAndDataSetup(
                        scenarioTitle,
                        currentScenarioState,
                        statusSubmitted,
                        initialHistoricPrescriptionsCount,
                        amount)

        mockingClient.forAzure.forSearchOrganisation {
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
        val supplier = Supplier.valueOf(gpSystem)
        SerenityHelpers.setGpSupplier(supplier)

        if (SerenityHelpers.getPatientOrNull() == null) {
           SerenityHelpers.setPatient(Patient.getDefault(supplier))
        }
        val currentPatient = SerenityHelpers.getPatient()
        CitizenIdSessionCreateJourney().createFor(currentPatient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(currentPatient)
        PrescriptionsSerenityHelpers.PROVIDER.set(supplier)
        prescriptionLoader = PrescriptionsFactory.getForSupplier(supplier).getPrescriptionsLoader
        val emisPrescriptionMap = mutableMapOf<String, PrescriptionRequestsGetResponse>()
        Serenity.setSessionVariable("EmisPrescriptionsMap").to(emisPrescriptionMap)
        val microtestPrescriptionMap = mutableMapOf<String, PrescriptionHistoryGetResponse>()
        Serenity.setSessionVariable("MicrotestPrescriptionsMap").to(microtestPrescriptionMap)
    }

    @Given("^I have (\\d+) historic prescriptions in this scenario$")
    fun iHaveXHistoricPrescriptionsInThisScenario(amount: Int) {
        ViewSpinePdsStubs(mockingClient).generateSpineStubs()
        initialHistoricPrescriptionsCount = amount
        val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
                noPrescriptions = amount,
                noCourses = amount,
                noRepeats = amount
        )
        prescriptionLoader.loadData(prescriptionLoaderConfig)
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()

        val currentPatient = ProxySerenityHelpers.getPatientOrProxy()
        when (currentProvider) {
            Supplier.EMIS -> {
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
            Supplier.TPP -> {
                mockingClient.forTpp {
                    prescriptions.listRepeatMedication(currentPatient)
                            .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply)
                }
            }
            Supplier.VISION -> {
                mockingClient.forVision {
                    prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults.visionUserSession)
                            .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                }
            }
            Supplier.MICROTEST -> {
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
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .prescriptions.postPrescriptionsConnection(patientId, prescriptionSubmissionRequest)
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

    @But("GP system responds with a conflict error when a repeat prescription is submitted")
    fun gpSystemRespondsWithConflictErrorWhenARepeatPrescriptionIsSubmitted() {
        PrescriptionsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .orderPrescriptionReturnsConflictResponse()
    }

    @Then("I see a order successful message on the Repeat prescription page with (\\d+) prescriptions")
    fun iSeeAOrderSuccessfulMessageOnTheRequestPrescriptionPageWithXPrescriptions(amount: Int) {
        assertTrueWithRetry(prescriptionPage.isOrderSuccessfullTextVisible(),
                "Expected order success text to be visible")
        assertAmountOfPrescriptionsIsCorrect(amount)
    }

    @Then("I see the Repeat prescription page with (\\d+) prescriptions")
    fun iSeeNumberOfPrescriptions(amount: Int) {
        assertAmountOfPrescriptionsIsCorrect(amount)
    }
    private fun assertAmountOfPrescriptionsIsCorrect(amount: Int) {
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()

        when (currentProvider) {
            Supplier.TPP -> {
                prescriptionPage.assertPrescriptionsMatch(
                        TppPrescriptionMapper.map(prescriptionLoader.data as ListRepeatMedicationReply),
                        amount,
                        false)
            }
            Supplier.EMIS -> {
                val map =
                        Serenity.sessionVariableCalled<MutableMap<String,
                                PrescriptionRequestsGetResponse>>("EmisPrescriptionsMap")

                prescriptionPage.assertPrescriptionsMatch(EmisPrescriptionMapper.map(
                        map[currentScenarioState]!!), amount)
            }
            Supplier.VISION -> {
                prescriptionPage.assertPrescriptionsMatch(VisionPrescriptionMapper
                        .map(prescriptionLoader.data as PrescriptionHistory), amount)
            }
            Supplier.MICROTEST -> {
                val map =
                        Serenity.sessionVariableCalled<MutableMap<String,
                                PrescriptionHistoryGetResponse>>("MicrotestPrescriptionsMap")

                prescriptionPage.assertPrescriptionsMatch(MicrotestPrescriptionMapper.map(
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

    @Then("I can view which medications from my prescription order succeeded and failed")
    fun iSeeAMessageIndicatingThePrescriptionWasPartiallySuccessful() {
        partiallySuccessfulPrescriptionsOrderPage.shouldBeDisplayed()

        val partialSuccessExpected = PrescriptionSerenityHelpers
                .PARTIAL_SUCCESS_RESULT
                .getOrFail<PartialSuccessFacade>()
        partiallySuccessfulPrescriptionsOrderPage.verifyMedications(partialSuccessExpected)
    }

    @When("I see nominated pharmacy information is shown and correct")
    fun iSeeNominatedPharmacyInformationIsCorrect() {

        val component = confirmRepeatPrescriptionsOrderPage.pharmacyDetailComponent

        val myNominatedPharmacy =
                NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.getOrFail<NhsAzureSearchOrganisationItem>()

        Assert.assertEquals(
                "Nominated Pharmacy name is not correct",
                myNominatedPharmacy.OrganisationName, component.pharmacyName.text)

        Assert.assertEquals(
                    "Address Line 1 is not correct",
                myNominatedPharmacy.Address1, component.pharmacyAddressLine1.text)


        Assert.assertEquals(
                    "Address Line 2 is not correct",
                myNominatedPharmacy.Address2, component.pharmacyAddressLine2.text)

        Assert.assertEquals(
                    "Address Line 3 is not correct",
                myNominatedPharmacy.Address3, component.pharmacyAddressLine3.text)

        Assert.assertEquals(
                    "City is not correct",
                myNominatedPharmacy.City, component.pharmacyCity.text)

        Assert.assertEquals(
                    "County is not correct",
                myNominatedPharmacy.County, component.pharmacyCounty.text)

        Assert.assertEquals(
                    "Postcode is not correct",
                myNominatedPharmacy.Postcode, component.pharmacyPostcode.text)

        if (myNominatedPharmacy.primaryPhone() != null) {
            Assert.assertEquals(
                    "Nominated Pharmacy phone number is not correct",
                    "Telephone: " + myNominatedPharmacy.primaryPhone(), component.pharmacyPhoneNumber.text)
        }
    }

    @Then("^I cannot see any nominated pharmacy information$")
    fun iCannotSeeNominatedPharmacyInformation() {
        Assert.assertNotNull(
                "PharmacyDetailComponent is null",
                confirmRepeatPrescriptionsOrderPage.pharmacyDetailComponent)

        Assert.assertFalse(
                "PharmacyDetailComponent is not visible",
                confirmRepeatPrescriptionsOrderPage.pharmacyDetailComponent.isVisible())
    }
}
