package mocking.data.myrecord

import mocking.tpp.models.RequestBinaryDataReply

object TppDocumentData {

    fun getDefaultDocumentData(): RequestBinaryDataReply {
        return RequestBinaryDataReply()
    }
}