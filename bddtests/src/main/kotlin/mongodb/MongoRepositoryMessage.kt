package mongodb

import org.bson.types.ObjectId
import worker.models.messages.SingleMessageFacade
import java.time.ZonedDateTime

data class MongoRepositoryMessage(val NhsLoginId: String?,
                                  val Sender: String,
                                  val Version: Int,
                                  val Body: String,
                                  val ReadTime: String?,
                                  val SenderContext: MongoRepositoryMessageSenderContext?,
                                  val Reply: MongoRepositoryMessageReply?,
                                  val _id: ObjectId?) {
    companion object {
        // We cannot serialise an object to create this because the ISODate objects cannot be created like that.
        fun createJson(message: SingleMessageFacade, nhsLoginId: String): String {
            val readString = if (!message.read) null else dateAsIsoDate(ZonedDateTime.now().minusDays(2))

            val senderContext =
                if (message.senderContext != null)
                    ", \"SenderContext\" : { \"SenderId\" : \"${message.senderContext.senderId}\" }"
                else ""

            return "{" +
                    "\"_id\" : ObjectId(\"${ObjectId().toHexString()}\")," +
                    "\"_ts\" : ISODate(\"${message.sentTime}\")," +
                    "\"NhsLoginId\" : \"${nhsLoginId}\"," +
                    "\"Sender\" : \"${message.sender}\"," +
                    "\"Version\" : ${message.version}," +
                    "\"Body\" : \"${message.body}\"," +
                    "\"ReadTime\" : $readString" +
                    "\"SentTime\" : ISODate(\"${message.sentTime}\")" +
                    senderContext +
                    appendReply(message) +
                    "}"
        }

        private fun appendReply(message: SingleMessageFacade): String {

            val responseDateTimeString = if (message.reply?.response != null)
                dateAsIsoDate(ZonedDateTime.now())
            else
                null

            val response =
                    if (message.reply?.response != null)
                        "\"Response\": \"${message.reply.response}\"," +
                                "\"ResponseDateTime\":$responseDateTimeString"
                    else ""

            return if (message.reply != null)
                ", \"Reply\": {\n" +
                        "\"Options\": [ ${appendReplyOptions(message).dropLast(1)} ]," + response + " }";
            else ""
        }

        private fun appendReplyOptions(message: SingleMessageFacade) : StringBuilder {
            val messageReplyOption = if ((message.reply != null) && (message.reply.options != null))
                message.reply.options
            else
                null
            val messageReplyOptionString = StringBuilder()

            if (messageReplyOption != null) {
                for (option in messageReplyOption) {
                    messageReplyOptionString.append("{\"Code\": \"${option.code}\"," +
                            " \"Display\": \"${option.display}\"},")
                }
            }
            return messageReplyOptionString
        }

        private fun dateAsIsoDate(date: ZonedDateTime?): String {
            return "ISODate(\"${MongoDBConnection.mongoDateFormatter.format(date)}\")"
        }
    }
}
