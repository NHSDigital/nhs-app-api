<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isImmunisationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="IMMUNISATIONS"
                           :text="$t('my_record.immunisations.sectionHeader')"
                           :aria-expanded="!isImmunisationsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.immunisations.sectionHeader') }}
    </analytics-tracked-tag>
    <immunisations :is-collapsed="isImmunisationsCollapsed" :immunisations="record.immunisations" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isProblemsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="PROBLEMS"
                           :text="$t('my_record.healthConditions.sectionHeader')"
                           :aria-expanded="!isProblemsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.healthConditions.sectionHeader') }}
    </analytics-tracked-tag>
    <problems :is-collapsed="isProblemsCollapsed" :problems="record.problems" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isMedicalHistoryCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="MEDICAL_HISTORY"
                           :text="$t('my_record.medicalHistory.sectionHeader')"
                           :aria-expanded="!isMedicalHistoryCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.medicalHistory.sectionHeader') }}
    </analytics-tracked-tag>
    <medicalHistory :is-collapsed="isMedicalHistoryCollapsed"
                    :medical-history="record.medicalHistories" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isRecallsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="RECALLS"
                           :text="$t('my_record.recalls.sectionHeader')"
                           :aria-expanded="!isRecallsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a">
      {{ $t('my_record.recalls.sectionHeader') }}
    </analytics-tracked-tag>
    <recalls :is-collapsed="isRecallsCollapsed" :recalls="record.recalls" />
  </div>
</template>

<script>
import Immunisations from '@/components/my-record/SharedComponents/Immunisations';
import Problems from '@/components/my-record/SharedComponents/Problems';
import MedicalHistory from '@/components/my-record/SharedComponents/MedicalHistory';
import Recalls from '@/components/my-record/SharedComponents/Recalls';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

const IMMUNISATIONS = 'immunisations';
const PROBLEMS = 'problems';
const MEDICAL_HISTORY = 'medicalHistory';
const RECALLS = 'recalls';

export default {
  name: 'DcrMICROTEST',
  components: {
    AnalyticsTrackedTag,
    Immunisations,
    Problems,
    MedicalHistory,
    Recalls,
  },
  props: {
    record: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      IMMUNISATIONS,
      PROBLEMS,
      MEDICAL_HISTORY,
      RECALLS,
      isImmunisationsCollapsed: process.client,
      isProblemsCollapsed: process.client,
      isMedicalHistoryCollapsed: process.client,
      isRecallsCollapsed: process.client,
    };
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case IMMUNISATIONS:
          this.isImmunisationsCollapsed =
            !this.isImmunisationsCollapsed;
          break;
        case PROBLEMS:
          this.isProblemsCollapsed =
            !this.isProblemsCollapsed;
          break;
        case MEDICAL_HISTORY:
          this.isMedicalHistoryCollapsed =
            !this.isMedicalHistoryCollapsed;
          break;
        case RECALLS:
          this.isRecallsCollapsed =
            !this.isRecallsCollapsed;
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
