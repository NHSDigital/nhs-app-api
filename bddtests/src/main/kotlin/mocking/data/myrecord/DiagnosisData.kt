package mocking.data.myrecord

import java.io.File
import java.nio.file.Paths

object  DiagnosisData {
    fun getVisionDiagnosisDataWithMultipleResults(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        val path = Paths.get("").toAbsolutePath().toString()
        val fileLocation = "$path/src/main/kotlin/mocking/data/myrecord/VariousDiagnosis.html"
        val html = File(fileLocation).readText()

        return response + html + responseStringEnd
    }

    fun getVisionDiagnosisDataWithNoDiagnosisData(): String {
        return "<![CDATA[<root><patient></patient></root>]]>"
    }
}
