package mocking.data.myrecord

object ExaminationsData {

    fun getVisionExaminationsDataWithNoExaminations(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }

    fun getVisionTestResultsDataWithMultipleResults(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd

    }
}