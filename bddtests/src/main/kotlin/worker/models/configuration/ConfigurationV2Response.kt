package worker.models.configuration

data class ConfigurationV2Response(val minimumSupportedAndroidVersion: String,
                                   val minimumSupportediOSVersion: String,
                                   val fidoServerUrl: String,
                                   val knownServices: List<KnownService>)

