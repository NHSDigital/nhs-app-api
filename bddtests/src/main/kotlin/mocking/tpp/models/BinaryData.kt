package mocking.tpp.models

import org.apache.commons.io.FileUtils
import java.io.File
import java.nio.file.Paths
import java.util.*
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement


@XmlAccessorType(XmlAccessType.FIELD)
data class BinaryData(
        @XmlAttribute var fileType: String = "jpg",
        @field:XmlElement(name = "BinaryDataPage") var binaryDataPage: BinaryDataPage = BinaryDataPage(
                loadFile()
        )
)

private fun loadFile(): String {
        val path = Paths.get("").toAbsolutePath().toString()
        val fileLocation = "$path/src/main/kotlin/mocking/data/myrecord/OccupationsTest2000px.jpg"

        val fileContent = FileUtils.readFileToByteArray(File(fileLocation))

        return Base64.getEncoder().encodeToString(fileContent)
}
