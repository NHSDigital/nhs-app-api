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
               :click-func="viewVisionTestResults"
               :click-param="testResultsPath"/>

    <menu-item id="diagnosis"
               data-purpose="diagnosis"
               :href="diagnosisPath"
               :text="$t('my_record.diagnosis.sectionHeader.default')"
               :click-func="goToUrl"
               :click-param="diagnosisPath" />

    <menu-item id="examinations"
               data-purpose="examinations"
               :href="examinationsPath"
               :text="$t('my_record.examinations.sectionHeader.default')"
               :click-func="goToUrl"
               :click-param="examinationsPath" />

    <menu-item id="procedures"
               data-purpose="procedures"
               :href="proceduresPath"
               :text="$t('my_record.procedures.sectionHeader.default')"
               :click-func="goToUrl"
               :click-param="proceduresPath" />
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import {
  TESTRESULTSDETAIL,
  IMMUNISATIONS,
  HEALTH_CONDITIONS,
  DIAGNOSIS_V2,
  EXAMINATIONS_V2,
  PROCEDURES_V2,
} from '@/lib/routes';

export default {
  name: 'DcrVISIONGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      testResultsPath: TESTRESULTSDETAIL.path,
      immunisationsPath: IMMUNISATIONS.path,
      healthConditionsPath: HEALTH_CONDITIONS.path,
      diagnosisPath: DIAGNOSIS_V2.path,
      examinationsPath: EXAMINATIONS_V2.path,
      proceduresPath: PROCEDURES_V2.path,
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
