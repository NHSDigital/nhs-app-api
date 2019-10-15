<template>
  <menu-item id="test-results"
             data-purpose="test-results"
             :href="testResultsPath"
             :text="$t('my_record.testResults.sectionHeader.default')"
             :click-func="viewVisionTestResults"
             :click-param="testResultsPath"/>
  <!-- put the rest in here -->
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { TESTRESULTSDETAIL } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'DcrVISIONGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      testResultsPath: TESTRESULTSDETAIL.path,
    };
  },
  methods: {
    viewVisionTestResults() {
      redirectTo(this, this.testResultsPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
