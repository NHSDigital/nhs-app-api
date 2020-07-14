package worker.models.myrecord

data class Encounters (
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<EncounterItem>
)
