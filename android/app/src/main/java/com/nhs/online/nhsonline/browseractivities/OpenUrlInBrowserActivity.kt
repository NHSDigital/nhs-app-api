package com.nhs.online.nhsonline.browseractivities

import android.content.Context
import android.content.Intent
import android.content.pm.ResolveInfo
import android.graphics.Color
import android.net.Uri
import androidx.browser.customtabs.CustomTabsIntent
import androidx.browser.customtabs.CustomTabsService.ACTION_CUSTOM_TABS_CONNECTION
import androidx.core.content.ContextCompat
import android.util.Log
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor

private const val CHROME_PACKAGE_NAME = "com.android.chrome"

class OpenUrlInBrowserActivity : ActivityInterface {

    override fun start(context: Context, url: String, interactor: IInteractor) {
        val supportedCustomTabsPackages = getCustomTabsPackages(context, url)

        if (supportedCustomTabsPackages.count() > 0) {
            val customTabsIntent = CustomTabsIntent.Builder()
                .setToolbarColor(Color.BLUE)
                .build()

            customTabsIntent.intent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP)

            if (supportedCustomTabsPackages.any { it.activityInfo.packageName == CHROME_PACKAGE_NAME }) {
                customTabsIntent.intent.setPackage(CHROME_PACKAGE_NAME)
            } else {
                customTabsIntent.intent.setPackage(supportedCustomTabsPackages[0].activityInfo.packageName)
            }
            customTabsIntent.launchUrl(context, Uri.parse(url))
        } else {
            val intent = Intent(Intent.ACTION_VIEW, Uri.parse(url))

           if (intent.resolveActivity(context.packageManager) != null) {
               ContextCompat.startActivity(context, intent, null)
           } else {
               val unavailableErrorMessage = ErrorMessage(context.resources, ErrorType.BrowserNotAvailable)
               Log.d(Application.TAG, "Browser is unavailable or disabled")
               interactor.showUnavailabilityError(unavailableErrorMessage)
           }
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
