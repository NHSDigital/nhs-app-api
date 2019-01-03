package webdrivers.options.device

import webdrivers.options.IWebDriverOption
import webdrivers.options.WebDriverOptionGroup
import java.util.*

class DeviceNativeWebAndroid(
        override val source: String = "android",
        override val group: WebDriverOptionGroup = WebDriverOptionGroup.DEVICE,
        override val message: Optional<String> = Optional.empty()) : IWebDriverOption, IDevice