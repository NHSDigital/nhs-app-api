package com.nhs.online.nhsonline.services.knownservices.enums

import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = false)
enum class JavaScriptInteractionMode {
    None,
    NhsApp,
    SilverThirdParty,
    Unknown
}