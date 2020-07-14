package worker.models.myrecord

data class Referrals (
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<ReferralItem>
)
