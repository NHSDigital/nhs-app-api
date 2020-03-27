package com.nhs.online.nhsonline.services

import com.nhs.online.nhsonline.services.knownservices.RootService
import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = true)
data class Configuration (
    var minimumSupportedAndroidVersion: String,
    var fidoServerUrl: String,
    var knownServices: List<RootService>,
    var nhsLoginLoggedInPaths: List<String>?
)
