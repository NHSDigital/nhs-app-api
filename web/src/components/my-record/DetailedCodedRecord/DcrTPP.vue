<template>
  <div :class="!$store.state.device.isNativeApp && $style.desktopWeb">
    <analytics-tracked-tag :class="[$style['record-title'],
                                    getCollapsedState(isEventsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="EVENTS"
                           :text="$t('my_record.events.sectionHeader')"
                           :aria-expanded="!isEventsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a"
                           tabindex="0">
      {{ $t('my_record.events.sectionHeader') }}
    </analytics-tracked-tag>
    <events :is-collapsed="isEventsCollapsed" :events="record.tppDcrEvents" />

    <analytics-tracked-tag :id="'testResultsHeader'"
                           :class="[$style['record-title'],
                                    getCollapsedState(isTestResultsCollapsed)]"
                           :click-func="myRecordSectionClick"
                           :click-param="TESTRESULTS"
                           :text="$t('my_record.testResults.sectionHeader.tpp')"
                           :aria-expanded="!isTestResultsCollapsed ? 'true' : 'false'"
                           data-purpose="accordion"
                           role="button"
                           tag="a"
                           tabindex="0">
      {{ $t('my_record.testResults.sectionHeader.tpp') }}
    </analytics-tracked-tag>
    <test-results :is-collapsed="isTestResultsCollapsed" :results="record.testResults"
                  :supplier="record.supplier" />
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import TestResults from '@/components/my-record/SharedComponents/TestResults';
import Events from '@/components/my-record/SharedComponents/Events';
import VueScrollTo from 'vue-scrollto';

const TESTRESULTS = 'testresults';
const EVENTS = 'events';

export default {
  name: 'DcrTPP',
  components: {
    AnalyticsTrackedTag,
    Events,
    TestResults,
    VueScrollTo,
  },
  props: {
    record: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      TESTRESULTS,
      EVENTS,
      isTestResultsCollapsed: process.client,
      isEventsCollapsed: process.client,
    };
  },
  mounted() {
    if (this.$route.hash) {
      setTimeout((route) => {
        VueScrollTo.scrollTo(route.hash, 500, { easing: VueScrollTo['ease-in'] });
      }, 500, this.$route);
      this.isTestResultsCollapsed = false;
    }
  },
  methods: {
    getCollapsedState(collapsed) {
      return collapsed ? this.$style.closed : this.$style.opened;
    },
    myRecordSectionClick(section) {
      switch (section) {
        case EVENTS:
          this.isEventsCollapsed =
            !this.isEventsCollapsed;
          break;
        case TESTRESULTS:
          this.isTestResultsCollapsed =
            !this.isTestResultsCollapsed;
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
