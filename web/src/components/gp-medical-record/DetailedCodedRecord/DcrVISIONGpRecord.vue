<template>
  <div>
    <menu-item id="immunisations"
               data-purpose="immunisations"
               header-tag="h2"
               :href="immunisationsPath"
               :text="$t('myRecord.detailedCodedRecord.immunisations')"
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.immunisations'),
                              record.immunisations.data.length)"
               :click-func="goToUrl"
               :click-param="immunisationsPath"
               :count="record.immunisations.data.length"/>

    <menu-item id="healthConditions"
               data-purpose="healthConditions"
               header-tag="h2"
               :href="healthConditionsPath"
               :text="$t('myRecord.detailedCodedRecord.healthConditions')"
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.healthConditions'),
                              record.problems.data.length)"
               :click-func="goToUrl"
               :click-param="healthConditionsPath"
               :count="record.problems.data.length"/>

    <menu-item id="test-results"
               data-purpose="test-results"
               header-tag="h2"
               :href="testResultsPath"
               :text="$t('myRecord.detailedCodedRecord.testResults')"
               :click-func="goToUrl"
               :click-param="testResultsPath"/>

    <menu-item id="diagnosis"
               data-purpose="diagnosis"
               header-tag="h2"
               :href="diagnosisPath"
               :text="$t('myRecord.detailedCodedRecord.diagnosis')"
               :click-func="goToUrl"
               :click-param="diagnosisPath" />

    <menu-item id="examinations"
               data-purpose="examinations"
               header-tag="h2"
               :href="examinationsPath"
               :text="$t('myRecord.detailedCodedRecord.examinations')"
               :click-func="goToUrl"
               :click-param="examinationsPath" />

    <menu-item id="procedures"
               data-purpose="procedures"
               header-tag="h2"
               :href="proceduresPath"
               :text="$t('myRecord.detailedCodedRecord.procedures')"
               :click-func="goToUrl"
               :click-param="proceduresPath" />
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import {
  HEALTH_CONDITIONS_PATH,
  IMMUNISATIONS_PATH,
  TESTRESULTSDETAIL_PATH,
  DIAGNOSIS_V2_PATH,
  EXAMINATIONS_V2_PATH,
  PROCEDURES_V2_PATH,
} from '@/router/paths';

export default {
  name: 'DcrVISIONGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      testResultsPath: TESTRESULTSDETAIL_PATH,
      immunisationsPath: IMMUNISATIONS_PATH,
      healthConditionsPath: HEALTH_CONDITIONS_PATH,
      diagnosisPath: DIAGNOSIS_V2_PATH,
      examinationsPath: EXAMINATIONS_V2_PATH,
      proceduresPath: PROCEDURES_V2_PATH,
    };
  },
  methods: {
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>
