package mocking.onlineConsultations.constants

class OnlineConsultationConstants {
    companion object {
        /*** SERVICE DEFINITIONS ***/
        const val CONDITION_LIST = "CONDITION_LIST"
        const val BREATHING_PROBLEMS = "BRP_BRP"

        /*** ElementIds ***/
        const val EMERGENCY_ACCEPTANCE_CHECKBOX = "checkbox-ADVICE_EMERGENCY_CONFIRM"
        const val BREATHING_PROBLEMS_CONDITION_ID = "BRP_BRP"
        const val HOW_CAN_WE_HELP_TEXTFIELD = "Q_BRP_BRP_AD_1-text"
        const val ALCOHOL_CHOICE = "Q_GEC_ADM_AD_80-choice-A_GEC_ADM_AD_140"
        const val MALE_GENDER_CHOICE = "PRE_STD_AD_SEX-choice-PRE_STD_AD_SEX_M"
        const val SELF_CHOICE = "checkbox-PRE_STD_AD_SELFONLY_SELF"
        const val EMERGENCY_CHOICE = "PRE_STD_AD_EMERGENCY-choice-PRE_STD_EMERGENCY_YES"
        const val NON_EMERGENCY_CHOICE = "PRE_STD_AD_EMERGENCY-choice-PRE_STD_EMERGENCY_NO"
        const val CONTINUE_BUTTON = "continueButton"
        const val DEMOGRAPHICS_BUTTON = "demographicsContinueButton"
        const val DEMOGRAPHICS_CHECKBOX = "demographics-checkbox"
        const val TERMS_AND_CONDITONS_CHECKBOX = "checkbox-GLO_PRE_DISCLAIMERS_1"
        const val IMAGE = "NHS_ADMIN_AD_REFERRALPAINORIGIN-image"
        const val QUANTITY_NUMBER_INPUT = "NHS_ADMIN_AD_REFERRALPAINDURATION-quantity-quantity"
        const val DATE_OF_BIRTH_INPUT = "PRE_STD_AD_DOB-date"
    }
}