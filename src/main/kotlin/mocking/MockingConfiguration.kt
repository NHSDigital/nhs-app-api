package mocking

import mocking.emis.EmisConfiguration

class MockingConfiguration {
    private val wiremockBaseUrl: String
    val emisConfiguration: EmisConfiguration
    val wiremockAdminUrl: String

    constructor(wiremockBaseUrl: String, emisConfiguration: EmisConfiguration) {
        this.emisConfiguration = emisConfiguration
        this.wiremockBaseUrl = if (wiremockBaseUrl.endsWith("/")) wiremockBaseUrl else "$wiremockBaseUrl/"
        this.wiremockAdminUrl = "${this.wiremockBaseUrl}__admin"
    }
}
