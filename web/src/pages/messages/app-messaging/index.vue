<template>
  <div v-if="showTemplate && loaded" id="mainDiv">
    <div v-if="error">
      <error-container>
        <error-title title="messages.error.messagesError" />
        <error-paragraph from="messages.error.problemGettingMessages" />
        <error-button from="generic.tryAgain" @click="reload" />
      </error-container>
    </div>
    <div v-else>
      <h2>{{ $t('messages.yourMessages') }}</h2>
      <ul v-if="hasSenders" id="inboxMessages" :class="$style['nhs-app-message']">
        <li v-for="(sender, index) in senders"
            :key="index"
            :class="{
              'nhsuk-u-margin-bottom-1': true,
              [$style['nhs-app-message__item']]: true,
              [$style['nhs-app-message__item--unread']]: !!sender.unreadCount
            }">
          <a :title="sender.name"
             :href="messagePath(sender.name)"
             :class="$style['nhs-app-message__link']"
             :aria-label="messageLabel(sender)"
             tabindex="0"
             @click="goToMessages(sender.name)"
             @click.stop.prevent="$emit('click')">
            <div :class="$style['flex-baseline-container']" aria-hidden="true">
              <h2 :class="['nhsuk-heading-xs', $style['nhs-app-message__title']]">
                {{ sender.name }}
              </h2>
              <span v-if="sender.unreadCount" :class="$style['nhs-app-message__meta']">
                <span :id="'unreadIndicator' + index"
                      :class="$style['nhs-app-message__count']"
                >{{ sender.unreadCount }}</span>
              </span>
            </div>
          </a>
        </li>
      </ul>

      <span v-else id="noMessages">{{ $t('messages.youHaveNoMessages') }}</span>

      <desktopGenericBackLink v-if="!isNativeApp"
                              data-purpose="back-link"
                              :path="backLink"
                              @clickAndPrevent="backClicked"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorButton from '@/components/errors/ErrorButton';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import { HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH, MESSAGES_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { toPlainText } from '@/lib/markdown';

export default {
  name: 'AppMessagingPage',
  components: {
    DesktopGenericBackLink,
    ErrorButton,
    ErrorContainer,
    ErrorParagraph,
    ErrorTitle,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      loaded: false,
      isNativeApp: this.$store.state.device.isNativeApp,
      backLink: MESSAGES_PATH,
    };
  },
  computed: {
    error() {
      return this.$store.state.messaging.error;
    },
    hasSenders() {
      return this.senders.length > 0;
    },
    senders() {
      return this.$store.state.messaging.senders;
    },
  },
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await this.loadSenders();
    },
  },
  async created() {
    await this.loadSenders();
  },
  methods: {
    async loadSenders() {
      this.loaded = false;
      this.$store.dispatch('messaging/clear');
      await this.$store.dispatch('messaging/loadSenders');
      this.loaded = true;
    },
    backClicked() {
      redirectTo(this, this.backLink);
    },
    messagePath(sender) {
      return `${HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH}?sender=${sender}`;
    },
    goToMessages(sender) {
      redirectTo(this, HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH, { sender });
    },
    messageLabel(sender) {
      let label = this.$t('messages.messagesFromSender')
        .replace('{sender}', sender.name);

      if (sender.unreadCount > 0) {
        label += this.$t('messages.youHaveCountUnreadMessagePlural')
          .replace('{count}', sender.unreadCount)
          .replace('{plural}', sender.unreadCount > 1 ? 's' : '');
      }
      return label;
    },
    sanitizedContent: toPlainText,
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
@import "@/style/custom/summary-message";
</style>
