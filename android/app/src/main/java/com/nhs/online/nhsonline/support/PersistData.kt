package com.nhs.online.nhsonline.support

import android.content.Context

private const val BETA_COOKIE = "BetaCookie"
private const val HELP_URL = "HelpUrl"

class PersistData(context: Context) : AppSharedPref(context) {
    fun getBetaCookie() = readString(BETA_COOKIE)

    fun storeBetaCookie(betaCookie: String) = storeString(BETA_COOKIE, betaCookie)

    fun getHelpUrl() = readString(HELP_URL)

    fun storeHelpUrl(url: String) = storeString(HELP_URL, url)
}