package com.nhs.online.nhsonline.services.knownservices.enums

import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = false)
enum class MenuTab(val tabIndex: Int) {
    None(-1),
    Advice(0),
    Appointments(1),
    Prescriptions(2),
    MyRecord(3),
    Messages(4),
    Unknown(-2)
}
