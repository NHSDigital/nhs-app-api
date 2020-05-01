<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
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
  </div>
</template>
<script>
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { GP_MESSAGES_VIEW_MESSAGE, GP_MESSAGES } from '@/lib/routes';
import { mimeType, datePart } from '@/lib/utils';

export default {
  name: 'DownloadAttachment',
  layout: 'nhsuk-layout',
  components: {
    GenericButton,
    DesktopGenericBackLink,
  },
  data() {
    return {
      gpMessagePath: GP_MESSAGES_VIEW_MESSAGE.path,
      type: this.$store.state.documents.currentDocument.type,
    };
  },
  computed: {
    getMimeType() {
      return mimeType(this.type);
    },
  },
  asyncData({ store, redirect }) {
    if (!store.state.documents.currentDocument ||
      !store.state.gpMessages.selectedMessageDetails) {
      redirect(GP_MESSAGES.path);
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
