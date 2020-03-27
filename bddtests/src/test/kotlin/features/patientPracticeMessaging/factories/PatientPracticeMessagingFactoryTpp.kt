package features.patientPracticeMessaging.factories

import constants.ErrorResponseCodeTpp
import constants.TppConstants
import mocking.data.TppListServiceAccessesData
import mocking.data.patientPracticeMessaging.TppMessagingData
import mocking.emis.models.PatientPracticeMessagingSerenityHelpers
import mocking.tpp.models.Error
import mocking.tpp.models.Message
import mocking.tpp.models.MessagesViewReply
import models.ExpectedMessage
import models.Patient
import utils.SerenityHelpers
import utils.getOrNull
import worker.models.patientPracticeMessaging.CreateMessageRequest

class PatientPracticeMessagingFactoryTpp: PracticePatientMessagingFactory() {
    override fun disabled(patient: Patient) {
        mockingClient.forTpp {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                            "Im1 messaging access is disabled by the practice",
                                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    override fun enabled(patient: Patient){
        mockingClient.forTpp {
            val listServicesAccessReply = TppListServiceAccessesData()
                    .enableService(TppConstants.Im1MessagingListServiceAccessDescription,
                                   TppConstants.Im1MessagingListServiceAccessCode,
                                   TppConstants.ListServiceAccessesAvailableStatus,
                                   TppConstants.ListServiceAccessesAvailableStatusDescription)
            listServiceAccesses(SerenityHelpers.getPatient())
                    .respondWithSuccess(listServicesAccessReply)
        }
    }

    override fun enabledWithPatientPracticeMessaging(patient: Patient,
                                                     hasUnread: Boolean) {
        mockingClient.forTpp {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppMessagingData.getDefaultTppMessages(hasUnread))
        }
        val expectedMessages = getExpectedMessages(TppMessagingData.getDefaultTppMessages().Message.toList(),
                                                           hasUnread)
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES,
                expectedMessages)
    }

    override fun unknownErrorWithPatientPracticeMessaging(patient: Patient) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun patientHasNoMessages(patient: Patient) {
        mockingClient.forTpp{
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithSuccess(
                            MessagesViewReply( Message = mutableListOf())
                    )
        }
    }

    private fun getExpectedMessages(messages: List<Message>, hasUnread: Boolean =
                                            false):
            List<ExpectedMessage> {
        var unreadCount = 0
        if (hasUnread) {
            unreadCount = 1
        }
        val expectedInboxMessageDates = PatientPracticeMessagingSerenityHelpers
                .EXPECTED_INBOX_MESSAGE_DATES
                .getOrNull<List<String>>()
        return messages.mapIndexed(fun(index, message): ExpectedMessage {
            return ExpectedMessage(
                    message.messageId,
                    message.conversationId,
                    messageText = message.messageText,
                    lastMessageDateTime = expectedInboxMessageDates!![index],
                    sender = message.sender,
                    recipient = message.recipient,
                    hasUnreadReplies = hasUnread,
                    unreadCount = unreadCount)
        })
    }

    override fun forbiddenErrorWithPatientPracticeMessaging(patient: Patient) {
        mockingClient.forTpp {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                            "Im1 messaging access is disabled by the practice",
                                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }


    override fun errorWithPatientPracticeMessagingMessageDetails(patient: Patient) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun errorWithPatientPracticeMessagingConversationDelete(patient: Patient) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun patientSuccessfullySendsAMessage(patient: Patient,
                                                  createMessageRequest: CreateMessageRequest) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun errorSendingAMessage(patient: Patient,
                                      createMessageRequest: CreateMessageRequest) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun noRecipients(patient: Patient) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

}