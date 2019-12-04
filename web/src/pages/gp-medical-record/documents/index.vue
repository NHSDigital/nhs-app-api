<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="documents.hasErrored"
      :has-access="documents.hasAccess"/>
    <div v-if="showTemplate" class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
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
                           :available="document.isAvailable"
                           :event-guid="document.eventGuid"
                           :code-id="document.codeId"/>
          </card-group-item>
        </card-group>
      </div>
    </div>
    <glossary v-if="!showError"/>
    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'rp03.backButton'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import chunk from 'lodash/fp/chunk';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DocumentItem from '@/components/gp-medical-record/documents/DocumentItem';
import Glossary from '@/components/Glossary';
import { GP_MEDICAL_RECORD } from '@/lib/routes';
import { isFalsy, redirectTo } from '@/lib/utils';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

export default {
  layout: 'nhsuk-layout',
  components: {
    CardGroup,
    CardGroupItem,
    DocumentItem,
    DesktopGenericBackLink,
    Glossary,
    DcrErrorNoAccessGpRecord,
  },
  data() {
    return {
      glossaryLinkURL: this.$store.app.$env.CLINICAL_ABBREVIATIONS_URL,
      backPath: GP_MEDICAL_RECORD.path,
    };
  },
  computed: {
    showError() {
      return (this.documents || {}).hasErrored ||
        (this.documents || {}).data.length === 0 ||
        !(this.documents || {}).hasAccess;
    },
  },
  async asyncData({ store, redirect }) {
    if (!store.state.myRecord.record.documents) {
      await store.dispatch('myRecord/load');
    }

    const { documents } = store.state.myRecord.record;

    if (isFalsy(store.app.$env.MY_RECORD_DOCUMENTS_ENABLED)) {
      redirect(GP_MEDICAL_RECORD.path);
    }

    return {
      documents,
      documentChunks: chunk(2)(documents.data),
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
  },
};
</script>
