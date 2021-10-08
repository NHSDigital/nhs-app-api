package config

private const val SESSION_EXPIRY_MINUTES: Long = 2
private const val MONGODB_DEFAULT_PORT: Long = 27017L

class Config private constructor() {

    val url: String
    var apiBackendUrl: String
    var cidBackendUrl: String
    val sjrBackendUrl: String
    val usersBackendUrl: String
    val userInfoBackendUrl: String
    val messagesBackendUrl: String
    val logBackendUrl: String
    val wiremockUrl: String

    val cidSettingsUrl: String
    val cidClientId: String
    val cidRedirectUri: String
    val cidJwtIssuer: String
    val emisApplicationId: String
    val emisVersion: String
    val organDonation: String
    val updatedOrganDonation: String
    val symptomChecker: String
    val dataPreferencesPath: String
    val dataPreferencesUrl: String

    val autoLogin: String
    val sessionExpiryMinutes: Long
    val showPageSourceForXPathQuery: Boolean
    val gpLookupApiKey: String
    val isDockerised: Boolean

    val sessionMongoDbHost: String
    val sessionMongoDbPort: Long
    val usersMongoDbHost: String
    val usersMongoDbPort: Long
    val messagesMongoDbHost: String
    val messagesMongoDbPort: Long
    val consentMongoDbHost: String
    val consentMongoDbPort: Long

    val accessibilityOutputFolder: String

    val nhsAppApiKey:String

    val qualtricsDirectoryId:String
    val qualtricsMailingList:String

    val cidGpSessionRedirectUri: String

    init {
        url = envOrDefault("url", "http://web.local.bitraft.io:3000")
        wiremockUrl = envOrDefault("wiremockUrl", "http://stubs.local.bitraft.io:8080")
        apiBackendUrl = envOrDefault("apiBackendUrl", "http://api.local.bitraft.io:8089")
        cidBackendUrl = envOrDefault("cidBackendUrl", "http://cid.local.bitraft.io:8084")
        sjrBackendUrl = envOrDefault("sjrBackendUrl", "http://servicejourneyrulesapi.local.bitraft.io:8086")
        usersBackendUrl = envOrDefault("usersBackendUrl", "http://users.local.bitraft.io:8083")
        userInfoBackendUrl = envOrDefault("userInfoBackendUrl", "http://userinfo.local.bitraft.io:8091")
        messagesBackendUrl = envOrDefault("messagesBackendUrl", "http://messages.local.bitraft.io:8085")
        logBackendUrl = envOrDefault("logBackendUrl", "http://log.local.bitraft.io:8090")
        isDockerised = envOrDefault("DOCKER",false)

        showPageSourceForXPathQuery = envOrDefault("XPATH_PAGE_SOURCE", false)

        cidSettingsUrl = envOrDefault(
                "CID_SETTINGS_URL",
                "http://settings.nhslogin.stubs.local.bitraft.io:8080/citizenid/settings")
        cidClientId = envOrDefault("CID_CLIENT_ID", "nhs-online")
        cidJwtIssuer = envOrDefault(
                "CITIZEN_ID_JWT_ISSUER",
                "http://auth.nhslogin.stubs.local.bitraft.io:8080/citizenid/")
        val webHostname = envOrDefault("WEB_HOST", "web.local.bitraft.io")
        val appScheme = envOrDefault("APP_SCHEME", "http")
        autoLogin = envOrDefault("AUTOLOGIN", "false")
        cidRedirectUri = envOrDefault("CID_REDIRECT_URI", "$appScheme://$webHostname:3000/auth-return")

        cidGpSessionRedirectUri =
                envOrDefault("CID_GP_SESSION_REDIRECT_URI",
                        "$appScheme://$webHostname:3000/on-demand-gp-return")
        
        emisApplicationId = envOrDefault("EMIS_APPLICATION_ID", "16C4B8A9-A6B1-4727-80E3-DA0C755CD6E7")
        emisVersion = envOrDefault("EMIS_VERSION", "2.1.0.0")

        organDonation = envOrDefault("ORGAN_DONATION_URL", "https://www.organdonation.nhs.uk/")
        updatedOrganDonation = "/organdonation"
        symptomChecker = envOrDefault("SYMPTOM_CHECKER_URL", "https://111.nhs.uk")
        sessionExpiryMinutes = envOrDefault("SESSION_EXPIRY_MINUTES", SESSION_EXPIRY_MINUTES)

        val dataPreferencesHost = wiremockUrl
        dataPreferencesPath = "/ndop/createsession"
        dataPreferencesUrl = envOrDefault("DATA_PREFERENCES_URL", dataPreferencesHost + dataPreferencesPath)

        sessionMongoDbHost = envOrDefault("SESSION_MONGO_DATABASE_HOST", "127.0.0.1")
        sessionMongoDbPort = envOrDefault("SESSION_MONGO_DATABASE_PORT", MONGODB_DEFAULT_PORT)

        usersMongoDbHost = envOrDefault("USERS_MONGO_DATABASE_HOST", "127.0.0.1")
        usersMongoDbPort = envOrDefault("USERS_MONGO_DATABASE_PORT", MONGODB_DEFAULT_PORT)

        messagesMongoDbHost = envOrDefault("MESSAGES_MONGO_DATABASE_HOST", "127.0.0.1")
        messagesMongoDbPort = envOrDefault("MESSAGES_MONGO_DATABASE_PORT", MONGODB_DEFAULT_PORT)

        consentMongoDbHost = envOrDefault("CONSENT_MONGO_DATABASE_HOST", "127.0.0.1")
        consentMongoDbPort = envOrDefault("CONSENT_MONGO_DATABASE_PORT", MONGODB_DEFAULT_PORT)

        gpLookupApiKey = envOrDefault("GP_LOOKUP_API_KEY", "testnhssearchservicekey")
        nhsAppApiKey = envOrDefault("NHSAPP_API_KEY", "testnhssearchservicekey")

        accessibilityOutputFolder = envOrDefault("ACCESSIBILITY_OUTPUT_FOLDER", "accessibilityoutput")

        qualtricsDirectoryId = envOrDefault("QUALTRICS_DIRECTORY_ID", "MockQualtricsDirectoryId")
        qualtricsMailingList = envOrDefault("QUALTRICS_MAILING_LIST_ID","MockQualtricsMailingListId")
    }

    private fun envOrDefault(key: String, defaultValue: String): String {
        return (System.getenv(key) ?: defaultValue)
                .also { println("$key set as $it") }
    }

    private fun envOrDefault(key: String, defaultValue: Long): Long {
        return (System.getenv(key)?.toLong() ?: defaultValue)
                .also { println("$key set as $it") }
    }

    private fun envOrDefault(key: String, defaultValue: Boolean): Boolean {
        return (System.getenv(key)?.toBoolean() ?: defaultValue)
                .also { println("$key set as $it") }
    }

    companion object {
        val instance: Config by lazy { createConfig() }

        val keyStore: KeyStore = KeyStore(constants.JwkValues.cidKeys)

        fun createConfig(): Config {
            return Config()
        }

    }
}
