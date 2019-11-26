<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate" :class="[$style.content,
                                        'pull-content',
                                        !$store.state.device.isNativeApp && $style.desktopWeb]">
        <div id="documentInfo" :class="$style.info" data-purpose="info">
          <p v-if="name">{{ dateString }}</p>
        </div>
        <div v-if="hasComments" :class="$style.comments">
          <b>{{ $t('my_record.documents.commentsHeader') }}</b>
          <div v-for="(comment, index) in retrieveComments"
               :id="'documentComment' + index"
               :key="'Comment'+index"
               :data-purpose="'documentCommentContainer'+index">
            <p :data-purpose="'documentComment'+index">{{ comment }}</p>
          </div>
        </div>
        <menu-item-list data-sid="action-list-menu">
          <menu-item v-if="!isTGAFile"
                     id="btn_viewDocument"
                     :text="$t('my_record.documents.actions.view')"
                     :aria-label="$t('my_record.documents.actions.view')"
                     :click-func="navigateToView"/>
          <menu-item id="btn_downloadDocument"
                     :click-func="startDownload"
                     :text="$t('my_record.documents.actions.download')"
                     :aria-label="$t('my_record.documents.actions.download')"/>
        </menu-item-list>
        <p id="downloadWarning">
          {{ $t('my_record.documents.downloadWarning') }}
        </p>
        <p>
          <glossary/>
        </p>
      </div>
    </div>
  </div>
</template>
<script>
import get from 'lodash/fp/get';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { MY_RECORD_DOCUMENT_DETAIL, MYRECORD } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { isFalsy, datePart } from '@/lib/utils';
import NativeCallbacks from '@/services/native-app';
import Glossary from '@/components/Glossary';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
    Glossary,
  },
  computed: {
    isTGAFile() {
      return this.type === 'tga' || this.type === 'TGA';
    },
    hasComments() {
      return (this.comments !== null && this.comments.length > 0);
    },
    retrieveComments() {
      return this.comments;
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
    const date = get('state.myRecord.document.date.value', store);

    if (isFalsy(store.app.$env.MY_RECORD_DOCUMENTS_ENABLED)
          || (!store.state.myRecord.hasAcceptedTerms && !hasAgreedToMedicalWarning()) || !date
    ) {
      redirect(MYRECORD.path);
      return {};
    }

    const name = get('state.myRecord.document.name', store);
    const dateString = `${store.app.i18n.t('my_record.documents.documentPageSubtext')} ${datePart(date, 'YearMonthDay')}`;
    const type = get('state.myRecord.document.type', store);
    const codeId = get('state.myRecord.document.codeId', store);
    const term = get('state.myRecord.document.term', store);
    const eventGuid = get('state.myRecord.document.eventGuid', store);
    const documentConsultationsWithComments = get('state.myRecord.documentConsultationsWithComments', store);
    const comments = [];

    if (documentConsultationsWithComments !== null
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
    if (name) {
      store.dispatch('header/updateHeaderText', name);
    } else {
      store.dispatch('header/updateHeaderText', dateString);
    }

    return {
      document: store.state.myRecord.document,
      dateString,
      name,
      type,
      comments,
    };
  },
  methods: {
    navigateToView() {
      this.$router.push({
        name: MY_RECORD_DOCUMENT_DETAIL.name,
        params: { id: this.$route.params.id },
      });
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
  @import '../../../style/spacings';
  @import '../../../style/textstyles';

  p {
    font-family: $default_web;
    font-weight: normal;
    margin-bottom: 1em;
  }
  .info {
    font-size: 1em;
    margin-bottom: 1em;
    margin-top: -1em;

    p {
      margin-bottom: 0
    }
  }
  .comments {
    margin-bottom: 1em;
    p {
      margin-bottom: 0;
    }
  }
</style>
