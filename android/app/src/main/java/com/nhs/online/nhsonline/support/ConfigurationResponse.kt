package com.nhs.online.nhsonline.support

import com.nhs.online.nhsonline.services.knownservices.KnownServices

class ConfigurationResponse {
    var isSupportedVersion: Boolean = false
    var fidoServerUrl: String = ""
    var knownServices: KnownServices? = null
    var callSuccessful: Boolean = false
    var nhsLoginLoggedInPaths: List<String> = listOf<String>()
}