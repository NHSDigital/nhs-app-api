package worker.models.configuration

data class ConfigurationResponse(val isDeviceSupported: Boolean?,
                                 val isThrottlingEnabled: Boolean?,
                                 val fidoServerUrl: String?)

