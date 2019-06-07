package accessibility

import java.io.BufferedReader
import java.io.File
import java.io.InputStreamReader
import kotlin.system.exitProcess

const val EXPECTED_ARG_COUNT = 4
const val INPUT_FOLDER_ARG_INDEX = 0
const val CMD_ARG_INDEX = 1
const val OUTPUT_FOLDER_ARG_INDEX = 2
const val CONFIG_FILE_ARG_INDEX = 3

data class FileReport(val Console: String = "", val Source: String = "", val Errors: String = "")

open class AccessibilityCheckRunner {
    companion object {
        var errorsEncountered = false
        var ignoreErrorMap: Map<String, String> = mapOf(
                // Add comma separated error codes for individual page to ignore
                "AppointmentBooking.html" to "WCAG2AA.Principle3.Guideline3_2.3_2_2.H32.2"
        )

        @JvmStatic
        fun main(args: Array<String>) {
            if (args.size == EXPECTED_ARG_COUNT) {
                val folderToScan = args[INPUT_FOLDER_ARG_INDEX]
                val runCommand = args[CMD_ARG_INDEX]
                val outputFolder = args[OUTPUT_FOLDER_ARG_INDEX]
                val configFile = args[CONFIG_FILE_ARG_INDEX]
                val fileReports = ArrayList<FileReport>()

                File(folderToScan).walk().forEach {
                    if (it.extension == "htm" || it.extension == "html") {
                        val report = AccessibilityCheckRunner().pa11y(
                            runCommand,
                            it.canonicalPath,
                            configFile,
                            it.name
                        )
                        fileReports.add(report)
                        it.copyTo(File(outputFolder).resolve(it.name), true)
                    }
                }

                HTMLFile().generateOutputFile(fileReports.toTypedArray(), outputFolder)
                when (errorsEncountered) {
                    true -> exitProcess(1)
                    else -> exitProcess(0)
                }
            }
            exitProcess(1)
        }
    }

    fun pa11y(runCommand: String, sourceToCheck: String, configFile: String, fileName: String): FileReport {
        var consoleOutput = ""
        var failures = ""
        var loopNum = 0
        var ignoreParameter = ""

        if (ignoreErrorMap.contains(fileName)) {
            ignoreParameter = "--ignore ${ignoreErrorMap.getValue(fileName)}"
        }

        val process = Runtime.getRuntime().exec("$runCommand " +
                "--config $configFile $ignoreParameter file://$sourceToCheck")

        val stdInput = BufferedReader(InputStreamReader(process.inputStream, "UTF-8"))
        val stdError = BufferedReader(InputStreamReader(process.errorStream, "UTF-8"))

        var lineRead = stdInput.readLine()
        while (lineRead != null) {
            if (lineRead.contains("Error: ", false)) {
                val errorId = sourceToCheck.split("/", "\\").last() + loopNum
                failures += "<a href=#$errorId>${lineRead.escapeHTML()}</a><br>\n"
                consoleOutput += "<span id=$errorId>${lineRead.escapeHTML()}</span><br>\n"
                loopNum ++
            } else {
                consoleOutput += "${lineRead.escapeHTML()}<br>\n"
            }
            lineRead = stdInput.readLine()
        }

        var errRead = stdError.readLine()
        while (errRead != null) {
            consoleOutput += "${errRead.escapeHTML()}<br>\n"
            errRead = stdError.readLine()
        }

        val exitCode = process.waitFor()
        consoleOutput += "Pa11y exit code: $exitCode<br>\n"
        if (exitCode > 0) {
            errorsEncountered = true
        }
        return FileReport(consoleOutput, sourceToCheck, failures)
    }

    private fun String.escapeHTML(): String {
        val text: String = this@escapeHTML
        if (text.isEmpty()) {
            return text
        }

        return buildString(length) {
            for (index in 0 until text.length) {
                val ch: Char = text[index]
                when (ch) {
                    '\'' -> append("&apos;")
                    '\"' -> append("&quot")
                    '&' -> append("&amp;")
                    '<' -> append("&lt;")
                    '>' -> append("&gt;")
                    else -> append(ch)
                }
            }
        }
    }
}

class HTMLFile {
    private val outputFileName = "index.html"
    fun generateOutputFile(fileReports: Array<FileReport>, outputFolder: String) {
        var iteration = 0
        var htmlSections = ""
        fileReports.forEach {
            htmlSections += createSection(it, iteration)
            iteration++
        }
        File(outputFolder).resolve(outputFileName).writeText(
            """<!DOCTYPE html><html lang="en" >
                <head>
                    <title>Accessibility test results</title>
                    <style>
                        * {font-family: "arial", "Verdana", sans-serif}

                        .heading {padding: 0.25em;
                            width: 100%;
                            border: none;
                            text-align: left;
                            outline: none;
                            font-size: 1.2em;
                            display:inline;
                        }

                        .page-heading {padding: 0.25em;
                            width: 100%;
                            border: none;
                            text-align: center;
                            outline: none;
                            font-size: 1.6em;
                        }

                        .toggle {
                            background-color: #0072CE;
                            color: white;
                            cursor: pointer;
                            padding: 0.25em;
                            width: 100%;
                            border: none;
                            text-align: left;
                            outline: none;
                            font-size: 1em;
                            border-radius: 5px;
                            margin-top: 0.125em;
                        }
                        .toggle.failed {
                            background-color: #DA291C;
                        }

                        .active, .toggle:hover {
                            background-color: #FFB81C;
                        }
                        .console-output {
                            padding: 0 1.2em;
                            display: none;
                            overflow: hidden;
                            background-color: rgba(0, 112, 204, 0.2);
                            font-size: 0.75em;
                            border-radius: 5px;
                            overflow-x: auto;
                        }
                        .console-output.failed {
                            background-color: rgba(217, 41, 28, 0.2);
                        }
                    </style>
                </head>
                <body>
                    <div class="page-heading"><strong>Accessibility Test Results<strong></div>
                        $htmlSections
                    <script type="text/javascript">
                        function toggle_visibility(id) {
                            var element = document.getElementById(id);
                            if(element.style.display == 'block')
                                element.style.display = 'none';
                            else
                                element.style.display = 'block'; }
                    </script>
                </body>
            </html>"""
        )
    }

    private fun createSection(fileReport: FileReport, iteration: Int): String {
        val fileName = File(fileReport.Source).name

        var element = """
            <br>
            <br><div class=heading><a href="$fileName">${fileName.capitalize()}</a></div>"""

        var failedClass = ""
        if (fileReport.Errors.isNotEmpty()) {
            failedClass = " failed"
            element += """
                <button class="toggle failed"
                        onclick="toggle_visibility('failure_content$iteration');">
                    Accessibility errors detected in page
                </button>
                <div id="failure_content$iteration" class="console-output failed">
                    <p>${fileReport.Errors}</p>
                </div>
                """
        }
        element += """
                <button class="toggle $failedClass"
                        onclick="toggle_visibility('complete_content$iteration');">
                    Complete page test output
                </button>
                <div id="complete_content$iteration"class="console-output $failedClass">
                <p>${fileReport.Console}</p>
                </div>"""
        return element
    }
}
