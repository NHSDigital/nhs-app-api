<template>
  <div>
    <menu-item id="immunisations"
               data-purpose="immunisations"
               :href="immunisationsPath"
               :text="$t('my_record.immunisations.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.immunisations.sectionHeader'),
                              record.immunisations.data.length)"
               :click-func="goToUrl"
               :click-param="immunisationsPath"
               :count="record.immunisations.data.length"/>

    <menu-item id="healthConditions"
               data-purpose="healthConditions"
               :href="healthConditionsPath"
               :text="$t('my_record.healthConditions.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.healthConditions.sectionHeader'),
                              record.problems.data.length)"
               :click-func="goToUrl"
               :click-param="healthConditionsPath"
               :count="record.problems.data.length"/>

    <menu-item id="test-results"
               data-purpose="test-results"
               :href="testResultsPath"
               :text="$t('my_record.testResults.sectionHeader.default')"
               :aria-label="
                 getAriaLabel($t('my_record.testResults.sectionHeader.default'),
                              record.testResults.data.length)"
               :click-func="goToUrl"
               :click-param="'/gp-medical-record/test-results'"
               :count="record.testResults.data.length"/>

    <menu-item id="consultations"
               data-purpose="consultations"
               :href="consultationsPath"
               :text="$t('my_record.consultationsAndEvents.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.consultationsAndEvents.sectionHeader'),
                              record.consultations.data.length)"
               :click-func="goToUrl"
               :click-param="consultationsPath"
               :count="record.consultations.data.length"/>
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
      record: this.$store.state.myRecord.record,
      testResultsPath: TESTRESULTS.path,
      immunisationsPath: IMMUNISATIONS.path,
      consultationsPath: CONSULTATIONS.path,
      healthConditionsPath: HEALTH_CONDITIONS.path,
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
