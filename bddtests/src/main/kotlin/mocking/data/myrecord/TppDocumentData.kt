package mocking.data.myrecord

import mocking.tpp.models.RequestBinaryDataReply

object TppDocumentData {
    fun getDocumentData(documentType: String? = null, multiPage: Boolean = false): RequestBinaryDataReply {
        val dataReply = RequestBinaryDataReply(multiPage)

        if (documentType != null) {
            dataReply.event.fileType = documentType
        }

        return dataReply
    }
}
