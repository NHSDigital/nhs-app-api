package com.nhs.online.nhsonline.support

import android.content.Context

private const val HELP_URL = "HelpUrl"

class PersistData(context: Context) : AppSharedPref(context) {
    fun getHelpUrl() = readString(HELP_URL)

    fun storeHelpUrl(url: String) = storeString(HELP_URL, url)
}