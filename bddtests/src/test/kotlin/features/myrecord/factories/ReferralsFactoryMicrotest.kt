package features.myrecord.factories

import mocking.data.myrecord.MyRecordSerenityHelpers
import mocking.microtest.myRecord.MyRecordResponseModel
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.myrecord.ReferralItem

class ReferralsFactoryMicrotest: ReferralsFactory() {

    override fun getExpectedReferrals(): List<ReferralItem> {
        val myRecord = MyRecordSerenityHelpers.MY_RECORD_DATA.getOrFail<MyRecordResponseModel>()
        val referrals = myRecord.referral.data

        val referralItems = referrals.map { item ->
            ReferralItem(
                    recordDate = worker.models.myrecord.Date(
                            value = item.recordDate,
                            datePart = item.recordDate),
                    description = "Reason: " + item.description,
                    speciality = "Speciality: " + item.speciality,
                    ubrn = "UBRN: " + item.ubrn
            )
        }

        val sortedReferrals = referralItems.sortedByDescending { it.recordDate }

        return sortedReferrals
    }

    override fun respondWithCorruptedReponse() {
        mockingClient.forMicrotest.mock {
            myRecord.myRecordRequest(SerenityHelpers.getPatient())
                    .respondWithCorruptedContent("Bad Data")
        }
    }
}
