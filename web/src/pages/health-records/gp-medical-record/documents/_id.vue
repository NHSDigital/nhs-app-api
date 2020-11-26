<template>
  <div v-if="showTemplate && !loading" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full" data-purpose="page-content">
      <div :class="[$style.content,
                    'pull-content',
                    !$store.state.device.isNativeApp && $style.desktopWeb]">

        <div id="documentInfo" class="nhsuk-u-margin-bottom-1" data-purpose="info">
          <p v-if="term && isValidFile">{{ dateString }}</p>
          <p v-if="!isValidFile">{{ $t('myRecord.gpMedicalRecord.toAccessContactSurgery') }}</p>
        </div>

        <div v-if="hasComments" class="nhsuk-u-padding-bottom-3">
          <strong>{{ $t('myRecord.gpMedicalRecord.comments') }}</strong>
          <span v-for="(comment, index) in retrieveComments"
                :id="'documentComment' + index"
                :key="'Comment'+index"
                :data-purpose="'documentComment'+index">
            <pre class="'nhsuk-u-font-size-16">{{ comment }}</pre>
          </span>
        </div>

        <template v-if="isValidFile">
          <menu-item-list data-sid="action-list-menu">
            <menu-item v-if="isViewable"
                       id="btn_viewDocument"
                       header-tag="h2"
                       :text="$t('myRecord.gpMedicalRecord.view')"
                       :aria-label="$t('myRecord.gpMedicalRecord.view')"
                       :click-func="navigateToView"/>

            <menu-item v-if="isDownloadable"
                       id="btn_downloadDocument"
                       header-tag="h2"
                       :click-func="startDownload"
                       :text="$t('myRecord.gpMedicalRecord.download')"
                       :aria-label="$t('myRecord.gpMedicalRecord.download')"/>
          </menu-item-list>

          <p id="downloadWarning">
            {{ $t('myRecord.gpMedicalRecord.whenYouDownloadYouBecomeResponsible') }}
          </p>

          <p>
            <glossary id="glossary"/>
          </p>
        </template>

        <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                :path="documentsPath"
                                @clickAndPrevent="backToDocumentsClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { GP_MEDICAL_RECORD_PATH, DOCUMENTS_PATH } from '@/router/paths';
import { DOCUMENT_DETAIL_NAME } from '@/router/names';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { isBlankString, isEmptyArray, isTruthy, redirectTo, createRouteByNameObject, datePart, mimeType } from '@/lib/utils';
import Glossary from '@/components/Glossary';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

function loadComments(store) {
  const documentComments = get('state.documents.currentDocument.comments', store);

  if (!Array.isArray(documentComments)) {
    return [];
  }

  return documentComments;
}

