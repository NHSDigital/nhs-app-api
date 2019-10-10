<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <nhs-arrow-banner :banner-text="$t('rp01.glossary.linkText')"
                        :link-url="glossaryLinkURL"
                        :is-analytics-tracked="true"/>
      <card-group v-for="(documentChunk, chunkIndex) in documentChunks"
                  :key="`document-chunk-${chunkIndex}`"
                  class="nhsuk-grid-row">
        <card-group-item v-for="(document, documentIndex) in documentChunk"
                         :key="`document-${documentIndex}`"
                         class="nhsuk-grid-column-one-half">
          <document-item :id="document.documentGuid"
                         :date="document.effectiveDate"
                         :type="document.extension"
                         :size-in-bytes="document.size"
                         :term="document.term"
                         :name="document.name"
                         :available="document.isAvailable"/>
        </card-group-item>
      </card-group>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import chunk from 'lodash/fp/chunk';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import DocumentItem from '@/components/my-record/documents/DocumentItem';
import NhsArrowBanner from '@/components/widgets/NhsArrowBanner';
import { MYRECORD } from '@/lib/routes';
import { isFalsy } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    CardGroup,
    CardGroupItem,
    DocumentItem,
    NhsArrowBanner,
  },
  data() {
    return {
      glossaryLinkURL: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
    };
  },
  asyncData({ store, redirect }) {
    const documents = get('state.myRecord.record.documents.data', store) || [];
    if (isFalsy(store.app.$env.MY_RECORD_DOCUMENTS_ENABLED) ||
        documents.length === 0) {
      redirect(MYRECORD.path);
    }

    return {
      documentChunks: chunk(2)(documents),
    };
  },
};
</script>
<style module lang="scss" scoped>
  .glossary {
    padding: 0.5em 1em 0em 1em;
  }
</style>
