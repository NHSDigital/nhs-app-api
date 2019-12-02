package mocking.data.myrecord

import java.io.File
import java.nio.file.Paths

object ExaminationsData {
    fun getVisionExaminationsDataWithMultipleResults(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        val path = Paths.get("").toAbsolutePath().toString()
        val fileLocation = "$path/src/main/kotlin/mocking/data/myrecord/VariousExaminations.html"
        val html = File(fileLocation).readText()

        return response + html + responseStringEnd
    }

    fun getVisionExaminationsDataWithNoExaminationsData(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }
}
