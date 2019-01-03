package webdrivers.options.device

import org.json.JSONObject
import webdrivers.options.IWebDriverOption
import webdrivers.options.WebDriverOptionGroup
import java.util.*

class DeviceWebMobile(
        override val source: String = "",
        override val group: WebDriverOptionGroup = WebDriverOptionGroup.DEVICE,
        override val message: Optional<String> = Optional.of(JSONObject("""
            {
                "id": 2,
                "method": "Emulation.setDeviceMetricsOverride",
                "params": {
                    "screenWidth": 412,
                    "screenHeight": 732,
                    "mobile": true,
                    "width": 412,
                    "height": 732,
                    "deviceScaleFactor": 2.625,
                }
            }
            """).toString())): IWebDriverOption, IDevice