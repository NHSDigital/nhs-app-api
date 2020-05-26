<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div :class="[$style.content,
                    'pull-content',
                    !$store.state.device.isNativeApp && $style.desktopWeb]">

        <div id="documentInfo" class="nhsuk-u-margin-bottom-1" data-purpose="info">
          <p v-if="term && isValidFile">{{ dateString }}</p>
          <p v-if="!isValidFile">{{ $t('my_record.documents.documentUnavailableSubtext') }}</p>
        </div>

        <div v-if="hasComments" class="nhsuk-u-padding-bottom-3">
          <b>{{ $t('my_record.documents.commentsHeader') }}</b>
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
                       :text="$t('my_record.documents.actions.view')"
                       :aria-label="$t('my_record.documents.actions.view')"
                       :click-func="navigateToView"/>

            <menu-item v-if="isDownloadable"
                       id="btn_downloadDocument"
                       header-tag="h2"
                       :click-func="startDownload"
                       :text="$t('my_record.documents.actions.download')"
                       :aria-label="$t('my_record.documents.actions.download')"/>
          </menu-item-list>

          <p id="downloadWarning">
            {{ $t('my_record.documents.downloadWarning') }}
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
import { DOCUMENT_DETAIL, DOCUMENTS, GP_MEDICAL_RECORD } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { isBlankString, isEmptyArray, isTruthy, redirectTo, datePart, mimeType } from '@/lib/utils';
import Glossary from '@/components/Glossary';

function updateHeaderText(store, term, isValidFile, datePartString, documentType, dateString) {
  if (isValidFile) {
    store.dispatch('header/updateHeaderText', term || dateString);

    return;
  }

  if (!isBlankString(documentType)) {
    store.dispatch('header/updateHeaderText',
      store.app.i18n.t('my_record.documents.documentTypeUnavailableHeader', {
        date: datePartString,
        type: documentType.toLowerCase(),
      }));

    store.dispatch('pageTitle/updatePageTitle',
      store.app.i18n.t('my_record.documents.documentTypeUnavailablePageTitle', {
        date: datePartString,
        type: documentType.toLowerCase(),
      }));

    return;
  }

  store.dispatch('header/updateHeaderText',
    store.app.i18n.t('my_record.documents.documentUnavailableHeader', { date: datePartString }));
  store.dispatch('pageTitle/updatePageTitle',
    store.app.i18n.t('my_record.documents.documentUnavailablePageTitle', { date: datePartString }));
}

function loadComments(store) {
  const documentComments = get('state.documents.currentDocument.comments', store);

  if (!Array.isArray(documentComments)) {
    return [];
  }

  return documentComments;
}

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
    Glossary,
    DesktopGenericBackLink,
  },

  computed: {
    hasComments() {
      return Array.isArray(this.comments) && !isEmptyArray(this.comments);
    },
    retrieveComments() {
      return this.comments;
    },
    documentsPath: () => DOCUMENTS.path,
  },

  async asyncData({ store, redirect, route }) {
    const date = get('state.documents.currentDocument.date', store);
    const needMoreInformation = get('state.documents.currentDocument.needMoreInformation', store);

    if (!store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning()) {
      redirect(GP_MEDICAL_RECORD.path);
      return {};
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
      dateString = `${documentType} ${store.app.i18n.t('my_record.documents.docTypePageSubtext')} ${datePartString}`;
    } else {
      dateString = `${store.app.i18n.t('my_record.documents.documentPageSubtext')} ${datePartString}`;
    }

    updateHeaderText(
      store,
      term,
      isValidFile,
      datePartString,
      documentType,
      dateString,
    );

    return {
      dateString,
      term,
      type,
      comments,
      size,
      isValidFile,
      isViewable,
      isDownloadable,
    };
  },
  methods: {
    navigateToView() {
      this.$router.push({
        name: DOCUMENT_DETAIL.name,
        params: { id: this.$route.params.id },
      });
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
