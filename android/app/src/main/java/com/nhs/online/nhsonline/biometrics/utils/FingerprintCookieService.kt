package com.nhs.online.nhsonline.biometrics.utils

import android.app.Activity
import android.util.Log
import android.webkit.CookieManager
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.biometrics.FingerprintService
import org.json.JSONObject
import java.net.URLDecoder

class FingerprintCookieService(val activity: Activity) {

    companion object {
        private val TAG = FingerprintService::class.java.simpleName
    }

    fun getAccessTokenFromCookie(): String? {
        val cookies = CookieManager.getInstance()
            .getCookie(activity.resources.getString(R.string.baseURL))
        Log.d(TAG, "All the cookies in a string:$cookies")

        return if (cookies != null) {
            val cookieValueDecoded = URLDecoder.decode(getCookieValue(cookies), "UTF-8")
            if (cookieValueDecoded != null) {
                JSONObject(cookieValueDecoded).getString(activity.resources.getString(R.string.accessToken))
            } else {
                null
            }
        } else {
            null
        }
    }

    private fun getCookieValue(cookies: String): String {
        var cookieValue = ""
        val cookieName = activity.resources.getString(R.string.sessionDetailsCookieName)
        val delimitedCookies = cookies.split(";")

        for (cookie in delimitedCookies) {
            if (cookie.contains(cookieName)) {
                val cookieParts =
                    cookie.split("=".toRegex()).dropLastWhile { it.isEmpty() }.toTypedArray()
                if (cookieParts.size < 2)
                    continue
                cookieValue = cookieParts[1]
                break
            }
        }
        return cookieValue
    }
}