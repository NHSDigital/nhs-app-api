<template>
  <div>
    <li :id="`readMessageReply`+replyIndex"
        :key="`readMessageReply`+replyIndex"
        :class="[$style['nhsuk-panel-group__item'], 'nhsuk-u-padding-bottom-3']">
      <message-panel :index="replyIndex"
                     :id-prefix="replyPrefixIdentifier"
                     :message="message"
                     :attachment-id="attachmentId"
                     :message-content="getContent"/>
    </li>
  </div>
</template>
<script>
import MessagePanel from '@/components/gp-messages/ReceivedMessagePanel';

export default {
  name: 'ReceivedMessage',
  components: { MessagePanel },
  props: {
    dividerVisible: {
      type: Boolean,
      default: false,
    },
    message: {
      type: Object,
      required: true,
    },
    replyIndex: {
      type: Number,
      required: true,
    },
    replyPrefixIdentifier: {
      type: String,
      required: true,
    },
    messageContent: {
      type: String,
      default: '',
    },
  },
  computed: {
    getContent() {
      return this.messageContent || '';
    },
    attachmentId() {
      return this.message.attachmentId;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/received-message";
</style>
