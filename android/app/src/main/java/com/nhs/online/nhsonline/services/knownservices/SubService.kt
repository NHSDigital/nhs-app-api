package com.nhs.online.nhsonline.services.knownservices

import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode
import com.squareup.moshi.JsonClass

@JsonClass(generateAdapter = true)
data class SubService(
        override var requiresAssertedLoginIdentity: Boolean,
        override var validateSession: Boolean,
        override var menuTab: MenuTab,
        override var viewMode: ViewMode,
        override var javaScriptInteractionMode: JavaScriptInteractionMode,
        override var showSpinner: Boolean,
        var path: String? = null,
        var queryString: String? = null
): KnownService