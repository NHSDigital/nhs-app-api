<template>
  <div>
    <menu-item id="test-results"
               data-purpose="test-results"
               :href="testResultsPath"
               :text="$t('my_record.testResults.sectionHeader.default')"
               :click-func="viewVisionTestResults"
               :click-param="testResultsPath"/>

    <menu-item id="immunisations"
               data-purpose="immunisations"
               :href="immunisationsPath"
               :text="$t('my_record.immunisations.sectionHeader')"
               :click-func="goToUrl"
               :click-param="immunisationsPath" />
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { TESTRESULTSDETAIL, IMMUNISATIONS } from '@/lib/routes';
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
  computed: {
    immunisationsPath() {
      return IMMUNISATIONS.path;
    },
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
