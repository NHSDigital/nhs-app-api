package com.nhs.online.nhsonline.manifests

import android.util.Xml
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import org.robolectric.annotation.Config
import org.xmlpull.v1.XmlPullParser
import java.io.File

@RunWith(RobolectricTestRunner::class)
@Config(manifest = Config.NONE)
class AndroidManifestContentTests {
    private val sourceDirPath = "../app/src"
    private val installLocationAttribute = "installLocation"
    private val expectedInstallLocation = "internalOnly"
    private val allowBackupAttribute = "allowBackup"
    private val expectedAllowBackupValue = "false"

    @Test
    fun appInstallsAndStoresOnlyOnInternalMemory() {
        val sourceDir = File(sourceDirPath)
        Assert.assertTrue("Specified directory $sourceDirPath doesn't exist", sourceDir.exists())

        val manifestFiles = searchManifestFiles(sourceDir)
        Assert.assertTrue("There are AndroidManifest.xml files",
            manifestFiles.isNotEmpty())

        manifestFiles.forEach { manifest ->
            val installLocations = findXmlTagAttributeInXmlFile(installLocationAttribute, manifest)
            Assert.assertFalse("More than 1 Install Location in ${manifest.path} tags: $installLocations",
                installLocations.size > 1)

            if (installLocations.isNotEmpty()) {
                val (tag, installLocationValue) = installLocations[0]
                Assert.assertEquals("Expected attribute is not defined in manifest tag",
                    "manifest", tag)
                Assert.assertEquals("Expected install location $expectedInstallLocation not found but found $installLocationValue in ${manifest.path}",
                    expectedInstallLocation, installLocationValue)
            }
        }
    }

    @Test
    fun appDisallowsBackups() {
        val sourceDir = File(sourceDirPath)
        Assert.assertTrue("Specified directory $sourceDirPath doesn't exist", sourceDir.exists())

        val manifestFiles = searchManifestFiles(sourceDir)
        Assert.assertTrue("There are AndroidManifest.xml files",
            manifestFiles.isNotEmpty())

        manifestFiles.forEach { manifest ->
            val allowBackupValues = findXmlTagAttributeInXmlFile(allowBackupAttribute, manifest)
            Assert.assertFalse("More than 1 allowBackup attribute in ${manifest.path} tags: $allowBackupValues",
                allowBackupValues.size > 1)

            if (allowBackupValues.isNotEmpty()) {
                val (tag, allowBackup) = allowBackupValues[0]
                Assert.assertEquals("Expected attribute is not defined in manifest tag",
                    "application", tag)
                Assert.assertEquals("Expected allowBackup value " +
                        "$expectedAllowBackupValue not found but found $allowBackup in ${manifest.path}",
                    expectedAllowBackupValue, allowBackup)
            } else {
                Assert.fail("Expected to find an allowBackup attribute with the value $expectedAllowBackupValue")
            }
        }
    }

    private fun searchManifestFiles(sourceDir: File): List<File> {
        val manifestFiles = arrayListOf<File>()
        sourceDir.listFiles()?.forEach {
            if (it.isDirectory) {
                val manifestFile = File(it, Config.DEFAULT_MANIFEST_NAME)
                if (manifestFile.exists())
                    manifestFiles.add(manifestFile)
            }
        }
        return manifestFiles
    }

    private fun findXmlTagAttributeInXmlFile(
        attribute: String,
        xmlFile: File
    ): List<Pair<String, String>> {
        val installLocations = arrayListOf<Pair<String, String>>()
        xmlFile.inputStream().use { xmlInputStream ->
            val xmlParser = Xml.newPullParser()
            xmlParser.setInput(xmlInputStream, null)

            while (xmlParser.next() != XmlPullParser.END_DOCUMENT) {
                if (xmlParser.eventType == XmlPullParser.START_TAG) {
                    val value: String? = xmlParser.getAttributeValue(null, attribute)
                    value?.let { installLocations.add(Pair(xmlParser.name, value)) }
                }
            }
        }

        return installLocations
    }
}