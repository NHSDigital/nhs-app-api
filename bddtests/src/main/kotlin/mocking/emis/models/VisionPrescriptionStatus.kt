package mocking.emis.models

enum class VisionPrescriptionStatus(val value: Int) {
    Processed(1) {
        override fun getDisplayName(): String {
            return "Processed"
        }
    },

    InProgress (-1){
        override fun getDisplayName(): String {
            return "In Progress"
        }
    },

    Rejected(0){
        override fun getDisplayName(): String {
            return "Rejected"
        }
    };

    abstract fun getDisplayName(): String
}