export default {
  components: {
    MenuItem,
    MenuItemList,
    Glossary,
    DesktopGenericBackLink,
  },
  data() {
    return {
      dateString: null,
      term: null,
      type: null,
      comments: null,
      size: null,
      isValidFile: null,
      isViewable: null,
      isDownloadable: null,
      loading: true,
    };
  },
  computed: {
    hasComments() {
      return Array.isArray(this.comments) && !isEmptyArray(this.comments);
    },
    retrieveComments() {
      return this.comments;
    },
    documentsPath: () => DOCUMENTS_PATH,
  },
  async mounted() {
    const store = this.$store;
    const route = this.$route;
    const date = get('state.documents.currentDocument.date', store);
    const needMoreInformation = get('state.documents.currentDocument.needMoreInformation', store);

    if (!store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning()) {
      redirectTo(this, GP_MEDICAL_RECORD_PATH);
      return;
    }

    if (isTruthy(needMoreInformation)) {
      await store.dispatch('documents/loadDocument', { documentIdentifier: route.params.id, updateMetaData: true });
    }

    const size = get('state.documents.currentDocument.size', store);
    const datePartString = (!date || !date.value) ? 'Unknown Date' : datePart(date.value, 'YearMonthDay');
    const term = get('state.documents.currentDocument.term', store);
    const type = get('state.documents.currentDocument.type', store);
    const documentType = get('state.documents.currentDocument.documentType', store);
    const isViewable = get('state.documents.currentDocument.isViewable', store);
    const isDownloadable = get('state.documents.currentDocument.isDownloadable', store);
    const isValidFile = get('state.documents.currentDocument.isValidFile', store);
    const comments = loadComments(store);

    let dateString;

    if (!isBlankString(documentType)) {
      dateString = `${documentType} ${this.$t('myRecord.gpMedicalRecord.addedOn')} ${datePartString}`;
    } else {
      dateString = `${this.$t('myRecord.gpMedicalRecord.documentAddedOn')} ${datePartString}`;
    }

    this.updateHeaderText(
      term,
      isValidFile,
      datePartString,
      documentType,
      dateString,
    );

    this.dateString = dateString;
    this.term = term;
    this.type = type;
    this.comments = comments;
    this.size = size;
    this.isValidFile = isValidFile;
    this.isViewable = isViewable;
    this.isDownloadable = isDownloadable;
    this.loading = false;
  },
  methods: {
    navigateToView() {
      const date = get('state.documents.currentDocument.date', this.$store);
      const datePartString =
        (!date || !date.value) ? 'Unknown Date' : datePart(date.value, 'YearMonthDay');
      const term = get('state.documents.currentDocument.term', this.$store);
      const documentType = get('state.documents.currentDocument.documentType', this.$store);

      let dateString;

      if (!isBlankString(documentType)) {
        dateString =
          `${documentType} ${this.$t('myRecord.gpMedicalRecord.addedOn')} ${datePartString}`;
      } else {
        dateString = `${this.$t('myRecord.gpMedicalRecord.documentAddedOn')} ${datePartString}`;
        dateString.charAt(0).toLowerCase();
      }

      const letterUpperRegex = /^[A-Z]+$/;

      let termToUse = term;

      if (term !== null && term !== '' && letterUpperRegex.test(term.charAt(0))) {
        termToUse = term.charAt(0).toLowerCase() + term.slice(1);
      }

      const headerText = this.$t('navigation.pages.titles.myRecordDocumentsDetail',
        { DocumentName: termToUse || dateString });

      this.$store.dispatch('documents/setViewTitle', headerText);
      this.$router.push(createRouteByNameObject({
        name: DOCUMENT_DETAIL_NAME,
        params: { id: this.$route.params.id },
        store: this.$store,
      }));
    },
    backToDocumentsClicked() {
      redirectTo(this, this.documentsPath, null);
    },
    async startDownload() {
      let fileName;

      if (!isBlankString(this.term)) {
        fileName = this.term;
      } else {
        fileName = this.dateString;
      }

      const fileExtension = this.mapFileTypeToDownloadType(this.type);
      const fileMimeType = mimeType(this.type);
      const isNative = this.$store.state.device.isNativeApp;

      await this.$store.dispatch('documents/downloadDocument',
        { documentIdentifier: this.$route.params.id,
          fileName,
          fileExtension,
          mimeType: fileMimeType,
          isNative });
    },

    // this function should mimic that in backendworker
    // PatientDocumentController#GetPatientDocumentForDownload
    mapFileTypeToDownloadType(fileType) {
      switch ((fileType || '').toLowerCase()) {
        case 'docm':
          return 'doc';
        case 'jfif':
          return 'jpg';
        default:
          return fileType;
      }
    },
    updateHeaderText(term, isValidFile, datePartString, documentType, dateString) {
      if (isValidFile) {
        EventBus.$emit(UPDATE_HEADER, term || dateString, true);
        return;
      }

      let formatArgs;
      let headerKey;
      let titleKey;

      if (!isBlankString(documentType)) {
        formatArgs = { date: datePartString, type: documentType.toLowerCase() };
        headerKey = 'myRecord.gpMedicalRecord.typeAddedOnDateIsNotAvailableThroughTheApp';
        titleKey = 'myRecord.gpMedicalRecord.typeAddedOnDateIsNotAvailable';
      } else {
        formatArgs = { date: datePartString };
        headerKey = 'myRecord.gpMedicalRecord.documentAddedOnDateIsNotAvailableThroughTheApp';
        titleKey = 'myRecord.gpMedicalRecord.documentAddedOnDateIsNotAvailable';
      }
      EventBus.$emit(UPDATE_HEADER, this.$t(headerKey, formatArgs), true);
      EventBus.$emit(UPDATE_TITLE, this.$t(titleKey, formatArgs), true);
    },
  },
};
</script>
<style module lang="scss" scoped>
  @import '../../../../style/textstyles';
  @import '~nhsuk-frontend/packages/core/settings/typography';
  @import '~nhsuk-frontend/packages/core/settings/globals';

  pre {
    font-family: $nhsuk-font;
  }
</style>
