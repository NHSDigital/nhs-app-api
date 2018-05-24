package config

import org.slf4j.Logger
import org.slf4j.LoggerFactory


class Config private constructor() {

    var url: String
    var backendUrl: String
    var wiremockUrl: String
    var nodeEnv: String
    var port: String

    var cidClientId: String
    var cidRedirectUri: String
    var cidAuthEndpoint: String
    var emisApplicationId: String
    var emisVersion: String
    var organDonation: String
    var symptomChecker: String

    var browserstackAccessKey: String
    var browserstackUrl: String
    var browserstackLocal: String
    var appPath: String
    var appiumServer: String


    init {
        url = envOrDefault("url", "http://localhost:3000")
        wiremockUrl = envOrDefault("wiremockUrl", "http://localhost:8080")
        backendUrl = envOrDefault("backendUrl", "http://localhost:8082")
        nodeEnv = envOrDefault("NODE_ENV", "production")
        port = envOrDefault("PORT", "3000")

        val browserstackUsername = envOrDefault("BROWSERSTACK_USERNAME", "NOT_PROVIDED")
        browserstackAccessKey = envOrDefault("BROWSERSTACK_ACCESSKEY", "NOT_PROVIDED")
        browserstackUrl = "http://$browserstackUsername:$browserstackAccessKey@hub-cloud.browserstack.com/wd/hub"
        browserstackLocal = envOrDefault("BROWSERSTACK_LOCAL", "true")
        appPath = envOrDefault("APP_PATH", "NOT_PROVIDED")
        appiumServer = envOrDefault("APPIUM_SERVER", "http://127.0.0.1:4723/wd/hub")


        cidClientId = envOrDefault("CID_CLIENT_ID", "nhs-online-poc")
        cidRedirectUri = envOrDefault("CID_REDIRECT_URI", "http://localhost:3000/auth-return")
        cidAuthEndpoint = envOrDefault("CID_AUTH_ENDPOINT", "http://localhost:8080/citizenid/cicauth/realms/NHS/protocol/openid-connect/auth")
        emisApplicationId = envOrDefault("EMIS_APPLICATION_ID", "D66BA979-60D2-49AA-BE82-AEC06356E41F")
        emisVersion = envOrDefault("EMIS_VERSION", "2.1.0.0")

        organDonation = envOrDefault("ORGAN_DONATION_URL", "https://www.organdonation.nhs.uk/")
        symptomChecker = envOrDefault("SYMPTOM_CHECKER_URL", "https://111.nhs.uk")
    }

    private fun envOrDefault(key: String, defaultValue: String): String {
        var result = System.getenv(key) ?: defaultValue
                .also { println("$key set as $it") }

        return result
    }

    companion object {
        val instance: Config by lazy { Config() }
        val logger: Logger = LoggerFactory.getLogger(Config::class.simpleName)
    }
}
