<template>
  <menu-item id="test-results"
             data-purpose="test-results"
             :href="testResultsPath"
             :text="$t('my_record.testResults.sectionHeader.default')"
             :click-func="goToUrl"
             :click-param="'/gp-medical-record/test-results'"/>

  <!-- put the rest in here -->
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { TESTRESULTS } from '@/lib/routes';

const TESTRESULTSSTRING = 'testresults';

export default {
  name: 'DcrEMISGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      TESTRESULTSSTRING,
    };
  },
  computed: {
    testResultsPath() {
      return TESTRESULTS.path;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
