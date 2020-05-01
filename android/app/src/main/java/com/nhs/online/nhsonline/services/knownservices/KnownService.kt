package com.nhs.online.nhsonline.services.knownservices

import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode

interface KnownService {
    var requiresAssertedLoginIdentity: Boolean
    var validateSession: Boolean
    var menuTab: MenuTab
    var viewMode: ViewMode
    var javaScriptInteractionMode: JavaScriptInteractionMode
    var showSpinner: Boolean
}