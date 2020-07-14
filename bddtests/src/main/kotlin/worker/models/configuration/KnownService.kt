package worker.models.configuration

data class KnownService(val titleKey: String,
                        val accessibleTitleKey: String,
                        val service: String,
                        val allowNativeInteraction: Boolean,
                        val url: String,
                        val isExternal: Boolean,
                        val useCustomTabs: Boolean,
                        val pathInfo: List<PathInfo>)
