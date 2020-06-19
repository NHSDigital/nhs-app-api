package com.nhs.online.nhsonline.support.intentHandlers

import android.content.Intent
import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.web.NhsWeb

class IntentHandlers {

    private val handlers: HashMap<String, IIntentHandler> = hashMapOf()

    fun registerHandler(handler: IIntentHandler) {
        handlers[handler.intentAction] = handler
    }

    fun handleIntent(intent: Intent, isAppClosed: Boolean, nhsWeb: NhsWeb) {
        Log.d(Application.TAG, "${this::class.java.simpleName}: Entering handleIntent")

        if (intent?.action == Intent.ACTION_VIEW) {
            handlers[intent.action]?.handle(intent, isAppClosed, nhsWeb)
            return
        }

        if (intent.extras?.isEmpty == false) {
            // as firebase messaging is from google and does not come under the android package
            handlers["firebaseMessaging"]?.handle(intent, isAppClosed, nhsWeb)
            return
        }

        handlers["default"]?.handle(intent, isAppClosed, nhsWeb)
    }
}

