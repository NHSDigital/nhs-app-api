package mocking.data.myrecord

object ProceduresData {

    fun getVisionProceduresDataWithNoProcedures(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }

    fun getVisionProceduresDataWithMultipleProcedures(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd

    }

}