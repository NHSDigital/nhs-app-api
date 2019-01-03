package webdrivers.options

import java.util.*

interface IWebDriverOption {

    val group: WebDriverOptionGroup
    val message: Optional<String>
}