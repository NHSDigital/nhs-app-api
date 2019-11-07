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

    <menu-item id="healthConditions"
               data-purpose="healthConditions"
               :href="healthConditionsPath"
               :text="$t('my_record.healthConditions.sectionHeader')"
               :click-func="goToUrl"
               :click-param="healthConditionsPath" />
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { TESTRESULTSDETAIL, IMMUNISATIONS, HEALTH_CONDITIONS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'DcrVISIONGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      testResultsPath: TESTRESULTSDETAIL.path,
      immunisationsPath: IMMUNISATIONS.path,
      healthConditionsPath: HEALTH_CONDITIONS.path,
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
