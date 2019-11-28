<template>
  <div>
    <menu-item id="test-results"
               data-purpose="test-results"
               :href="testResultsPath"
               :text="$t('my_record.testResults.sectionHeader.tpp')"
               :aria-label="
                 getAriaLabel($t('my_record.testResults.sectionHeader.tpp'),
                              record.testResults.data.length)"
               :click-func="goToUrl"
               :click-param="'/gp-medical-record/test-results'"
               :count="record.testResults.data.length"/>

    <menu-item id="events"
               data-purpose="events"
               :href="eventsPath"
               :text="$t('my_record.consultationsAndEvents.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.consultationsAndEvents.sectionHeader'),
                              record.tppDcrEvents.data.length)"
               :click-func="goToUrl"
               :click-param="eventsPath"
               :count="record.tppDcrEvents.data.length"/>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { TESTRESULTS,
  EVENTS,
} from '@/lib/routes';

export default {
  name: 'DcrTPPGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      testResultsPath: TESTRESULTS.path,
      eventsPath: EVENTS.path,
    };
  },
  methods: {
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
