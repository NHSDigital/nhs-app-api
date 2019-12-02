<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isAllergiesAndAdverseReactionsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="ALLERGIES_AND_ADVERSE_REACTIONS"
                           :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
                           :aria-expanded="!isAllergiesAndAdverseReactionsCollapsed ?
                             'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.allergiesAndAdverseReactions.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <allergies-and-adverse-reactions :is-collapsed="isAllergiesAndAdverseReactionsCollapsed"
                                     :allergies="record.allergies" />

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isAcuteMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="ACUTE_MEDICATIONS"
                           :text="$t('my_record.acuteMedications.sectionHeader')"
                           :aria-expanded="!isAcuteMedicationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.acuteMedications.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <medications :is-collapsed="isAcuteMedicationsCollapsed"
                 :medications="record.medications.data.acuteMedications"
                 :has-error="record.medications.hasErrored"
                 :has-undetermined-access="record.medications.hasUndeterminedAccess" />

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isCurrentRepeatMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="CURRENT_REPEAT_MEDICATIONS"
                           :text="$t('my_record.currentRepeatMedications.sectionHeader')"
                           :aria-expanded="!isCurrentRepeatMedicationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.currentRepeatMedications.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <medications :is-collapsed="isCurrentRepeatMedicationsCollapsed"
                 :medications="record.medications.data.currentRepeatMedications"
                 :has-error="record.medications.data.hasErrored"
                 :has-undetermined-access="record.medications.hasUndeterminedAccess" />

    <analytics-tracked-tag :class="['nhsuk-heading-s',
                                    'nhsuk-u-padding-3',
                                    $style['record-title'],
                                    getCollapsedState(isDiscontinuedRepeatMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="DISCONTINUED_REPEAT_MEDICATIONS"
                           :text="$t('my_record.discontinuedRepeatMedications.sectionHeader')"
                           :aria-expanded="!isDiscontinuedRepeatMedicationsCollapsed ?
                             'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      <h2 class="nhsuk-heading-s nhsuk-u-padding-0 nhsuk-u-margin-0">
        {{ $t('my_record.discontinuedRepeatMedications.sectionHeader') }}</h2>
    </analytics-tracked-tag>
    <medications :is-collapsed="isDiscontinuedRepeatMedicationsCollapsed"
                 :medications="record.medications.data.discontinuedRepeatMedications"
                 :has-error="record.medications.data.hasErrored"
                 :has-undetermined-access="record.medications.hasUndeterminedAccess" />
  </div>
</template>

<script>
import Medications from '@/components/my-record/SharedComponents/Medications';
import AllergiesAndAdverseReactions from '@/components/my-record/SharedComponents/AllergiesAndAdverseReactions';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

const ALLERGIES_AND_ADVERSE_REACTIONS = 'allergiesandadversereactions';
const ACUTE_MEDICATIONS = 'acutemedications';
const CURRENT_REPEAT_MEDICATIONS = 'currentrepeatmedications';
const DISCONTINUED_REPEAT_MEDICATIONS = 'discontinuedrepeatmedications';

export default {
  name: 'ScrMICROTEST',
  components: {
    AllergiesAndAdverseReactions,
    Medications,
    AnalyticsTrackedTag,
  },
  props: {
    record: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      ALLERGIES_AND_ADVERSE_REACTIONS,
      ACUTE_MEDICATIONS,
      CURRENT_REPEAT_MEDICATIONS,
      DISCONTINUED_REPEAT_MEDICATIONS,
      isAllergiesAndAdverseReactionsCollapsed: process.client,
      isAcuteMedicationsCollapsed: process.client,
      isCurrentRepeatMedicationsCollapsed: process.client,
      isDiscontinuedRepeatMedicationsCollapsed: process.client,
    };
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case ALLERGIES_AND_ADVERSE_REACTIONS:
          this.isAllergiesAndAdverseReactionsCollapsed =
            !this.isAllergiesAndAdverseReactionsCollapsed;
          break;
        case ACUTE_MEDICATIONS:
          this.isAcuteMedicationsCollapsed =
            !this.isAcuteMedicationsCollapsed;
          break;
        case CURRENT_REPEAT_MEDICATIONS:
          this.isCurrentRepeatMedicationsCollapsed =
            !this.isCurrentRepeatMedicationsCollapsed;
          break;
        case DISCONTINUED_REPEAT_MEDICATIONS:
          this.isDiscontinuedRepeatMedicationsCollapsed =
            !this.isDiscontinuedRepeatMedicationsCollapsed;
          break;
        default:
          break;
      }
    },
  },
};
</script>


<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
</style>
