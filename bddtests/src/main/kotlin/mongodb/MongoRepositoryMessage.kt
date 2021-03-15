package mongodb

import worker.models.messages.SingleMessageFacade
import java.time.ZonedDateTime

data class MongoRepositoryMessage(val NhsLoginId: String?,
                                  val Sender: String,
                                  val Version: Int,
                                  val Body: String,
                                  val ReadTime: String?,
                                  val SenderContext: MongoRepositoryMessageSenderContext?) {
    companion object {
        // We cannot serialise an object to create this because the ISODate objects cannot be created like that.
        fun createJson(message: SingleMessageFacade, nhsLoginId: String): String {
            val readString = if (!message.read) null else dateAsIsoDate(ZonedDateTime.now().minusDays(2))
            return "{" +
                    "\"_ts\" : ISODate(\"${message.sentTime}\")," +
                    "\"NhsLoginId\" : \"${nhsLoginId}\"," +
                    "\"Sender\" : \"${message.sender}\"," +
                    "\"Version\" : ${message.version}," +
                    "\"Body\" : \"${message.body}\"," +
                    "\"ReadTime\" : $readString" +
                    "\"SentTime\" : ISODate(\"${message.sentTime}\")" +
                    "},"
        }

        private fun dateAsIsoDate(date: ZonedDateTime): String {
            return "ISODate(\"${MongoDBConnection.mongoDateFormatter.format(date)}\")"
        }
    }
}
