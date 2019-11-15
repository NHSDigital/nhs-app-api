<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="showTemplate" :class="[$style.content,
                                        'pull-content',
                                        !$store.state.device.isNativeApp && $style.desktopWeb]">
        <div id="documentInfo" :class="$style.info" data-purpose="info">
          <p v-if="name">{{ dateString }}</p>
        </div>
        <message-dialog message-type="warning" icon-text="Important">
          <message-text id="downloadWarning" :class="$style.warningText">
            {{ $t('my_record.documents.downloadWarning') }}
          </message-text>
        </message-dialog>
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
      </div>
    </div>
  </div>
</template>
<script>
import get from 'lodash/fp/get';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { MY_RECORD_DOCUMENT_DETAIL, MYRECORD } from '@/lib/routes';
import hasAgreedToMedicalWarning from '@/lib/sessionStorage';
import { isFalsy, datePart } from '@/lib/utils';
import NativeCallbacks from '@/services/native-app';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    MenuItemList,
    MessageDialog,
    MessageText,
  },
  computed: {
    isTGAFile() {
      return this.type === 'tga' || this.type === 'TGA';
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

  .info {
    font-size: 1em;
    margin-bottom: 1em;
    margin-top: -1em;
}
</style>
