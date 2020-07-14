package utils

import pages.ELEMENT_RETRY_TIME
import pages.MILLISECONDS_IN_A_SECOND
import pages.TIME_TO_WAIT_FOR_ELEMENT
import java.io.File

class DownloadHelpers {
     fun downloadFile(attachmentName: String): Boolean {
        var hasFileDownloaded  = false
        val currentDir = System.getProperty("user.dir")
        val dir = File("$currentDir/tmpDownloads")
        dir.mkdirs()

        var retryCount = (TIME_TO_WAIT_FOR_ELEMENT / ELEMENT_RETRY_TIME).toInt()
        while (!hasFileDownloaded && retryCount > 0) {
            val dirContents = dir.listFiles()

            val expected = dirContents.filter { it.name == attachmentName }

            if (expected.isNotEmpty()) {
                hasFileDownloaded = true
                break
            }
            println("File not found, possibly still downloading. Retrying...")
            retryCount--
            Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
        }
        return hasFileDownloaded
    }
}
