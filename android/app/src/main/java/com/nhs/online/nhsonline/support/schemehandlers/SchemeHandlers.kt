package com.nhs.online.nhsonline.support.schemehandlers

import android.util.Log
import com.nhs.online.nhsonline.Application

class SchemeHandlers {

    private val handlers: HashMap<String, ISchemeHandler> = hashMapOf()

    fun registerHandler(handler: ISchemeHandler) {
        handlers[handler.scheme] = handler
    }

    fun handleUrl(url: String) : Boolean {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering handleUrl")

        val handler = handlers[extractHandlerKey(url)]

        if(handler != null) return handler.handle(url)

        return false
    }

    private fun extractHandlerKey(url: String): String {
        return url.substringBefore(":") + ":"
    }
}

