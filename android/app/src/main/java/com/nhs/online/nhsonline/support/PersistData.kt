package com.nhs.online.nhsonline.support

import android.content.Context

private const val HELP_URL = "HelpUrl"
private const val PERSISTED_LINK = "PersistedLink"

class PersistData(context: Context) : AppSharedPref(context) {
    fun getHelpUrl() = readString(HELP_URL)

    fun storeHelpUrl(url: String) = storeString(HELP_URL, url)

    fun getPersistedLink() = readString(PERSISTED_LINK)

    fun storePersistedLink(persistedLink: String) = storeString(PERSISTED_LINK, persistedLink)

    fun clearPersistedLink() = storeString(PERSISTED_LINK, "")
}
