package webdrivers.options.device

import webdrivers.options.IWebDriverOption
import webdrivers.options.WebDriverOptionGroup
import java.util.*

class DeviceNativeWebIOS(
        override val source: String = "ios",
        override val group: WebDriverOptionGroup = WebDriverOptionGroup.DEVICE,
        override val message: Optional<String> = Optional.empty()) : IWebDriverOption, IDevice