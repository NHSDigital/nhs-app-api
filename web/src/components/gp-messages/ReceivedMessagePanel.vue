<template>
  <div>
    <div :class="$style['nhsuk-panel-sender']">
      <span :id="idPrefix+`MessageReplySender`+index"
            class="nhsuk-u-font-size-16">{{ message.sender }}</span>
    </div>
    <div :id="idPrefix+`MessageReplyPanel`+index"
         :class="[$style['nhsuk-panel'],
                  'nhsuk-u-margin-top-0',
                  'nhsuk-u-margin-bottom-0',
                  'nhsuk-u-padding-2']">
      <linkify-content class="panel-content nhsuk-u-font-size-19"
                       :content="getContent" tag="p"/>
      <div v-if="fileAttached">
        <strong>
          <span :class="['nhsuk-u-font-size-16','nhsuk-u-margin-bottom-0',
                         'nhsuk-u-padding-top-0', $style.attachmentText]">
            {{ $t('gp_messages.view_details.attachment') }}
          </span>
        </strong>
        <div>
          <desktopGenericBackLink id="viewLink"
                                  :class="[$style['attachmentLink'], 'nhsuk-u-margin-right-2']"
                                  :path="viewAttachmentPath"
                                  button-text="gp_messages.view_details.view"
                                  @clickAndPrevent="viewClicked"/>
          <desktopGenericBackLink id="downloadLink"
                                  :class="$style['attachmentLink']"
                                  :path="downloadAttachmentPath"
                                  button-text="gp_messages.view_details.download"
                                  @clickAndPrevent="downloadClicked"/>
        </div>
      </div>
    </div>
    <p :id="idPrefix+`MessageReplyDateTime`+index"
       class="nhsuk-u-font-size-16 nhsuk-u-margin-bottom-0">
      {{ formattedTime }}
    </p>
  </div>
</template>

<script>
import LinkifyContent from '@/components/widgets/LinkifyContent';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { formatIndividualMessageTime, redirectTo, isBlankString } from '@/lib/utils';
import {
  GP_MESSAGES_VIEW_ATTACHMENT_PATH,
  GP_MESSAGES_DOWNLOAD_ATTACHMENT_PATH,
} from '@/router/paths';
import { GP_MESSAGES_DOWNLOAD_ATTACHMENT_NAME } from '@/router/names';

export default {
  name: 'ReceivedMessagePanel',
  components: {
    LinkifyContent,
    DesktopGenericBackLink,
  },
  props: {
    index: {
      type: Number,
      required: true,
    },
    idPrefix: {
      type: String,
      required: true,
    },
    message: {
      type: Object,
      required: true,
    },
    attachmentId: {
      type: String,
      default: undefined,
    },
    messageContent: {
      type: String,
      default: '',
    },
  },
  data() {
    return {
      viewAttachmentPath: GP_MESSAGES_VIEW_ATTACHMENT_PATH,
      downloadAttachmentPath: GP_MESSAGES_DOWNLOAD_ATTACHMENT_PATH,
    };
  },
  computed: {
    formattedTime() {
      return formatIndividualMessageTime(this.message.sentDateTime, this.$t.bind(this));
    },
    getContent() {
      return this.messageContent || '';
    },
    fileAttached() {
      return !isBlankString(this.attachmentId);
    },
  },
  methods: {
    async loadDocument() {
      await this.$store.dispatch('documents/loadDocument', { documentIdentifier: this.attachmentId, updateMetaData: true });
    },
    async viewClicked() {
      await this.loadDocument();
      redirectTo(this, this.viewAttachmentPath);
    },
    async downloadClicked() {
      await this.loadDocument();
      this.$store.dispatch('gpMessages/setAttachmentId', this.attachmentId);
      this.$router.push({
        name: GP_MESSAGES_DOWNLOAD_ATTACHMENT_NAME,
        params: { date: this.message.sentDateTime },
      });
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/settings/all';
  @import '~nhsuk-frontend/packages/core/tools/all';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/settings/globals';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/components/panel/panel';
  @import "../../style/colours";

  .nhsuk-panel {
    border: 1px solid $border_grey;
    width: fit-content;
  }

  .nhsuk-panel-sender {
    text-align: left;
  }

  .attachmentText {
    color: #425563;
  }

  .attachmentLink {
    display: inline;
  }
</style>
