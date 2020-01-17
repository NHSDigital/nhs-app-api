package mocking.data.myrecord

import java.io.File
import java.nio.file.Paths

object ProceduresData {
    fun getVisionProceduresDataWithMultipleProcedures(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        val path = Paths.get("").toAbsolutePath().toString()
        val fileLocation = "$path/src/main/kotlin/mocking/data/myrecord/VariousProcedures.html"
        val html = File(fileLocation).readText()

        return response + html + responseStringEnd
    }

    fun getBadProceduresData(): String {
        return "<![BADDATA[<root><patient>responseStringEnd</patient></root>]]>"
    }
}
