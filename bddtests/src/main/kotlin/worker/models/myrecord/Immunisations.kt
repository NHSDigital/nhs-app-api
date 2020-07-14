package worker.models.myrecord

data class Immunisations(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<ImmunisationItem>
)
