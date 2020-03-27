package features.patientPracticeMessaging.factories

import constants.Supplier
import mocking.SupplierSpecificFactory
import mocking.MockingClient
import models.Patient
import worker.models.patientPracticeMessaging.CreateMessageRequest

abstract class PracticePatientMessagingFactory {

    abstract fun enabled(patient: Patient)
    abstract fun disabled(patient: Patient)
    abstract fun enabledWithPatientPracticeMessaging(patient: Patient, hasUnread: Boolean)
    abstract fun forbiddenErrorWithPatientPracticeMessaging(patient: Patient)
    abstract fun unknownErrorWithPatientPracticeMessaging(patient: Patient)
    abstract fun errorWithPatientPracticeMessagingMessageDetails(patient: Patient)
    abstract fun errorWithPatientPracticeMessagingConversationDelete(patient: Patient)
    abstract fun patientHasNoMessages(patient: Patient)
    abstract fun patientSuccessfullySendsAMessage(patient: Patient, createMessageRequest: CreateMessageRequest)
    abstract fun errorSendingAMessage(patient: Patient, createMessageRequest: CreateMessageRequest)
    abstract fun noRecipients(patient: Patient)

    val mockingClient = MockingClient.instance

    companion object : SupplierSpecificFactory<PracticePatientMessagingFactory>() {

        override val map: HashMap<Supplier, (() -> PracticePatientMessagingFactory)>
                by lazy {
                    hashMapOf(
                        Supplier.EMIS to { PracticePatientMessagingFactoryEmis() },
                        Supplier.TPP to { PatientPracticeMessagingFactoryTpp() }
                     )
                }

    }
}
