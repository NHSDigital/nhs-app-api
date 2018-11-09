package com.nhs.online.nhsonline

import android.app.Application
import android.app.admin.DevicePolicyManager
import android.content.ComponentName
import android.content.Context
import android.content.pm.ApplicationInfo
import android.preference.PreferenceManager
import android.speech.tts.TextToSpeech
import android.speech.tts.TextToSpeech.*
import android.view.WindowManager
import android.webkit.WebView
import java.util.*
import kotlin.concurrent.schedule
import kotlin.math.max

@Suppress("unused") // This is referenced by the AndroidManifest.xml
class Application : Application() {


    override fun onCreate() {
        super.onCreate()

        setLocale()
        enableWebDebuggingIfApplicationDebuggable()
    }

    private fun setLocale() {
        var textToSpeech = TextToSpeech(this, null)

        // Note we are specifying a language, and a region, the variant doesn't matter.
        val locale = Locale(resources.getString(R.string.locale_language), resources.getString(R.string.locale_region))

        // This method checks the device to see if the specified locale is installed, and returns one of five constants (integers):
        // 1. LANG_AVAILABLE (0) - Denotes the language is available for the language by the locale, but not the country and variant.
        // 2. LANG_COUNTRY_AVAILABLE (1) - Denotes the language is available for the language and country specified by the locale, but not the variant.
        // 3. LANG_COUNTRY_VAR_AVAILABLE (2) - Denotes the language is available exactly as specified by the locale.
        // 4. LANG_MISSING_DATA (-1) - Denotes the language data is missing.
        // 5. LANG_NOT_SUPPORTED (-2) - Denotes the language is not supported.
        var availability = textToSpeech.isLanguageAvailable(locale)

        // Either of the following indicate the language/region combo is installed on the device. If they aren't, we leave things as they are.
        // language is a property on textToSpeech, the language for the app is set on it's setter, so it doesn't matter that we aren't doing anything with the instance.
        when (availability) {
            LANG_COUNTRY_AVAILABLE, LANG_COUNTRY_VAR_AVAILABLE -> {
                textToSpeech.language = locale
            }
        }
    }

    private fun enableWebDebuggingIfApplicationDebuggable() {
        if ((applicationInfo.flags and ApplicationInfo.FLAG_DEBUGGABLE) == ApplicationInfo.FLAG_DEBUGGABLE) {
            WebView.setWebContentsDebuggingEnabled(true)
        }
    }

    companion object {
        val TAG = "NHSApp"
    }
}
