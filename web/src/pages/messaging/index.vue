<template>
  <div v-if="showTemplate" id="mainDiv">
    <ul v-if="hasSenderMessages" :class="$style['nhs-app-message']">
      <li v-for="(senderMessage, index) in senderMessages"
          :key="index"
          :class="{
            [$style['nhs-app-message__item']]: true,
            [$style['nhs-app-message__item--unread']]: !!senderMessage.unreadCount
          }">
        <summary-message v-for="(message, messageIndex) in senderMessage.messages"
                         :key="messageIndex"
                         :message="message"
                         :sender="senderMessage.sender"
                         :unread-count="senderMessage.unreadCount"/>
      </li>
    </ul>
    <span v-else>{{ $t('messaging.index.noMessages') }}</span>
  </div>
</template>

<script>
import SummaryMessage from '@/components/messaging/SummaryMessage';

export default {
  layout: 'nhsuk-layout',
  components: {
    SummaryMessage,
  },
  data() {
    return {
      senderMessages: this.$store.state.messaging.senderMessages,
    };
  },
  computed: {
    hasSenderMessages() {
      return this.senderMessages.length > 0;
    },
  },
  async fetch({ store }) {
    await store.dispatch('messaging/load');
  },
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/nhsuk';
@import '../../style/arrow';
ul.nhs-app-message {
  list-style: none;
}

.nhs-app-message {
  margin-bottom: 1em;
  border-top: 1px $nhsuk-border-color solid;
  padding-left: nhsuk-spacing(0);
  @include govuk-media-query($until: desktop) {
    margin-left: (-$nhsuk-gutter-half);
    margin-right: (-$nhsuk-gutter-half);
  }
}

.nhs-app-message__item {
  margin-bottom: nhsuk-spacing(0);
  box-sizing: border-box;
  @include icon-arrow-left-white-background;
  position: relative;
  margin-left: nhsuk-spacing(0);
  border-bottom: 1px $nhsuk-border-color solid;
  padding: nhsuk-spacing(0);
}
</style>
