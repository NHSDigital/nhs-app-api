<template>
  <div>
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isAllergiesAndAdverseReactionsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="ALLERGIESANDADVERSEREACTIONS"
                           :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.allergiesAndAdverseReactions.sectionHeader') }}
    </analytics-tracked-tag>
    <allergies-and-adverse-reactions :is-collapsed="isAllergiesAndAdverseReactionsCollapsed"
                                     :data="myRecord.allergies" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isAcuteMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="ACUTEMEDICATIONS"
                           :text="$t('my_record.acuteMedications.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.acuteMedications.sectionHeader') }}
    </analytics-tracked-tag>
    <medications :is-collapsed="isAcuteMedicationsCollapsed"
                 :data="myRecord.medications.data.acuteMedications"
                 :has-error="myRecord.medications.hasErrored"/>

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isCurrentRepeatMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="CURRENTREPEATMEDICATIONS"
                           :text="$t('my_record.currentRepeatMedications.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.currentRepeatMedications.sectionHeader') }}
    </analytics-tracked-tag>
    <medications :is-collapsed="isCurrentRepeatMedicationsCollapsed"
                 :data="myRecord.medications.data.currentRepeatMedications"
                 :has-error="myRecord.medications.hasErrored" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isDiscontinuedRepeatMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="DISCONTINUEDREPEATMEDICATIONS"
                           :text="$t('my_record.discontinuedRepeatMedications.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.discontinuedRepeatMedications.sectionHeader') }}
    </analytics-tracked-tag>
    <medications :is-collapsed="isDiscontinuedRepeatMedicationsCollapsed"
                 :data="myRecord.medications.data.discontinuedRepeatMedications"
                 :has-error="myRecord.medications.hasErrored" />
  </div>
</template>

<script>

import Medications from '@/components/my-record/SharedComponents/Medications';
import AllergiesAndAdverseReactions from '@/components/my-record/SharedComponents/AllergiesAndAdverseReactions';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

const ALLERGIESANDADVERSEREACTIONS = 'allergiesandadversereactions';
const ACUTEMEDICATIONS = 'acutemedications';
const CURRENTREPEATMEDICATIONS = 'currentrepeatmedications';
const DISCONTINUEDREPEATMEDICATIONS = 'discontinuedrepeatmedications';

export default {
  components: {
    AllergiesAndAdverseReactions,
    Medications,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      ALLERGIESANDADVERSEREACTIONS,
      ACUTEMEDICATIONS,
      CURRENTREPEATMEDICATIONS,
      DISCONTINUEDREPEATMEDICATIONS,
      isAllergiesAndAdverseReactionsCollapsed: true,
      isAcuteMedicationsCollapsed: true,
      isCurrentRepeatMedicationsCollapsed: true,
      isDiscontinuedRepeatMedicationsCollapsed: true,
      myRecord: this.$parent.myRecord,
    };
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case ALLERGIESANDADVERSEREACTIONS:
          this.isAllergiesAndAdverseReactionsCollapsed =
            !this.isAllergiesAndAdverseReactionsCollapsed;
          break;
        case ACUTEMEDICATIONS:
          this.isAcuteMedicationsCollapsed =
            !this.isAcuteMedicationsCollapsed;
          break;
        case CURRENTREPEATMEDICATIONS:
          this.isCurrentRepeatMedicationsCollapsed =
            !this.isCurrentRepeatMedicationsCollapsed;
          break;
        case DISCONTINUEDREPEATMEDICATIONS:
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
