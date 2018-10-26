<template>
  <div>
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isImmunisationsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="IMMUNISATIONS"
                           :text="$t('my_record.immunisations.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.immunisations.sectionHeader') }}
    </analytics-tracked-tag>
    <immunisations :is-collapsed="isImmunisationsCollapsed" :data="myRecord.immunisations" />

    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isProblemsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="PROBLEMS"
                           :text="$t('my_record.problems.sectionHeader')"
                           data-purpose="accordion"
                           tag="h2">
      {{ $t('my_record.problems.sectionHeader') }}
    </analytics-tracked-tag>
    <problems :is-collapsed="isProblemsCollapsed" :data="myRecord.problems" />

  </div>
</template>

<script>
import Immunisations from '@/components/my-record/SharedComponents/Immunisations';
import Problems from '@/components/my-record/SharedComponents/Problems';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

const IMMUNISATIONS = 'immunisations';
const PROBLEMS = 'problems';

export default {
  components: {
    AnalyticsTrackedTag,
    Immunisations,
    Problems,
  },
  data() {
    return {
      IMMUNISATIONS,
      PROBLEMS,
      isProblemsCollapsed: true,
      isImmunisationsCollapsed: true,
      myRecord: this.$parent.myRecord,
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
