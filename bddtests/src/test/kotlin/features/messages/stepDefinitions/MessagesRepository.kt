package features.messages.stepDefinitions

import mongodb.MongoDBConnection
import mongodb.MongoRepositoryMessage
import org.junit.Assert
import utils.getOrFail
import worker.models.messages.MessageRequest

class MessagesRepository {
    companion object {
        fun assertSingleMessageInRepository(expectedMessage: MessageRequest, read: Boolean) {
            MongoDBConnection.MessagesCollection.assertNumberOfDocuments(1)
            val messages = MongoDBConnection.MessagesCollection
                .getValues<MongoRepositoryMessage>(MongoRepositoryMessage::class.java)
            Assert.assertNotNull("Messages", messages)
            Assert.assertEquals("Number of Messages", 1, messages.count())
            val message = messages.first()
            Assert.assertNotNull("Message id", message)
            val expectedNhsLoginId = MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.getOrFail<String>()
            Assert.assertEquals("Message nhsLoginId", expectedNhsLoginId, message.NhsLoginId)
            Assert.assertEquals("Message body", expectedMessage.body, message.Body)
            Assert.assertEquals("Message version", expectedMessage.version, message.Version)
            if (read) {
                Assert.assertNotNull("Message read", message.ReadTime)
            } else {
                Assert.assertNull("Message read", message.ReadTime)
            }

            if (expectedMessage.senderContext != null) {
                Assert.assertNotNull("Message sender context", message.SenderContext)

                val expectedSenderContext = expectedMessage.senderContext!!
                val actualSenderContext = message.SenderContext!!

                Assert.assertEquals(
                    "Message sender context supplier Id",
                    expectedSenderContext.supplierId,
                    actualSenderContext.SupplierId
                )
                Assert.assertEquals(
                    "Message sender context communication Id",
                    expectedSenderContext.communicationId,
                    actualSenderContext.CommunicationId
                )
                Assert.assertEquals(
                    "Message sender context transmission Id",
                    expectedSenderContext.transmissionId,
                    actualSenderContext.TransmissionId
                )
                Assert.assertEquals(
                    "Message sender context request reference",
                    expectedSenderContext.requestReference,
                    actualSenderContext.RequestReference
                )
                Assert.assertEquals(
                    "Message sender context communication created date time",
                    expectedSenderContext.communicationCreatedDateTime,
                    actualSenderContext.CommunicationCreatedDateTime
                )
                Assert.assertEquals(
                    "Message sender context campaign Id",
                    expectedSenderContext.campaignId,
                    actualSenderContext.CampaignId
                )
                Assert.assertEquals(
                    "Message sender context ODS Code",
                    expectedSenderContext.odsCode,
                    actualSenderContext.OdsCode
                )
                Assert.assertEquals(
                    "Message sender context Nhs Number",
                    expectedSenderContext.nhsNumber,
                    actualSenderContext.NhsNumber
                )
                Assert.assertEquals(
                    "Message sender context Nhs Login Id",
                    expectedSenderContext.nhsLoginId,
                    actualSenderContext.NhsLoginId
                )
            }
        }
    }
}
