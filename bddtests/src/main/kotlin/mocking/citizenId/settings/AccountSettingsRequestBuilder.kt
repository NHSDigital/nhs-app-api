package mocking.citizenId.settings

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class AccountSettingsRequestBuilder : CitizenIdMappingBuilder("GET", "/settings") {

    fun respondWithSettingsPage(patient: Patient?): Mapping {
        val htmlBody: String
        when {
            patient != null -> {
                val firstName = patient.name.firstName
                val surname = patient.name.surname
                htmlBody = """
                            <html>
                                <body>
                                    <h2>Account settings for $firstName $surname</h2>
                                    <dl>
                                        <dt>First Name</di>
                                        <dd>$firstName</dd>
                                        <dt>Surname</di>
                                        <dd>$surname</dd>
                                        <dt>Email</di>
                                        <dd>${patient.contactDetails.emailAddress}</dd>
                                    </dl>
                                </body>
                            </html>
                        """.trimIndent()
            }
            else -> {
                htmlBody = """
                            <html>
                                <body>
                                    <h2>Test setup incorrect. Patient is null</h2>
                                </body>
                            </html>
                        """.trimIndent()
            }
        }

        return respondWith(HttpStatus.SC_OK) { andTemplatedHtmlBody(htmlBody) }
    }
}
