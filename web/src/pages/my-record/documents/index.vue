<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <glossary :extra-classes="[$style.glossary]"/>
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
import chunk from 'lodash/fp/chunk';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import DocumentItem from '@/components/my-record/documents/DocumentItem';
import Glossary from '@/components/Glossary';
import { MYRECORD } from '@/lib/routes';
import { isFalsy } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    CardGroup,
    CardGroupItem,
    DocumentItem,
    Glossary,
  },
  data() {
    return {
      documentChunks: chunk(2)((this.$store.state.myRecord.record.documents || {}).data || []),
    };
  },
  async asyncData({ store, redirect }) {
    if (isFalsy(store.app.$env.MY_RECORD_DOCUMENTS_ENABLED)) {
      redirect(MYRECORD.path);
      return;
    }
    if (store.state.myRecord.hasAcceptedTerms && !store.state.myRecord.hasLoaded) {
      await store.dispatch('myRecord/load');
    }
    if (!((store.state.myRecord.record || {}).documents)) {
      redirect(MYRECORD.path);
    }
  },
};
</script>
<style module lang="scss" scoped>
  .glossary {
    padding: 0.5em 1em 0em 1em;
  }
</style>
