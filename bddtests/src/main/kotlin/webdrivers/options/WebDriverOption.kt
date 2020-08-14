package webdrivers.options

import net.serenitybdd.core.Serenity
import org.json.JSONObject

/**
 * Class describing options used for the web driver communication with the chrome browser instance.
 *
 * See https://chromedevtools.github.io/devtools-protocol/ for available methods for invocation.
 */
enum class WebDriverOption(val key: String, val message: String) {

    NO_JS("no-js",
            JSONObject("""
            {
                "id": 1,
                "method": "Emulation.setScriptExecutionDisabled",
                "params": {
                    "value": true
                }
            }
            """).toString()) {

        override fun isEnabled() = Serenity.getCurrentSession()
                .getOrDefault(NO_JS.key, false) as Boolean
    };

    /**
     * @return true if this option should be configured at web driver initialisation.
     */
    abstract fun isEnabled(): Boolean
}
