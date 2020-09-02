<template>
  <div>
    <dcr-error-no-access-gp-record
      v-if="showError"
      :has-errored="documents.hasErrored"
      :has-access="documents.hasAccess"/>

    <div v-if="showTemplate" class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <menu-item-list>
          <menu-item v-for="(document, index) in orderedDocuments"
                     :id="document.documentIdentifier"
                     :key="index"
                     header-tag="h2"
                     :target="'_blank'"
                     :text="documentTitle(document.term, document.effectiveDate, document.type)"
                     :click-func="documentClicked"
                     :click-param="document"
                     :description="documentDescription(document.extension, document.size)"
                     :aria-label="`${documentTitle(document.term,
                                                   document.effectiveDate,
                                                   document.type)}.
                                ${documentDescription(document.extension,
                     document.size)}`"/>
        </menu-item-list>
      </div>
    </div>

    <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                            :path="backPath"
                            :button-text="'generic.back'"
                            @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import orderBy from 'lodash/fp/orderBy';
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MenuItem from '@/components/MenuItem';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import MenuItemList from '@/components/MenuItemList';
import { GP_MEDICAL_RECORD_PATH } from '@/router/paths';
import { DOCUMENT_NAME } from '@/router/names';
import { isBlankString, isNumber, redirectTo, readableBytes, datePart } from '@/lib/utils';
import {
  CLINICAL_ABBREVIATIONS_URL,
} from '@/router/externalLinks';

function mapDocumentInfo(document) {
  const {
    name,
    codeId,
    extension,
    effectiveDate,
    term,
    type,
    eventGuid,
    size,
    isValidFile,
    comments,
    needMoreInformation,
  } = document;

  return {
    type: extension,
    name,
    date: effectiveDate,
    codeId,
    term,
    eventGuid,
    size,
    isValidFile,
    comments,
    documentType: type,
    needMoreInformation,
    isViewable: true,
    isDownloadable: true,
  };
}

export default {
  components: {
    DesktopGenericBackLink,
    DcrErrorNoAccessGpRecord,
    MenuItem,
    MenuItemList,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      glossaryLinkURL: CLINICAL_ABBREVIATIONS_URL,
      backPath: GP_MEDICAL_RECORD_PATH,
      documents: null,
    };
  },
  computed: {
    showError() {
      return this.documents && (
        this.documents.hasErrored ||
        this.documents.data.length === 0 ||
        !this.documents.hasAccess);
    },
    orderedDocuments() {
      return orderBy([document => this.getEffectiveDate(document.effectiveDate, '')], ['desc'])((this.documents || {}).data);
    },
  },
  async mounted() {
    if (!['EMIS', 'TPP'].includes(this.$store.state.myRecord.record.supplier)) {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    if (!this.$store.state.myRecord.record.documents) {
      await this.$store.dispatch('myRecord/load');
    }

    this.documents = this.$store.state.myRecord.record.documents;
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    documentDescription(extension, size) {
      if (isBlankString(extension)) {
        return '';
      }

      if (!isNumber(size) || size < 1) {
        return `(${extension.toUpperCase()})`;
      }

      return `(${extension.toUpperCase()}, ${readableBytes(size)})`;
    },
    documentTitle(title, date, type) {
      let dateString;

      if (date && date.value) {
        dateString = datePart(date.value, 'YearMonthDay');
      } else {
        dateString = this.$t('myRecord.unknownDate');
      }

      if (!isBlankString(title)) {
        return `${title} ${this.$t('myRecord.gpMedicalRecord.addedOnDate', { date: dateString })}`;
      }

      if (!isBlankString(type)) {
        return `${type} ${this.$t('myRecord.gpMedicalRecord.addedOnDate', { date: dateString })}`;
      }

      return dateString;
    },
    documentClicked(document) {
      this.$store.dispatch('documents/setSelectedDocumentInfo', mapDocumentInfo(document));
      this.$router.push({
        name: DOCUMENT_NAME,
        params: { id: document.documentIdentifier },
      });
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
  },
};
</script>
