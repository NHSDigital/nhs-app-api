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

    <menu-item id="consultations"
               data-purpose="consultations"
               header-tag="h2"
               :href="consultationsPath"
               :text="$t('my_record.consultationsAndEvents.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.consultationsAndEvents.sectionHeader'),
                              record.consultations.data.length)"
               :click-func="goToUrl"
               :click-param="consultationsPath"
               :count="record.consultations.data.length"/>
    <sjr-if journey="documents">
      <menu-item id="documents"
                 data-purpose="documents"
                 :href="documentsPath"
                 header-tag="h2"
                 :text="$t('my_record.documents.sectionHeader')"
                 :aria-label="
                   getAriaLabel($t('my_record.documents.sectionHeader'),
                                record.documents.data.length)"
                 :click-func="goToUrl"
                 :click-param="documentsPath"
                 :count="record.documents.data.length"/>
    </sjr-if>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import SjrIf from '@/components/SjrIf';
import {
  HEALTH_CONDITIONS_PATH,
  TESTRESULTS_PATH,
  CONSULTATIONS_PATH,
  IMMUNISATIONS_PATH,
  DOCUMENTS_PATH,
} from '@/router/paths';

export default {
  name: 'DcrEMISGpRecord',
  components: {
    MenuItem,
    SjrIf,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      testResultsPath: TESTRESULTS_PATH,
      immunisationsPath: IMMUNISATIONS_PATH,
      consultationsPath: CONSULTATIONS_PATH,
      healthConditionsPath: HEALTH_CONDITIONS_PATH,
      documentsPath: DOCUMENTS_PATH,
    };
  },
  methods: {
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>
