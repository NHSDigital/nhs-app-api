package com.nhs.online.nhsonline.support.intentHandlers

import android.content.Intent
import com.nhs.online.nhsonline.web.NhsWeb

interface IIntentHandler
{
    val intentAction: String

    fun handle(intent: Intent, isAppClosed: Boolean, nhsWeb: NhsWeb)
}