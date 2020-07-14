package webdrivers.options.nojs


import org.json.JSONObject
import webdrivers.options.IWebDriverOption
import webdrivers.options.WebDriverOptionGroup
import java.util.*


class NoJsOption(
        override val group: WebDriverOptionGroup = WebDriverOptionGroup.NO_JS,
        override val message: Optional<String> = Optional.of(JSONObject("""
            {
                "id": 1,
                "method": "Emulation.setScriptExecutionDisabled",
                "params": {
                    "value": true
                }
            }
            """).toString())) : IWebDriverOption
