package worker.models.myrecord

data class Allergies(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<AllergyItem>
)
