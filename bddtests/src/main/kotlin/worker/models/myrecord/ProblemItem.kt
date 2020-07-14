package worker.models.myrecord

data class ProblemItem (
        val effectiveDate: Date,
        val lineItems: MutableList<ProblemLineItem>
)
