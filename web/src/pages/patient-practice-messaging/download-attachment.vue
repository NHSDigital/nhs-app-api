<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p id="downloadInformation" class="nhsuk-u-margin-top-3">
          {{ $t('patient_practice_messaging.downloadAttachment.downloadWarning') }}
        </p>
        <generic-button id="downloadButton"
                        :button-classes="['nhsuk-button', 'nhsuk-u-margin-top-3']"
                        @click="downloadButtonClicked">
          {{ $t('patient_practice_messaging.downloadAttachment.buttonText') }}
        </generic-button>
        <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                :path="patientPracticeMessagePath"
                                @clickAndPrevent="backToMessageClicked"/>
      </div>
    </div>
  </div>
</template>
<script>
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE, PATIENT_PRACTICE_MESSAGING } from '@/lib/routes';
import { mimeType, datePart } from '@/lib/utils';

export default {
  name: 'DownloadAttachment',
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    DesktopGenericBackLink,
  },
  computed: {
    patientPracticeMessagePath() {
      return PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE.path;
    },
    type() {
      return this.$store.state.documents.currentDocument.type;
    },
    getMimeType() {
      return mimeType(this.type);
    },
  },
  asyncData({ store, redirect }) {
    if (!store.state.documents.currentDocument ||
      !store.state.patientPracticeMessaging.selectedMessageDetails) {
      redirect(PATIENT_PRACTICE_MESSAGING.path);
    }
  },
  methods: {
    backToMessageClicked() {
      this.$router.go(-1);
    },
    async downloadButtonClicked() {
      const { attachmentId } = this.$store.state.patientPracticeMessaging;
      const messageDate = datePart(this.$route.params.date, 'YearMonthDay');
      await this.$store.dispatch('documents/downloadDocument',
        { documentIdentifier: attachmentId,
          fileName: `Attachment_${messageDate}`,
          fileExtension: this.type,
          mimeType: this.getMimeType,
          isNative: this.$store.state.device.isNativeApp });
    },
  },
};
</script>
