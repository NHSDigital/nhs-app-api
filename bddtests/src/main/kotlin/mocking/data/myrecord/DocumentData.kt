package mocking.data.myrecord

import mocking.emis.documents.DocumentResponseModel


object DocumentData {

    private const val COMPRESSED_DOCUMENT = "H4sIAAAAAAAEAH2QyWrEMAyGX0UYesx2GCiJYxiGXntpoGdPosSGeKmtDJM+fZ2E0g6UghYk" +
            "pI9f4orMLLhCOQhukCQoIp/hx6JvLbs4S2gp61aPDPqjahnhnYptsYFeyRCR2oXG7JlB8Q/kjdYZ/0T1Mf6sWmmwZRNaDJJc+DV7jt5F" +
            "zN9dGCKMLkD++tJBdcqrKi/zckeQphkFL77zcdjVDavgg74J7iFuOlo2JmwW9SfWVeWpgVlbzBTqSVHqnJ4aMDJM2talJ9i8SoEJHr20" +
            "D4xRGj2v9UXO+hp0A49gJjqlIySTQBgJBtfft7CYdFTOi42XhPrku8DiELv/V3wBbzBSSJ8BAAA="

    fun getDefaultDocumentData(): DocumentResponseModel {
        return DocumentResponseModel(COMPRESSED_DOCUMENT)
    }
}