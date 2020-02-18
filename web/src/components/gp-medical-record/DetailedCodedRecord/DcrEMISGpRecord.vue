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
               :click-param="'/gp-medical-record/test-results'"
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

    <menu-item v-if="documentsEnabledForSupplier"
               id="documents"
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
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { isTruthy } from '@/lib/utils';
import { TESTRESULTS,
  IMMUNISATIONS,
  CONSULTATIONS,
  HEALTH_CONDITIONS,
  DOCUMENTS,
} from '@/lib/routes';
import get from 'lodash/fp/get';

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
      documentsPath: DOCUMENTS.path,
    };
  },
  computed: {
    documentsEnabledForSupplier() {
      const documentEnabledSupplierList =
        this.$store.app.$env.MY_RECORD_DOCUMENTS_ENABLED_SUPPLIERS;
      if (isTruthy(this.$store.app.$env.MY_RECORD_DOCUMENTS_ENABLED) &&
        documentEnabledSupplierList !== null) {
        return documentEnabledSupplierList.includes(get('$store.state.myRecord.record.supplier')(this));
      }
      return false;
    },
  },
  methods: {
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>
