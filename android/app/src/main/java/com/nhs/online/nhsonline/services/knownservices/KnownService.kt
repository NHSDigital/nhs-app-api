package com.nhs.online.nhsonline.services.knownservices

import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.IntegrationLevel
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode

interface KnownService {
    var requiresAssertedLoginIdentity: Boolean
    var validateSession: Boolean
    var menuTab: MenuTab
    var javaScriptInteractionMode: JavaScriptInteractionMode
    var showSpinner: Boolean
    var integrationLevel: IntegrationLevel

    val viewMode: ViewMode
        get() = when(integrationLevel) {
            IntegrationLevel.Bronze -> ViewMode.AppTab
            else -> ViewMode.WebView
        }
}
