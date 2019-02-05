package com.nhs.online.nhsonline.support

import android.content.Context

private const val BETA_COOKIE = "BetaCookie"

class PersistData(context: Context) : AppSharedPref(context) {
    fun getBetaCookie() = readString(BETA_COOKIE)

    fun storeBetaCookie(betaCookie: String) = storeString(BETA_COOKIE, betaCookie)
}