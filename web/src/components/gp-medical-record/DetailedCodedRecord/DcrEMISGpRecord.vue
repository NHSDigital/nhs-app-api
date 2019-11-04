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
  <!-- put the rest in here -->
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { TESTRESULTS,
  IMMUNISATIONS,
  CONSULTATIONS,
} from '@/lib/routes';

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
    immunisationsPath() {
      return IMMUNISATIONS.path;
    },
    consultationsPath() {
      return CONSULTATIONS.path;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
