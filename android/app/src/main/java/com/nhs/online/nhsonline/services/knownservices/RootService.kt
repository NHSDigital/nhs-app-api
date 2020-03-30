package com.nhs.online.nhsonline.services.knownservices

import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.IntegrationLevel
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = true)
data class RootService(
        override var requiresAssertedLoginIdentity: Boolean,
        override var validateSession: Boolean,
        override var menuTab: MenuTab,
        override var javaScriptInteractionMode: JavaScriptInteractionMode,
        override var showSpinner: Boolean,
        override var integrationLevel: IntegrationLevel,
        var url: String,
        var subServices: List<SubService>? = null
): KnownService
