package webdrivers.options.device

import org.json.JSONObject
import webdrivers.options.IWebDriverOption
import webdrivers.options.WebDriverOptionGroup
import java.util.*

class DeviceWebDesktop(
        override val source: String = "",
        override val group: WebDriverOptionGroup = WebDriverOptionGroup.DEVICE,
        override val message: Optional<String> = Optional.of(JSONObject("""
            {
                "id": 3,
                "method": "Emulation.setDeviceMetricsOverride",
                "params": {
                    "screenWidth": 1920,
                    "screenHeight": 1080,
                    "mobile": false,
                    "width": 1920,
                    "height": 1080,
                }
            }
            """).toString())) : IWebDriverOption, IDevice