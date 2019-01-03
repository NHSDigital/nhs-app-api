package webdrivers.options

/**
 * Class describing options used for the web driver communication with the chrome browser instance.
 *
 * See https://chromedevtools.github.io/devtools-protocol/ for available methods for invocation.
 */
enum class WebDriverOptionGroup(val group: String) {

    NO_JS("no-js"),
    DEVICE("device"),
}
