<template>
  <div v-if="showTemplate && loaded" id="mainDiv">
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
import { redirectTo, stripHtml } from '@/lib/utils';
import { HEALTH_INFORMATION_UPDATES_MESSAGES_PATH } from '@/router/paths';
import SummaryMessage from '@/components/messaging/SummaryMessage';

export default {
  name: 'AppMessagingPage',
  components: {
    SummaryMessage,
  },
  data() {
    return {
      loaded: false,
    };
  },
  computed: {
    senderMessages() {
      return this.$store.state.messaging.senderMessages;
    },
    hasSenderMessages() {
      return this.senderMessages.length > 0;
    },
  },
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await this.loadMessages();
    },
  },
  async created() {
    await this.loadMessages();
  },
  methods: {
    async loadMessages() {
      await this.$store.dispatch('messaging/load');
      this.loaded = true;
    },
    generateMessageUrl(senderMessage) {
      return senderMessage.unreadCount > 0
        ? `${HEALTH_INFORMATION_UPDATES_MESSAGES_PATH}#unreadMessages`
        : HEALTH_INFORMATION_UPDATES_MESSAGES_PATH;
    },
    goToMessages(senderMessage) {
      this.$store.dispatch('messaging/selectSender', senderMessage.sender);
      redirectTo(this, this.generateMessageUrl(senderMessage.unreadCount));
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
