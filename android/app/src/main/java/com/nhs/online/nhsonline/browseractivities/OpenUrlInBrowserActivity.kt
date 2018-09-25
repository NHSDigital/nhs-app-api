package com.nhs.online.nhsonline.browseractivities

import android.content.Context
import android.content.Intent
import android.graphics.Color
import android.net.Uri
import android.support.customtabs.CustomTabsIntent
import android.support.v4.content.ContextCompat
import java.net.URL
import android.support.customtabs.CustomTabsService.ACTION_CUSTOM_TABS_CONNECTION
import android.content.pm.ResolveInfo
import com.nhs.online.nhsonline.services.KnownServices

class OpenUrlInBrowserActivity(val nativeAppHosts: Array<String>) : ActivityInterface
{
    private val CHROME_PACKAGE_NAME = "com.android.chrome"

    override fun canStart(context: Context, url: String): Boolean
    {
        val knownServices = KnownServices(context)
        if (knownServices.shouldURLOpenExternally(URL(url))){
            return true
        }
        val currentHost = URL(url).host
        nativeAppHosts.forEach { nativeAppHost ->
            if (URL(nativeAppHost).host == currentHost) {
                return false
            }
        }

        return true
    }

    override fun start(context: Context, url: String)
    {
        if (!canStart(context, url)) {
            throw RuntimeException("Cannot open url in browser")
        }

        var supportedCustomTabsPackages = getCustomTabsPackages(context, url)

        val knownServices = KnownServices(context)

            if (supportedCustomTabsPackages.count() > 0
                    && !knownServices.isHotJar(URL(url))) {
                val customTabsIntent = CustomTabsIntent.Builder()
                        .setToolbarColor(Color.BLUE)
                        .build()

                if (supportedCustomTabsPackages.any { it.activityInfo.packageName == CHROME_PACKAGE_NAME }) {
                    customTabsIntent.intent.setPackage(CHROME_PACKAGE_NAME)
                } else {
                    customTabsIntent.intent.setPackage(supportedCustomTabsPackages[0].activityInfo.packageName)
                }
                customTabsIntent.launchUrl(context, Uri.parse(url))
            } else {
                val intent = Intent(Intent.ACTION_VIEW, Uri.parse(url))

                ContextCompat.startActivity(context, intent, null)
            }

    }

    private fun getCustomTabsPackages(context: Context, urlString: String): ArrayList<ResolveInfo> {
        val pm = context.packageManager
        val activityIntent = Intent(Intent.ACTION_VIEW, Uri.parse(urlString))
        val resolvedActivityList = pm.queryIntentActivities(activityIntent, 0)
        val packagesSupportingCustomTabs = ArrayList<ResolveInfo>()

        for (info in resolvedActivityList) {
            val serviceIntent = Intent()
            serviceIntent.action = ACTION_CUSTOM_TABS_CONNECTION
            serviceIntent.setPackage(info.activityInfo.packageName)

            if (pm.resolveService(serviceIntent, 0) != null) {
                packagesSupportingCustomTabs.add(info)
            }
        }

        return packagesSupportingCustomTabs
    }
}
