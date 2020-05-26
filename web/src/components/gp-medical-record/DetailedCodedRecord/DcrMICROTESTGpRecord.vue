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
               :aria-label="
                 getAriaLabel($t('my_record.testResults.sectionHeader.default'),
                              record.testResults.data.length)"
               :click-func="goToUrl"
               :click-param="testResultsPath"
               :count="record.testResults.data.length"/>

    <menu-item id="medicalHistory"
               data-purpose="medicalHistory"
               header-tag="h2"
               :href="medicalHistoryPath"
               :text="$t('my_record.medicalHistory.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.medicalHistory.sectionHeader'),
                              record.medicalHistories.data.length)"
               :click-func="goToUrl"
               :click-param="medicalHistoryPath"
               :count="record.medicalHistories.data.length"/>

    <menu-item id="recalls"
               data-purpose="recalls"
               header-tag="h2"
               :href="recallsPath"
               :text="$t('my_record.recalls.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.recalls.sectionHeader'),
                              record.recalls.data.length)"
               :click-func="goToUrl"
               :click-param="recallsPath"
               :count="record.recalls.data.length"/>

    <menu-item id="encounters"
               data-purpose="encounters"
               header-tag="h2"
               :href="encountersPath"
               :text="$t('my_record.encounters.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.encounters.sectionHeader'),
                              record.encounters.data.length)"
               :click-func="goToUrl"
               :click-param="encountersPath"
               :count="record.encounters.data.length"/>

    <menu-item id="referrals"
               data-purpose="referrals"
               header-tag="h2"
               :href="referralsPath"
               :text="$t('my_record.referrals.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.referrals.sectionHeader'),
                              record.referrals.data.length)"
               :click-func="goToUrl"
               :click-param="referralsPath"
               :count="record.referrals.data.length"/>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { IMMUNISATIONS,
  HEALTH_CONDITIONS,
  RECALLS, REFERRALS,
  ENCOUNTERS,
  MEDICAL_HISTORY,
  TESTRESULTS } from '@/lib/routes';

export default {
  name: 'DcrMICROTESTGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      immunisationsPath: IMMUNISATIONS.path,
      healthConditionsPath: HEALTH_CONDITIONS.path,
      recallsPath: RECALLS.path,
      referralsPath: REFERRALS.path,
      encountersPath: ENCOUNTERS.path,
      medicalHistoryPath: MEDICAL_HISTORY.path,
      testResultsPath: TESTRESULTS.path,
    };
  },
  methods: {
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>
