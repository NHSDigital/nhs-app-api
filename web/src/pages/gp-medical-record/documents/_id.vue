<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div :class="[$style.content,
                    'pull-content',
                    !$store.state.device.isNativeApp && $style.desktopWeb]">
        <div id="documentInfo" class="nhsuk-u-margin-bottom-1" data-purpose="info">
          <p v-if="name && isValid">{{ dateString }}</p>
          <p v-if="!isValid">{{ $t('my_record.documents.documentUnavailableSubtext') }}</p>
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
        <menu-item-list v-if="isValid" data-sid="action-list-menu">
          <menu-item id="btn_viewDocument"
                     header-tag="h2"
                     :text="$t('my_record.documents.actions.view')"
                     :aria-label="$t('my_record.documents.actions.view')"
                     :click-func="navigateToView"/>
          <menu-item id="btn_downloadDocument"
                     header-tag="h2"
                     :click-func="startDownload"
                     :text="$t('my_record.documents.actions.download')"
                     :aria-label="$t('my_record.documents.actions.download')"/>
        </menu-item-list>
        <p v-if="isValid" id="downloadWarning">
          {{ $t('my_record.documents.downloadWarning') }}
        </p>
        <p v-if="isValid">
          <glossary id="glossary"/>
        </p>
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
import { redirectTo, datePart } from '@/lib/utils';
import NativeCallbacks from '@/services/native-app';
import Glossary from '@/components/Glossary';

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
      return (this.comments !== null && this.comments.length > 0);
    },
    retrieveComments() {
      return this.comments;
    },
    isValid() {
      return this.isValidFile;
    },
    documentsPath() {
      return DOCUMENTS.path;
    },
    getMimeType() {
      switch (this.type.toUpperCase()) {
        case 'PDF':
          return 'application/pdf';
        case 'JPG':
        case 'JPE':
        case 'JPEG':
        case 'JFIF':
          return 'image/jpg';
        case 'DOC':
        case 'DOCX':
        case 'DOT':
          return 'application/msword';
        case 'RTF':
          return 'application/rtf';
        case 'TXT':
          return 'text/plain';
        case 'GIF':
        case 'TIF':
        case 'TIFF':
          return 'image/gif';
        case 'BMP':
          return 'image/bmp';
        case 'DIB':
          return 'image/dib';
        case 'TGA':
          return 'image/x-tga';
        case 'PNG':
          return 'image/png';
        default:
          return 'application/octet-stream';
      }
    },
  },
  asyncData({ store, redirect }) {
    const date = get('state.myRecord.document.date', store);

    if (!store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning()) {
      redirect(GP_MEDICAL_RECORD.path);
      return {};
    }

    const datePartString = (!date || !date.value) ? 'Unknown Date' : datePart(date.value, 'YearMonthDay');
    const name = get('state.myRecord.document.name', store);
    const type = get('state.myRecord.document.type', store);
    const documentType = get('state.myRecord.document.documentType', store);
    const dateString = documentType !== null ?
      `${documentType} ${store.app.i18n.t('my_record.documents.docTypePageSubtext')} ${datePartString}` :
      `${store.app.i18n.t('my_record.documents.documentPageSubtext')} ${datePartString}`;
    const codeId = get('state.myRecord.document.codeId', store);
    const term = get('state.myRecord.document.term', store);
    const eventGuid = get('state.myRecord.document.eventGuid', store);
    const isValidFile = get('state.myRecord.document.isValidFile', store);
    const documentConsultationsWithComments = get('state.myRecord.documentConsultationsWithComments', store);
    const comments = [];
    const size = get('state.myRecord.document.size', store);

    if (get('state.myRecord.document.comments', store) !== null) {
      comments.push(get('state.myRecord.document.comments', store));
    } else if (documentConsultationsWithComments !== null
      && documentConsultationsWithComments.length > 0) {
      const documentConsultation = (documentConsultationsWithComments || []).filter(p =>
        p.consultationHeaders.filter(x => x.observationsWithTerm.filter(r => r.codeId === codeId &&
                  r.term === term &&
                  r.eventGuid === eventGuid).length > 0).length > 0);

      if (documentConsultation.length > 0) {
        documentConsultation.forEach((consultation) => {
          consultation.consultationHeaders
            .filter(p => p.header === 'Document')
            .map(x => x.comments).forEach((commentList) => {
              commentList.forEach((comment) => {
                comments.push(comment);
              });
            });
        });
      }
    }

    if (isValidFile) {
      if (name) {
        store.dispatch('header/updateHeaderText', name);
      } else {
        store.dispatch('header/updateHeaderText', dateString);
      }
    } else {
      store.dispatch('header/updateHeaderText',
        store.app.i18n.t('my_record.documents.documentUnavailableHeader', { date: datePartString }));
      store.dispatch('pageTitle/updatePageTitle',
        store.app.i18n.t('my_record.documents.documentUnavailablePageTitle', { date: datePartString }));
    }

    return {
      document: store.state.myRecord.document,
      dateString,
      name,
      type,
      comments,
      size,
      isValidFile,
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
    startDownload() {
      /* eslint-disable func-names */
      const fileExtension = this.mapFileTypeToDownloadType(this.type);
      const mimeType = this.getMimeType;
      const isNative = this.$store.state.device.isNativeApp;
      let fullFileName = '';
      const { userAgent } = window.navigator;

      // Tenarys do not work in IE
      if (this.name !== null) {
        fullFileName = `${this.name}.${fileExtension}`;
      } else {
        fullFileName = `${this.dateString}.${fileExtension}`;
      }

      this.$store.dispatch('myRecord/downloadDocument', this.$route.params.id)
        .then((response) => {
          let blob;
          /*
              Edge or IE do not support the File constructor
              IE does not support createObjectURL so need to use msSaveOrOpenBlob
              Trident is used to match IE11 and MSIE is used for IE < 11
              All other relevant browsers support File but do not seem to
              download if the Blob constructor is used.
          */
          if (userAgent.match(/Edge/i)) {
            blob = new Blob([response], fullFileName, { type: mimeType });
          } else if (userAgent.match(/Trident/i)) {
            blob = new Blob([response], fullFileName, { type: mimeType });
            window.navigator.msSaveOrOpenBlob(blob, fullFileName);
            return;
          } else if (userAgent.match(/CriOS/i)) {
            const reader = new FileReader();
            blob = new File([response], fullFileName, { type: mimeType });
            reader.onload = function () {
              window.location.href = reader.result;
            };
            reader.readAsDataURL(blob);
          } else {
            blob = new File([response], fullFileName, { type: mimeType });
          }
          if (isNative) {
            const fileReader = new FileReader();
            fileReader.readAsDataURL(blob);
            fileReader.onloadend = function () {
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
        });
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
