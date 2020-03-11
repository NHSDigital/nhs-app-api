package com.nhs.online.nhsonline.services.knownservices.enums

import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = false)
enum class MenuTab(val tabIndex: Int) {
    None(-1),
    Symptoms(0),
    Appointments(1),
    Prescriptions(2),
    MyRecord(3),
    More(4),
    Unknown(-2)
}