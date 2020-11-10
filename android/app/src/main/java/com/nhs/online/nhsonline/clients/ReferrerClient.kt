package com.nhs.online.nhsonline.clients

import android.util.Log
import com.android.installreferrer.api.InstallReferrerClient
import com.android.installreferrer.api.InstallReferrerStateListener
import com.android.installreferrer.api.ReferrerDetails
import com.nhs.online.nhsonline.support.AppSharedPref

class ReferrerClient (
        private val prefs: AppSharedPref,
        private val referrerClient: InstallReferrerClient,
){

    fun storeReferrer() {
        if (prefs.readString("appReferrer", "") == null ||
                prefs.readString("appReferrer", "") == ""){
            createReferrerClientAndStoreDetails()
        }
    }

    fun getReferrer(): String {
        val referrer = prefs.readString("appReferrer", "")

        return if (!referrer.isNullOrBlank()){
            referrer
        } else {
            ""
        }
    }

    private fun createReferrerClientAndStoreDetails(){
        Log.d("ReferrerClient", "starting referrer client connection to store details")
        referrerClient.startConnection(object : InstallReferrerStateListener {

            override fun onInstallReferrerSetupFinished(responseCode: Int) {
                when (responseCode) {
                    InstallReferrerClient.InstallReferrerResponse.OK -> {
                        val response: ReferrerDetails = referrerClient.installReferrer
                        Log.d("ReferrerClient", "Connection established to retrieve referrer information")

                        prefs.storeString("appReferrer", response.installReferrer)
                        referrerClient.endConnection()
                    }

                    InstallReferrerClient.InstallReferrerResponse.FEATURE_NOT_SUPPORTED -> {
                        Log.d("ReferrerClient", "Unsupported feature for logging referrer")
                        referrerClient.endConnection()
                    }

                    InstallReferrerClient.InstallReferrerResponse.SERVICE_UNAVAILABLE -> {
                        Log.d("ReferrerClient", "Service not available for install referrer")
                        referrerClient.endConnection()
                    }

                    InstallReferrerClient.InstallReferrerResponse.SERVICE_DISCONNECTED -> {
                        Log.d("ReferrerClient", "Service has been disconnected for install referrer")
                        referrerClient.endConnection()
                    }

                    InstallReferrerClient.InstallReferrerResponse.DEVELOPER_ERROR -> {
                        Log.d("ReferrerClient", "Unable to create client connection due to a developer error response")
                        referrerClient.endConnection()
                    }
                }
            }

            override fun onInstallReferrerServiceDisconnected() {
                Log.d("ReferrerClient", "Referrer client has been disconnected")
            }
        })
    }
}
