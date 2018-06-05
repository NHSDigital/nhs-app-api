package com.nhs.online.nhsonline.support

import android.arch.lifecycle.Lifecycle
import android.arch.lifecycle.LifecycleObserver
import android.arch.lifecycle.OnLifecycleEvent
import com.nhs.online.nhsonline.AppWebInterface
import com.nhs.online.nhsonline.MainActivity
import com.nhs.online.nhsonline.services.KnownServices
import kotlinx.android.synthetic.main.activity_main.*

class ValidateSessionLifeCycleObserver(private var context: MainActivity,
                               private var appWebInterface: AppWebInterface,
                               private var knownServices: KnownServices): LifecycleObserver {

    @OnLifecycleEvent(Lifecycle.Event.ON_START)
    fun onMoveToForeground() {
        if (knownServices.shouldValidateSession(context.webview.url)) {
            appWebInterface.validateSession();
        }
    }
}