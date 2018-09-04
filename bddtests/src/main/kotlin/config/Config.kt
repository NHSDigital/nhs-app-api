package config

import org.slf4j.Logger
import org.slf4j.LoggerFactory
import java.net.URI

private const val SESSION_EXPIRY_MINUTES: Long = 3

class Config private constructor() {

    var url: String
    var pfsBackendUrl: String
    var cidBackendUrl: String
    var wiremockUrl: String
    var nodeEnv: String
    var port: String

    var cidClientId: String
    var cidRedirectUri: String
    var cidNativeRedirectUri: String
    var cidAuthEndpoint: String
    val cidRegisterEndpoint: String
    val cidJwtIssuer: String
    var emisApplicationId: String
    var emisVersion: String
    var organDonation: String
    var symptomChecker: String
    var dataPreferencesHost: String
    var dataPreferencesPath: String
    var dataPreferencesUrl: String

    var browserstackAccessKey: String
    var browserstackUrl: String
    var browserstackLocal: String
    var appPath: String
    var appiumServer: String
    var sessionExpiryMinutes: Long

    init {
        url = envOrDefault("url", "http://web.local.bitraft.io:3000")
        val uri = URI(url)
        wiremockUrl = envOrDefault("wiremockUrl", "http://${uri.host}:8080")
        cidBackendUrl = envOrDefault("cidBackendUrl", "http://cid.local.bitraft.io:8084")
        pfsBackendUrl = envOrDefault("pfsBackendUrl", "http://api.local.bitraft.io:8082")
        nodeEnv = envOrDefault("NODE_ENV", "production")
        port = envOrDefault("PORT", "3000")

        val browserstackUsername = envOrDefault("BROWSERSTACK_USERNAME", "NOT_PROVIDED")
        browserstackAccessKey = envOrDefault("BROWSERSTACK_ACCESSKEY", "NOT_PROVIDED")
        browserstackUrl = "http://$browserstackUsername:$browserstackAccessKey@hub-cloud.browserstack.com/wd/hub"
        browserstackLocal = envOrDefault("BROWSERSTACK_LOCAL", "true")
        appPath = envOrDefault("APP_PATH", "NOT_PROVIDED")
        appiumServer = envOrDefault("APPIUM_SERVER", "http://127.0.0.1:4723/wd/hub")


        cidClientId = envOrDefault("CID_CLIENT_ID", "nhs-online")
        cidJwtIssuer = envOrDefault("CITIZEN_ID_JWT_ISSUER", "https://auth.ext.signin.nhs.uk")
        val cidHostname = envOrDefault("CID_HOST", "web.local.bitraft.io")
        cidRedirectUri = envOrDefault("CID_REDIRECT_URI", "http://$cidHostname:3000/auth-return")
        cidNativeRedirectUri = envOrDefault("CID_NATIVE_REDIRECT_URI", "nhsapp://$cidHostname:3000/auth-return")
        cidAuthEndpoint = envOrDefault("CID_AUTH_ENDPOINT", "http://$cidHostname:8080/authorize")
        cidRegisterEndpoint = envOrDefault("CID_REGISTER_ENDPOINT", "http://$cidHostname:8080/register")
        emisApplicationId = envOrDefault("EMIS_APPLICATION_ID", "16C4B8A9-A6B1-4727-80E3-DA0C755CD6E7")
        emisVersion = envOrDefault("EMIS_VERSION", "2.1.0.0")

        organDonation = envOrDefault("ORGAN_DONATION_URL", "https://www.organdonation.nhs.uk/")
        symptomChecker = envOrDefault("SYMPTOM_CHECKER_URL", "https://111.nhs.uk")
        sessionExpiryMinutes = envOrDefault("SESSION_EXPIRY_MINUTES", SESSION_EXPIRY_MINUTES)

        dataPreferencesHost = "http://stubs.local.bitraft.io:8080"
        dataPreferencesPath = "/ndop/createsession"
        dataPreferencesUrl = envOrDefault("DATA_PREFERENCES_URL", dataPreferencesHost + dataPreferencesPath)
    }

    private fun envOrDefault(key: String, defaultValue: String): String {
        return System.getenv(key) ?: defaultValue
                .also { println("$key set as $it") }
    }

    private fun envOrDefault(key: String, defaultValue: Long): Long {
        val result = System.getenv(key)?.toLong() ?: defaultValue
                .also { println("$key set as $it") }

        return result
    }

    companion object {
        val instance: Config by lazy { createConfig() }

        val keyStore: KeyStore = KeyStore(constants.JwkValues.cidKeys)
        val logger: Logger = LoggerFactory.getLogger(Config::class.simpleName)

        fun createConfig():Config{
            return Config()
        }

    }
}
