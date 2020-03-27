package com.nhs.online.nhsonline.support

import com.nhs.online.nhsonline.BuildConfig
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.activities.MainActivity
import com.nhs.online.nhsonline.services.Configuration
import com.nhs.online.nhsonline.services.knownservices.KnownServices
import com.nhs.online.nhsonline.services.ConfigurationService
import java.util.concurrent.Executors

class ConfigurationServiceManager(private val activity: MainActivity, private val configurationService: ConfigurationService) {

    fun getConfigurationResponse(): ConfigurationResponse {
        activity.showProgressDialog()

        val configuration = callConfiguration()
        val configurationResponse = ConfigurationResponse()

        configuration?.let { config ->
            val isSupportedVersion = isSupportedVersion(config.minimumSupportedAndroidVersion)
            val fidoServerUrl = config.fidoServerUrl
            val knownServices = KnownServices(config.knownServices)

            configurationResponse.isSupportedVersion = isSupportedVersion
            configurationResponse.fidoServerUrl = fidoServerUrl
            configurationResponse.knownServices = knownServices
            configurationResponse.nhsLoginLoggedInPaths = config.nhsLoginLoggedInPaths ?: listOf<String>()
            configurationResponse.callSuccessful = true
        }

        activity.dismissProgressDialog()
        return configurationResponse
    }

    private fun callConfiguration(): Configuration? {
        val threadPool = Executors.newSingleThreadExecutor()

        val task = configurationService
        val future = threadPool.submit(task)

        val configuration = future.get()

        threadPool.shutdown()

        return configuration
    }

    private fun isSupportedVersion(minimumSupportedAndroidVersionString: String): Boolean {
        val appVersion = stringToKotlinVersion(BuildConfig.VERSION_NAME)
        val minimumSupportedAndroidVersion = stringToKotlinVersion(minimumSupportedAndroidVersionString)

        appVersion?.let {
            minimumSupportedAndroidVersion?.let {
                if (minimumSupportedAndroidVersion <= appVersion) {
                    return true
                }
            }
        }

        return false
    }

    private fun stringToKotlinVersion(versionString: String): KotlinVersion? {
        val versionNumberRegex = Regex(activity.getString(R.string.versionRegex))

        if (!versionNumberRegex.matches(versionString)) {
            return null
        }

        val versionNumbers = versionString.split(".")

        return when (versionNumbers.size) {
            2 -> KotlinVersion(versionNumbers[0].toInt(), versionNumbers[1].toInt())
            3 -> KotlinVersion(versionNumbers[0].toInt(), versionNumbers[1].toInt(), versionNumbers[2].toInt())
            else -> null
        }
    }
}