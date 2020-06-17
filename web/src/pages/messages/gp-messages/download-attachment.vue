<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <p id="downloadInformation" class="nhsuk-u-margin-top-3">
        {{ $t('gp_messages.downloadAttachment.downloadWarning') }}
      </p>
      <generic-button id="downloadButton"
                      :button-classes="['nhsuk-button', 'nhsuk-u-margin-top-3']"
                      @click="downloadButtonClicked">
        {{ $t('gp_messages.downloadAttachment.buttonText') }}
      </generic-button>
      <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                              :path="gpMessagePath"
                              @clickAndPrevent="backToMessageClicked"/>
    </div>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { mimeType, datePart, redirectTo } from '@/lib/utils';
import {
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_PATH,
} from '@/router/paths';

export default {
  name: 'GpMessagesDownloadAttachmentPage',
  components: {
    GenericButton,
    DesktopGenericBackLink,
  },
  data() {
    return {
      gpMessagePath: GP_MESSAGES_VIEW_MESSAGE_PATH,
      type: this.$store.state.documents.currentDocument.type,
    };
  },
  computed: {
    getMimeType() {
      return mimeType(this.type);
    },
  },
  created() {
    if (!this.$store.state.documents.currentDocument ||
      !this.$store.state.gpMessages.selectedMessageDetails) {
      redirectTo(this, GP_MESSAGES_PATH);
    }
  },
  methods: {
    backToMessageClicked() {
      this.$router.go(-1);
    },
    async downloadButtonClicked() {
      const { attachmentId } = this.$store.state.gpMessages;
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
