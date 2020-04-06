<template>
  <div>
    <li :id="`readMessageReply`+replyIndex"
        :key="`readMessageReply`+replyIndex"
        :class="[$style['nhsuk-panel-group__item'], 'nhsuk-u-padding-bottom-3']">
      <message-panel :index="replyIndex"
                     :id-prefix="replyPrefixIdentifier"
                     :message="message"
                     :message-content="getContent"/>
    </li>
  </div>
</template>
<script>
import MessagePanel from '@/components/patient-practice-messaging/ReceivedMessagePanel';

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

  .nhsuk-panel-group__item {
    padding-right: 15%;
  }

</style>
