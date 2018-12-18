package config

import org.slf4j.Logger
import org.slf4j.LoggerFactory
import java.net.URI

private const val SESSION_EXPIRY_MINUTES: Long = 2
private const val MONGODB_DEFAULT_PORT: Long = 27017L

class Config private constructor() {

    var url: String
    var pfsBackendUrl: String
    var cidBackendUrl: String
    var wiremockUrl: String
    var nodeEnv: String
    var port: String

    var cidClientId: String
    var cidRedirectUri: String
    var cidAuthEndpoint: String
    val cidRegisterEndpoint: String
    val cidJwtIssuer: String
    var emisApplicationId: String
    var emisVersion: String
    var organDonation: String
    var updatedOrganDonation: String
    var symptomChecker: String
    var dataPreferencesHost: String
    var dataPreferencesPath: String
    var dataPreferencesUrl: String
    var brotherMailerPath: String

    var browserstackAccessKey: String
    var browserstackUrl: String
    var browserstackLocal: String
    var browserstackLocalIdentifier: String
    var autoLogin: String
    var appPath: String
    var appiumServer: String
    var sessionExpiryMinutes: Long
    val showPageSourceForXPathQuery: String
    val gpLookupApiKey: String

    val mongoDbHost: String
    val mongoDbPort: Long

    init {
        url = envOrDefault("url", "http://web.local.bitraft.io:3000")
        val uri = URI(url)
        val wiremockUrlString = if (uri.port == -1) "${uri.scheme}://${uri.host}" else "http://${uri.host}:8080"
        wiremockUrl = envOrDefault("wiremockUrl", wiremockUrlString)
        cidBackendUrl = envOrDefault("cidBackendUrl", "http://cid.local.bitraft.io:8084")
        pfsBackendUrl = envOrDefault("pfsBackendUrl", "http://api.local.bitraft.io:8082")
        nodeEnv = envOrDefault("NODE_ENV", "production")
        port = envOrDefault("PORT", "3000")

        val browserstackUsername = envOrDefault("BROWSERSTACK_USERNAME", "NOT_PROVIDED")
        browserstackAccessKey = envOrDefault("BROWSERSTACK_ACCESSKEY", "NOT_PROVIDED")
        browserstackUrl = "http://$browserstackUsername:$browserstackAccessKey@hub-cloud.browserstack.com/wd/hub"
        browserstackLocal = envOrDefault("BROWSERSTACK_LOCAL", "true")
        browserstackLocalIdentifier = envOrDefault("BROWSERSTACK_LOCAL_IDENTIFIER","")
        showPageSourceForXPathQuery = envOrDefault("XPATH_PAGE_SOURCE", "false")
        appPath = envOrDefault("APP_PATH", "NOT_PROVIDED")
        appiumServer = envOrDefault("APPIUM_SERVER", "http://127.0.0.1:4723/wd/hub")


        cidClientId = envOrDefault("CID_CLIENT_ID", "nhs-online")
        cidJwtIssuer = envOrDefault("CITIZEN_ID_JWT_ISSUER", "https://auth.ext.signin.nhs.uk")
        val cidHostname = envOrDefault("CID_HOST", "stubs.local.bitraft.io")
        val webHostname = envOrDefault("WEB_HOST", "web.local.bitraft.io")
        val appScheme = envOrDefault("APP_SCHEME", "http")
        autoLogin = envOrDefault("AUTOLOGIN", "false")
        cidRedirectUri = envOrDefault("CID_REDIRECT_URI", "$appScheme://$webHostname:3000/auth-return")
        cidAuthEndpoint = envOrDefault("CID_AUTH_ENDPOINT", "http://$cidHostname:8080/authorize")
        cidRegisterEndpoint = envOrDefault("CID_REGISTER_ENDPOINT", "http://$cidHostname:8080/register")
        emisApplicationId = envOrDefault("EMIS_APPLICATION_ID", "16C4B8A9-A6B1-4727-80E3-DA0C755CD6E7")
        emisVersion = envOrDefault("EMIS_VERSION", "2.1.0.0")

        organDonation = envOrDefault("ORGAN_DONATION_URL", "https://www.organdonation.nhs.uk/")
        updatedOrganDonation = "/organ-donation"
        symptomChecker = envOrDefault("SYMPTOM_CHECKER_URL", "https://111.nhs.uk")
        sessionExpiryMinutes = envOrDefault("SESSION_EXPIRY_MINUTES", SESSION_EXPIRY_MINUTES)

        dataPreferencesHost = wiremockUrl
        dataPreferencesPath = "/ndop/createsession"
        dataPreferencesUrl = envOrDefault("DATA_PREFERENCES_URL", dataPreferencesHost + dataPreferencesPath)

        mongoDbHost = envOrDefault("SESSION_MONGO_DATABASE_HOST", "127.0.0.1")
        mongoDbPort = envOrDefault("SESSION_MONGO_DATABASE_PORT", MONGODB_DEFAULT_PORT)
        brotherMailerPath = "/brothermailer/signup"

        gpLookupApiKey = envOrDefault("GP_LOOKUP_API_KEY", "testnhssearchservicekey")
    }

    private fun envOrDefault(key: String, defaultValue: String): String {
        return (System.getenv(key) ?: defaultValue)
                .also { println("$key set as $it") }
    }

    private fun envOrDefault(key: String, defaultValue: Long): Long {

        return (System.getenv(key)?.toLong() ?: defaultValue)
                .also { println("$key set as $it") }
    }

    companion object {
        val instance: Config by lazy { createConfig() }

        val keyStore: KeyStore = KeyStore(constants.JwkValues.cidKeys)
        val logger: Logger = LoggerFactory.getLogger(Config::class.simpleName)

        fun createConfig(): Config {
            return Config()
        }

    }
}
