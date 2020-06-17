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
               :click-param="testResultsPath"
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
  TESTRESULTS_PATH,
  EVENTS_PATH,
  DOCUMENTS_PATH,
} from '@/router/paths';

export default {
  name: 'DcrTPPGpRecord',
  components: {
    MenuItem,
    SjrIf,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      testResultsPath: TESTRESULTS_PATH,
      eventsPath: EVENTS_PATH,
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
