package mocking.data.myrecord

object  DiagnosisData {

    fun getVisionDiagnosisDataWithNoTestResults(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }

    fun getVisionDiagnosisDataWithMultipleResults(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }
}