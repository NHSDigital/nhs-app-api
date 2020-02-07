<template>
  <div>
    <menu-item id="test-results"
               data-purpose="test-results"
               header-tag="h2"
               :href="testResultsPath"
               :text="$t('my_record.testResults.sectionHeader.tpp')"
               :aria-label="
                 getAriaLabel($t('my_record.testResults.sectionHeader.tpp'),
                              record.testResults.data.length)"
               :click-func="goToUrl"
               :click-param="'/gp-medical-record/test-results'"
               :count="record.testResults.data.length"/>

    <menu-item id="events"
               data-purpose="events"
               header-tag="h2"
               :href="eventsPath"
               :text="$t('my_record.consultationsAndEvents.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.consultationsAndEvents.sectionHeader'),
                              record.tppDcrEvents.data.length)"
               :click-func="goToUrl"
               :click-param="eventsPath"
               :count="record.tppDcrEvents.data.length"/>

    <menu-item v-if="documentsEnabled"
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
import { TESTRESULTS,
  EVENTS,
  DOCUMENTS,
} from '@/lib/routes';
import { isTruthy } from '@/lib/utils';

export default {
  name: 'DcrTPPGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      testResultsPath: TESTRESULTS.path,
      eventsPath: EVENTS.path,
      documentsPath: DOCUMENTS.path,
      documentsEnabled: isTruthy(this.$store.app.$env.MY_RECORD_DOCUMENTS_ENABLED),
    };
  },
  methods: {
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>
