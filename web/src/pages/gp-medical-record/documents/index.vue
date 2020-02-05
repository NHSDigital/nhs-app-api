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
                     :id="document.documentGuid"
                     :key="index"
                     header-tag="h2"
                     :target="'_blank'"
                     :text="documentTitle(document.term, document.effectiveDate)"
                     :click-func="documentClicked"
                     :click-param="document"
                     :description="documentDescription(document.extension, document.size)"
                     :aria-label="`${documentTitle(document.term, document.effectiveDate)}.
                                ${documentDescription(document.extension, document.size)}`"/>
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
import DcrErrorNoAccessGpRecord from '@/components/gp-medical-record/SharedComponents/DCRErrorNoAccessGpRecord';
import { GP_MEDICAL_RECORD, DOCUMENT } from '@/lib/routes';
import { isFalsy, redirectTo, readableBytes, datePart } from '@/lib/utils';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import orderBy from 'lodash/fp/orderBy';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    DcrErrorNoAccessGpRecord,
    MenuItem,
    MenuItemList,
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
    orderedDocuments() {
      return orderBy([document => this.getEffectiveDate(document.effectiveDate, '')], ['desc'])(this.documents.data);
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
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    documentDescription(extension, size) {
      if (size) {
        return `(${extension.toUpperCase()}, ${readableBytes(size)})`;
      }
      return `(${extension.toUpperCase()})`;
    },
    documentTitle(title, date) {
      let dateString;
      if (date && date.value) {
        dateString = datePart(date.value, 'YearMonthDay');
      } else {
        dateString = this.$t('my_record.noStartDate');
      }

      if (title) {
        return `${title} ${this.$t('my_record.documents.documentMenuItemTitle', { date: dateString })}`;
      }
      return dateString;
    },
    documentClicked(document) {
      this.$store.dispatch('myRecord/setSelectedDocumentInfo', {
        type: document.extension,
        name: document.name,
        date: document.effectiveDate,
        codeId: document.codeId,
        term: document.term,
        eventGuid: document.eventGuid,
        size: document.size,
      });
      this.$router.push({ name: DOCUMENT.name, params: { id: document.documentGuid } });
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value ? effectiveDate.value : defaultValue;
    },
  },
};
</script>
