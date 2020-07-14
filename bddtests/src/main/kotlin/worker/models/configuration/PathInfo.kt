package worker.models.configuration

data class PathInfo(val titleKey: String,
                    val accessibleTitleKey: String,
                    val service: String,
                    val allowNativeInteraction: Boolean,
                    val validateSession: Boolean,
                    val path: String,
                    val MenuTab: Int)
