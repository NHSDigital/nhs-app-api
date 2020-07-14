package mocking.data.myrecord

import mocking.emis.documents.DocumentResponseModel
import org.apache.commons.codec.binary.Base64OutputStream
import java.io.ByteArrayOutputStream
import java.io.File
import java.nio.file.Paths
import java.util.zip.GZIPOutputStream


object DocumentData {

    fun getDefaultDocumentData(): DocumentResponseModel {
        return DocumentResponseModel(loadFile())
    }

    private fun loadFile(): String {
        var data = ""
        val path = Paths.get("").toAbsolutePath().toString()
        val fileLocation = "$path/src/main/kotlin/mocking/data/myrecord/pdf.html"
        File(fileLocation).forEachLine {
            data += it
        }

        return ByteArrayOutputStream().use { baos ->
            Base64OutputStream(baos).use { base64 ->
                GZIPOutputStream(base64).use { gzip ->
                    gzip.write(data.toByteArray())
                }
            }
            baos.toString()
        }
    }
}
