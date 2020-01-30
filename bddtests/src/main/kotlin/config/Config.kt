package config

private const val SESSION_EXPIRY_MINUTES: Long = 2
private const val MONGODB_DEFAULT_PORT: Long = 27017L

class Config private constructor() {

    var url: String
    var apiBackendUrl: String
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
    var browserstackBrowserResolution: String
    var browserstackTimezone: String
    var browserstackaAppVersion:String
    var browserstackNetworkProfile:String
    var browserstackDeviceName:String
    var browserstackDeviceOSversion:String
    var isNativeAppTestRun:Boolean
    var autoLogin: String
    var appPath: String
    var appiumServer: String
    var sessionExpiryMinutes: Long
    val showPageSourceForXPathQuery: Boolean
    val gpLookupApiKey: String
    val postcodeLookupSearchRadiusKm: String
    var isDockerised: Boolean

    val mongoDbHost: String
    val mongoDbPort: Long
    val usersMongoDbHost: String
    val usersMongoDbPort: Long
    val messagesMongoDbHost: String
    val messagesMongoDbPort: Long

    val nativeUrlSuffix: String

    val accessibilityOutputFolder: String

    init {
        url = envOrDefault("url", "http://web.local.bitraft.io:3000")
        wiremockUrl = envOrDefault("wiremockUrl", "http://stubs.local.bitraft.io:8080")
        cidBackendUrl = envOrDefault("cidBackendUrl", "http://cid.local.bitraft.io:8084")
        apiBackendUrl = envOrDefault("apiBackendUrl", "http://api.local.bitraft.io:8089")
        nodeEnv = envOrDefault("NODE_ENV", "production")
        port = envOrDefault("PORT", "3000")
        isDockerised = envOrDefault("DOCKER",false)

        val browserstackUsername = envOrDefault("BROWSERSTACK_USERNAME", "NOT_PROVIDED")
        browserstackAccessKey = envOrDefault("BROWSERSTACK_ACCESSKEY", "NOT_PROVIDED")
        browserstackUrl = "http://$browserstackUsername:$browserstackAccessKey@hub-cloud.browserstack.com/wd/hub"
        browserstackLocal = envOrDefault("BROWSERSTACK_LOCAL", "true")
        browserstackLocalIdentifier = envOrDefault("BROWSERSTACK_LOCAL_IDENTIFIER","")
        browserstackBrowserResolution = envOrDefault("BROWSERSTACK_BROWSER_RESOLUTION","")
        browserstackaAppVersion = envOrDefault("BROWSERSTACK_APP_VERSION","")
        browserstackNetworkProfile = envOrDefault("BROWSERSTACK_NETWORK_PROFILE","")
        browserstackTimezone = envOrDefault("BROWSERSTACK_TIMEZONE", "UTC")
        showPageSourceForXPathQuery = envOrDefault("XPATH_PAGE_SOURCE", false)
        appPath = envOrDefault("APP_PATH", "NOT_PROVIDED")
        appiumServer = envOrDefault("APPIUM_SERVER", "http://127.0.0.1:4723/wd/hub")
        browserstackDeviceName = envOrDefault("BROWSERSTACK_DEVICE_NAME", "")
        browserstackDeviceOSversion = envOrDefault("BROWSERSTACK_OS_VERSION", "")
        isNativeAppTestRun = envOrDefault("IS_NATIVE_APP_RUN", false)

        cidClientId = envOrDefault("CID_CLIENT_ID", "nhs-online")
        cidJwtIssuer = envOrDefault("CITIZEN_ID_JWT_ISSUER", "https://auth.ext.signin.nhs.uk")
        val cidHostname = envOrDefault("CID_HOST", "stubs.local.bitraft.io")
        val webHostname = envOrDefault("WEB_HOST", "web.local.bitraft.io")
        val appScheme = envOrDefault("APP_SCHEME", "http")
        val nativeAppScheme = envOrDefault("NATIVE_APP_SCHEME", "nhsapp")
        autoLogin = envOrDefault("AUTOLOGIN", "false")
        cidRedirectUri = envOrDefault("CID_REDIRECT_URI", "$appScheme://$webHostname:3000/auth-return")
        cidNativeRedirectUri = envOrDefault("CID_REDIRECT_URI", "$nativeAppScheme://$webHostname:3000/auth-return")
        cidAuthEndpoint = envOrDefault("CID_AUTH_ENDPOINT", "http://$cidHostname:8080/authorize")
        cidRegisterEndpoint = envOrDefault("CID_REGISTER_ENDPOINT", "http://$cidHostname:8080/register")
        emisApplicationId = envOrDefault("EMIS_APPLICATION_ID", "16C4B8A9-A6B1-4727-80E3-DA0C755CD6E7")
        emisVersion = envOrDefault("EMIS_VERSION", "2.1.0.0")

        organDonation = envOrDefault("ORGAN_DONATION_URL", "https://www.organdonation.nhs.uk/")
        updatedOrganDonation = "/organdonation"
        symptomChecker = envOrDefault("SYMPTOM_CHECKER_URL", "https://111.nhs.uk")
        sessionExpiryMinutes = envOrDefault("SESSION_EXPIRY_MINUTES", SESSION_EXPIRY_MINUTES)

        dataPreferencesHost = wiremockUrl
        dataPreferencesPath = "/ndop/createsession"
        dataPreferencesUrl = envOrDefault("DATA_PREFERENCES_URL", dataPreferencesHost + dataPreferencesPath)

        mongoDbHost = envOrDefault("SESSION_MONGO_DATABASE_HOST", "127.0.0.1")
        mongoDbPort = envOrDefault("SESSION_MONGO_DATABASE_PORT", MONGODB_DEFAULT_PORT)

        usersMongoDbHost = envOrDefault("USERS_MONGO_DATABASE_HOST", "127.0.0.1")
        usersMongoDbPort = envOrDefault("USERS_MONGO_DATABASE_PORT", MONGODB_DEFAULT_PORT)

        messagesMongoDbHost = envOrDefault("MESSAGES_MONGO_DATABASE_HOST", "127.0.0.1")
        messagesMongoDbPort = envOrDefault("MESSAGES_MONGO_DATABASE_PORT", MONGODB_DEFAULT_PORT)
        brotherMailerPath = "/brothermailer/signup.ashx"

        gpLookupApiKey = envOrDefault("GP_LOOKUP_API_KEY", "testnhssearchservicekey")
        postcodeLookupSearchRadiusKm = envOrDefault("POSTCODE_LOOKUP_SEARCH_RADIUS_KM", "10");

        accessibilityOutputFolder = envOrDefault("ACCESSIBILITY_OUTPUT_FOLDER", "accessibilityoutput")
        nativeUrlSuffix = envOrDefault("URL_NATIVE_SUFFIX","")
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
