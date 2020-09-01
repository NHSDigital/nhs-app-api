<template>
  <div>
    <menu-item id="test-results"
               data-purpose="test-results"
               header-tag="h2"
               :href="testResultsPath"
               :text="$t('myRecord.detailedCodedRecord.testResultsPastSixMonths')"
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.testResultsPastSixMonths'),
                              record.testResults.data.length)"
               :click-func="goToUrl"
               :click-param="testResultsPath"
               :count="record.testResults.data.length"/>

    <menu-item id="events"
               data-purpose="events"
               header-tag="h2"
               :href="eventsPath"
               :text="$t('myRecord.detailedCodedRecord.consultationsAndEvents')"
               :aria-label="
                 getAriaLabel($t('myRecord.detailedCodedRecord.consultationsAndEvents'),
                              record.tppDcrEvents.data.length)"
               :click-func="goToUrl"
               :click-param="eventsPath"
               :count="record.tppDcrEvents.data.length"/>

    <sjr-if journey="documents">
      <menu-item id="documents"
                 data-purpose="documents"
                 :href="documentsPath"
                 header-tag="h2"
                 :text="$t('myRecord.detailedCodedRecord.documents')"
                 :aria-label="
                   getAriaLabel($t('myRecord.detailedCodedRecord.documents'),
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
