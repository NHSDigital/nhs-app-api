package com.nhs.online.nhsonline.services.knownservices.enums

import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = false)
enum class IntegrationLevel {
    Gold,
    GoldOverlay,
    GoldWithNoHeaders,
    SilverWithoutWebNavigation,
    SilverWithWebNavigation,
    Bronze,
    Unknown,
}