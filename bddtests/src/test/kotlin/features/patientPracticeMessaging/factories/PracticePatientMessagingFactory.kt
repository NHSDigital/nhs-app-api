package features.patientPracticeMessaging.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import mocking.emis.patientPracticeMessaging.PatientMessageSummary
import models.ExpectedMessage
import models.Patient
import worker.models.patientPracticeMessaging.CreateMessageRequest

abstract class PracticePatientMessagingFactory {

    abstract fun disabled(patient: Patient)
    abstract fun enabledWithPatientPracticeMessaging(patient: Patient, hasUnread: Boolean)
    abstract fun forbiddenErrorWithPatientPracticeMessaging(patient: Patient)
    abstract fun unknownErrorWithPatientPracticeMessaging(patient: Patient)
    abstract fun errorWithPatientPracticeMessagingMessageDetails(patient: Patient)
    abstract fun getExpectedMessages(expectedMessages: List<PatientMessageSummary>): List<ExpectedMessage>
    abstract fun patientHasNoMessages(patient: Patient)
    abstract fun patientSuccessfullySendsAMessage(patient: Patient, createMessageRequest: CreateMessageRequest)
    abstract fun errorSendingAMessage(patient: Patient, createMessageRequest: CreateMessageRequest)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<PracticePatientMessagingFactoryEmis>() {

        override val map: HashMap<Supplier, (() -> PracticePatientMessagingFactoryEmis)>
                by lazy {
                    hashMapOf(Supplier.EMIS to { PracticePatientMessagingFactoryEmis() })
                }

    }
}
