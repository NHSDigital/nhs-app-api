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
                            :button-text="'rp03.backButton'"
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
import { GP_MEDICAL_RECORD, DOCUMENT } from '@/lib/routes';
import { isBlankString, isNumber, redirectTo, readableBytes, datePart } from '@/lib/utils';

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
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    DcrErrorNoAccessGpRecord,
    MenuItem,
    MenuItemList,
  },
  mixins: [ReloadRecordMixin],
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
    orderedDocuments() {
      return orderBy([document => this.getEffectiveDate(document.effectiveDate, '')], ['desc'])(this.documents.data);
    },
  },
  async asyncData({ store }) {
    if (!store.state.myRecord.record.documents) {
      await store.dispatch('myRecord/load');
    }

    return {
      documents: store.state.myRecord.record.documents,
    };
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
        dateString = this.$t('my_record.noStartDate');
      }

      if (!isBlankString(title)) {
        return `${title} ${this.$t('my_record.documents.documentMenuItemTitle', { date: dateString })}`;
      }

      if (!isBlankString(type)) {
        return `${type} ${this.$t('my_record.documents.documentMenuItemTitle', { date: dateString })}`;
      }

      return dateString;
    },
    documentClicked(document) {
      this.$store.dispatch('myRecord/setSelectedDocumentInfo', mapDocumentInfo(document));

      this.$router.push({
        name: DOCUMENT.name,
        params: { id: document.documentIdentifier },
      });
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
  },
};
</script>
