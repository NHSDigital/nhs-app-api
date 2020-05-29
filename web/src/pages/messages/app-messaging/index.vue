<template>
  <div v-if="showTemplate" id="mainDiv">
    <h2>{{ $t('app_messaging.index.subHeader') }}</h2>
    <ul v-if="hasSenderMessages" id="inboxMessages" :class="$style['nhs-app-message']">
      <li v-for="(senderMessage, index) in senderMessages"
          :key="index"
          :class="{
            [$style['nhs-app-message__item']]: true,
            [$style['nhs-app-message__item--unread']]: !!senderMessage.unreadCount
          }">

        <summary-message v-for="(message, messageIndex) in senderMessage.messages"
                         :key="messageIndex"
                         :title="senderMessage.sender"
                         :sub-title="sanitizedContent(message.body)"
                         :date-time="message.sentTime"
                         :unread-count="senderMessage.unreadCount"
                         :aria-label="messageLabel(senderMessage, message)"
                         :href="generateMessageUrl(senderMessage)"
                         :list-index="messageIndex"
                         :has-unread-messages="isUnread(senderMessage)"
                         date-format="DD/MM/YYYY"
                         @click="goToMessages(senderMessage)"/>
      </li>
    </ul>

    <span v-else id="noMessages">{{ $t('app_messaging.index.noMessages') }}</span>
  </div>
</template>

<script>
import { formatDate } from '@/plugins/filters';
import { createUri } from '@/lib/noJs';
import { redirectTo, stripHtml } from '@/lib/utils';
import { HEALTH_INFORMATION_UPDATES_MESSAGES } from '@/lib/routes';
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
  methods: {
    setUpUnreadAnchor(unreadCount) {
      if (unreadCount > 0) {
        return `${HEALTH_INFORMATION_UPDATES_MESSAGES.path}#unreadMessages`;
      }
      return HEALTH_INFORMATION_UPDATES_MESSAGES.path;
    },
    generateMessageUrl(senderMessage) {
      return createUri({
        path: this.setUpUnreadAnchor(senderMessage.unreadCount),
        noJs: { messaging: { selectedSender: senderMessage.sender } },
      });
    },
    goToMessages(senderMessage) {
      this.$store.dispatch('messaging/selectSender', senderMessage.sender);
      redirectTo(this, this.setUpUnreadAnchor(senderMessage.unreadCount));
    },
    messageLabel(senderMessage, message) {
      let label = this.$t('app_messaging.index.hidden.intro')
        .replace('{sender}', senderMessage.sender)
        .replace('{date}', formatDate(message.sentTime, 'DD MMMM YYYY'));

      if (this.unreadCount > 0) {
        label += this.$t('app_messaging.index.hidden.unread')
          .replace('{count}', senderMessage.unreadCount)
          .replace('{plural}', senderMessage.unreadCount > 1 ? 's' : '');
      }
      return label;
    },
    sanitizedContent: stripHtml,
    isUnread: message => message.unreadCount > 0,
  },
};
</script>

<style module lang="scss" scoped>
@import '~nhsuk-frontend/packages/core/settings/breakpoints';
@import '~nhsuk-frontend/packages/core/settings/colours';
@import '~nhsuk-frontend/packages/core/settings/globals';
@import '~nhsuk-frontend/packages/core/settings/spacing';
@import '~nhsuk-frontend/packages/core/tools/sass-mq';
@import '~nhsuk-frontend/packages/core/tools/spacing';
@import '../../../style/arrow';
@import '../../../style/messaging';
</style>
