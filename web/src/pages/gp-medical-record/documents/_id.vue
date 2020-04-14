<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div :class="[$style.content,
                    'pull-content',
                    !$store.state.device.isNativeApp && $style.desktopWeb]">

        <div id="documentInfo" class="nhsuk-u-margin-bottom-1" data-purpose="info">
          <p v-if="name && isValidFile">{{ dateString }}</p>
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
import { getType as lookupMimeType } from 'mime';
import Mime from 'mime/Mime';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { DOCUMENT_DETAIL, DOCUMENTS, GP_MEDICAL_RECORD } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { isBlankString, isEmptyArray, isTruthy, redirectTo, datePart } from '@/lib/utils';
import NativeCallbacks from '@/services/native-app';
import Glossary from '@/components/Glossary';

const customMimeTypes = new Mime({
  'image/bmp': ['dib'],
});

/*
  Edge or IE do not support the File constructor
  IE does not support createObjectURL so need to use msSaveOrOpenBlob
  Trident is used to match IE11 and MSIE is used for IE < 11
  All other relevant browsers support File but do not seem to
  download if the Blob constructor is used.
*/
function buildFileBlob(userAgent, response, fullFileName, mimeType) {
  if (userAgent.match(/Edge/i)) {
    return new Blob([response], fullFileName, { type: mimeType });
  }

  if (userAgent.match(/Trident/i)) {
    const blob = new Blob([response], fullFileName, { type: mimeType });

    window.navigator.msSaveOrOpenBlob(blob, fullFileName);

    return blob;
  }

  if (userAgent.match(/CriOS/i)) {
    const reader = new FileReader();
    const blob = new File([response], fullFileName, { type: mimeType });

    reader.onload = () => {
      window.location.href = reader.result;
    };
    reader.readAsDataURL(blob);

    return blob;
  }

  return new File([response], fullFileName, { type: mimeType });
}

function updateHeaderText(store, name, isValidFile, datePartString, documentType, dateString) {
  if (isValidFile) {
    store.dispatch('header/updateHeaderText', name || dateString);

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
  const documentComments = get('state.myRecord.document.comments', store);

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
    mimeType() {
      return lookupMimeType(this.type) || customMimeTypes.getType(this.type) || 'application/octet-stream';
    },
  },

  async asyncData({ store, redirect, route }) {
    const date = get('state.myRecord.document.date', store);
    const needMoreInformation = get('state.myRecord.document.needMoreInformation', store);

    if (!store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning()) {
      redirect(GP_MEDICAL_RECORD.path);
      return {};
    }

    if (isTruthy(needMoreInformation)) {
      await store.dispatch('myRecord/loadDocument', route.params.id);
    }

    const size = get('state.myRecord.document.size', store);
    const datePartString = (!date || !date.value) ? 'Unknown Date' : datePart(date.value, 'YearMonthDay');
    const name = get('state.myRecord.document.name', store);
    const type = get('state.myRecord.document.type', store);
    const documentType = get('state.myRecord.document.documentType', store);
    const isViewable = get('state.myRecord.document.isViewable', store);
    const isDownloadable = get('state.myRecord.document.isDownloadable', store);
    const isValidFile = get('state.myRecord.document.isValidFile', store);
    const comments = loadComments(store);

    let dateString;

    if (!isBlankString(documentType)) {
      dateString = `${documentType} ${store.app.i18n.t('my_record.documents.docTypePageSubtext')} ${datePartString}`;
    } else {
      dateString = `${store.app.i18n.t('my_record.documents.documentPageSubtext')} ${datePartString}`;
    }

    updateHeaderText(
      store,
      name,
      isValidFile,
      datePartString,
      documentType,
      dateString,
    );

    return {
      document: store.state.myRecord.document,
      dateString,
      name,
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

      if (!isBlankString(this.name)) {
        fileName = this.name;
      } else {
        fileName = this.dateString;
      }

      const fileExtension = this.mapFileTypeToDownloadType(this.type);
      const { mimeType } = this;
      const isNative = this.$store.state.device.isNativeApp;
      const { userAgent } = window.navigator;
      const fullFileName = `${fileName}.${fileExtension}`;

      const response = await this.$store.dispatch('myRecord/downloadDocument',
        { documentIdentifier: this.$route.params.id, fileName });

      const blob = buildFileBlob(userAgent, response, fullFileName, mimeType);

      if (isNative) {
        const fileReader = new FileReader();

        fileReader.readAsDataURL(blob);
        fileReader.onloadend = () => {
          const base64data = fileReader.result;

          NativeCallbacks.startDownload(base64data, fullFileName, mimeType);
        };

        return;
      }

      const link = document.createElement('a');

      link.href = window.URL.createObjectURL(blob);
      link.download = fullFileName;

      document.body.appendChild(link);

      link.click();

      document.body.removeChild(link);
    },
    // this function should mimic that in backendworker
    // PatientDocumentController#GetPatientDocumentForDownload
    mapFileTypeToDownloadType(fileType) {
      switch ((fileType || '').toLowerCase()) {
        case 'docm':
          return 'doc';
        case 'rtf':
          return 'txt';
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
  @import '../../../style/textstyles';
  @import '~nhsuk-frontend/packages/core/settings/typography';
  @import '~nhsuk-frontend/packages/core/settings/globals';

  pre {
    font-family: $nhsuk-font;
  }
</style>
