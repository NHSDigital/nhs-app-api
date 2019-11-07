<template>
  <div>
    <menu-item id="immunisations"
               data-purpose="immunisations"
               :href="immunisationsPath"
               :text="$t('my_record.immunisations.sectionHeader')"
               :click-func="goToUrl"
               :click-param="immunisationsPath" />

    <menu-item id="consultations"
               data-purpose="consultations"
               :href="consultationsPath"
               :text="$t('my_record.consultationsAndEvents.sectionHeader')"
               :click-func="goToUrl"
               :click-param="consultationsPath"/>

    <menu-item id="test-results"
               data-purpose="test-results"
               :href="testResultsPath"
               :text="$t('my_record.testResults.sectionHeader.default')"
               :click-func="goToUrl"
               :click-param="'/gp-medical-record/test-results'"/>

    <menu-item id="healthConditions"
               data-purpose="healthConditions"
               :href="healthConditionsPath"
               :text="$t('my_record.healthConditions.sectionHeader')"
               :click-func="goToUrl"
               :click-param="healthConditionsPath" />
  <!-- put the rest in here -->
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { TESTRESULTS,
  IMMUNISATIONS,
  CONSULTATIONS,
  HEALTH_CONDITIONS,
} from '@/lib/routes';

export default {
  name: 'DcrEMISGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      testResultsPath: TESTRESULTS.path,
      immunisationsPath: IMMUNISATIONS.path,
      consultationsPath: CONSULTATIONS.path,
      healthConditionsPath: HEALTH_CONDITIONS.path,
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
