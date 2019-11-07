<template>
  <div>
    <menu-item id="events"
               data-purpose="events"
               :href="eventsPath"
               :text="$t('my_record.consultationsAndEvents.sectionHeader')"
               :click-func="goToUrl"
               :click-param="eventsPath"/>

    <menu-item id="test-results"
               data-purpose="test-results"
               :href="testResultsPath"
               :text="$t('my_record.testResults.sectionHeader.tpp')"
               :click-func="goToUrl"
               :click-param="'/gp-medical-record/test-results'"/>
    <!-- put the rest in here -->
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
      testResultsPath: TESTRESULTS.path,
      eventsPath: EVENTS.path,
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
