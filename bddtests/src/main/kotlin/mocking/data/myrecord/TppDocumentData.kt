package mocking.data.myrecord

import mocking.tpp.models.RequestBinaryDataReply

object TppDocumentData {
    fun getDocumentData(documentType: String? = null): RequestBinaryDataReply {
        val dataReply = RequestBinaryDataReply()

        if (documentType != null) {
            dataReply.event.fileType = documentType
        }

        return dataReply
    }
}
