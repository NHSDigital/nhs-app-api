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
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.testResults'),
                              record.testResults.data.length)"
               :click-func="goToUrl"
               :click-param="testResultsPath"
               :count="record.testResults.data.length"/>

    <menu-item id="medicalHistory"
               data-purpose="medicalHistory"
               header-tag="h2"
               :href="medicalHistoryPath"
               :text="$t('myRecord.detailedCodedRecord.medicalHistory')"
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.medicalHistory'),
                              record.medicalHistories.data.length)"
               :click-func="goToUrl"
               :click-param="medicalHistoryPath"
               :count="record.medicalHistories.data.length"/>

    <menu-item id="recalls"
               data-purpose="recalls"
               header-tag="h2"
               :href="recallsPath"
               :text="$t('myRecord.detailedCodedRecord.recalls')"
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.recalls'),
                              record.recalls.data.length)"
               :click-func="goToUrl"
               :click-param="recallsPath"
               :count="record.recalls.data.length"/>

    <menu-item id="encounters"
               data-purpose="encounters"
               header-tag="h2"
               :href="encountersPath"
               :text="$t('myRecord.detailedCodedRecord.encounters')"
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.encounters'),
                              record.encounters.data.length)"
               :click-func="goToUrl"
               :click-param="encountersPath"
               :count="record.encounters.data.length"/>

    <menu-item id="referrals"
               data-purpose="referrals"
               header-tag="h2"
               :href="referralsPath"
               :text="$t('myRecord.detailedCodedRecord.referrals')"
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.referrals'),
                              record.referrals.data.length)"
               :click-func="goToUrl"
               :click-param="referralsPath"
               :count="record.referrals.data.length"/>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import {
  HEALTH_CONDITIONS_PATH,
  IMMUNISATIONS_PATH,
  TESTRESULTS_PATH,
  RECALLS_PATH,
  REFERRALS_PATH,
  ENCOUNTERS_PATH,
  MEDICAL_HISTORY_PATH,
} from '@/router/paths';

export default {
  name: 'DcrMICROTESTGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      immunisationsPath: IMMUNISATIONS_PATH,
      healthConditionsPath: HEALTH_CONDITIONS_PATH,
      recallsPath: RECALLS_PATH,
      referralsPath: REFERRALS_PATH,
      encountersPath: ENCOUNTERS_PATH,
      medicalHistoryPath: MEDICAL_HISTORY_PATH,
      testResultsPath: TESTRESULTS_PATH,
    };
  },
  methods: {
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>
