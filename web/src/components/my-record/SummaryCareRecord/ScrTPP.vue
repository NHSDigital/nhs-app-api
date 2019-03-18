<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isAllergiesAndAdverseReactionsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="ALLERGIESANDADVERSEREACTIONS"
                           :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
                           :aria-expanded="!isAllergiesAndAdverseReactionsCollapsed ?
                             'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a"
                           tabindex="0">
      {{ $t('my_record.allergiesAndAdverseReactions.sectionHeader') }}
    </analytics-tracked-tag>
    <allergies-and-adverse-reactions :is-collapsed="isAllergiesAndAdverseReactionsCollapsed"
                                     :allergies="record.allergies" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isAcuteMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="ACUTEMEDICATIONS"
                           :text="$t('my_record.acuteMedications.sectionHeader')"
                           :aria-expanded="!isAcuteMedicationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a"
                           tabindex="0">
      {{ $t('my_record.acuteMedications.sectionHeader') }}
    </analytics-tracked-tag>
    <medications :is-collapsed="isAcuteMedicationsCollapsed"
                 :medications="record.medications.data.acuteMedications"
                 :has-error="record.medications.hasErrored"/>

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isCurrentRepeatMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="CURRENTREPEATMEDICATIONS"
                           :text="$t('my_record.currentRepeatMedications.sectionHeader')"
                           :aria-expanded="!isCurrentRepeatMedicationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a"
                           tabindex="0">
      {{ $t('my_record.currentRepeatMedications.sectionHeader') }}
    </analytics-tracked-tag>
    <medications :is-collapsed="isCurrentRepeatMedicationsCollapsed"
                 :medications="record.medications.data.currentRepeatMedications"
                 :has-error="record.medications.hasErrored" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isDiscontinuedRepeatMedicationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="DISCONTINUEDREPEATMEDICATIONS"
                           :text="$t('my_record.discontinuedRepeatMedications.sectionHeader')"
                           :aria-expanded="!isDiscontinuedRepeatMedicationsCollapsed ?
                             'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a"
                           tabindex="0">
      {{ $t('my_record.discontinuedRepeatMedications.sectionHeader') }}
    </analytics-tracked-tag>
    <medications :is-collapsed="isDiscontinuedRepeatMedicationsCollapsed"
                 :medications="record.medications.data.discontinuedRepeatMedications"
                 :has-error="record.medications.hasErrored" />
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
  props: {
    record: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      ALLERGIESANDADVERSEREACTIONS,
      ACUTEMEDICATIONS,
      CURRENTREPEATMEDICATIONS,
      DISCONTINUEDREPEATMEDICATIONS,
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
  @import '../../../style/desktopWeb/accessibility';

  div {
   &.desktopWeb {
    .record-title {
    cursor: pointer;
    &:focus {
      @include outlineStyle
     }
    }
   }
  }

</style>
