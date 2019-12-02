<template>
  <div>
    <menu-item id="immunisations"
               data-purpose="immunisations"
               header-tag="h2"
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
               header-tag="h2"
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
               header-tag="h2"
               :href="testResultsPath"
               :text="$t('my_record.testResults.sectionHeader.default')"
               :click-func="goToUrl"
               :click-param="testResultsPath"/>

    <menu-item id="diagnosis"
               data-purpose="diagnosis"
               header-tag="h2"
               :href="diagnosisPath"
               :text="$t('my_record.diagnosis.sectionHeader.default')"
               :click-func="goToUrl"
               :click-param="diagnosisPath" />

    <menu-item id="examinations"
               data-purpose="examinations"
               header-tag="h2"
               :href="examinationsPath"
               :text="$t('my_record.examinations.sectionHeader.default')"
               :click-func="goToUrl"
               :click-param="examinationsPath" />

    <menu-item id="procedures"
               data-purpose="procedures"
               header-tag="h2"
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
