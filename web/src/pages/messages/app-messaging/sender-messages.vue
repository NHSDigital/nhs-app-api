<template>
  <div v-if="showTemplate && loaded" id="mainDiv">
    <div v-if="error">
      <shutter-container>
        <error-title title="messages.error.cannotShowSenderMessages" />
        <error-paragraph from="messages.error.errorOpeningYourSenderMessages" />
        <error-paragraph from="messages.error.youCanTryOpeningYourSenderMessagesAgain" />
        <error-button from="generic.tryAgain" @click="reload" />
        <p data-purpose="msg-text">{{ ifProblemContinuesText }}</p>
      </shutter-container>
    </div>
    <div v-else>
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <page-title css-class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
            <span class="nhsuk-caption-l nhsuk-u-margin-bottom-0">
              {{ $t('messages.messagesFrom') }}
            </span>
            {{ senderName }}
          </page-title>
        </div>
      </div>
      <div v-if="unreadMessages.length > 0" :class="$style['nhs-app-message__group']">
        <p
          id="unreadCountMessagesTitle"
          :class="[$style['nhs-app-message__title'], $style['nhs-app-message__title--unread']]"
          :aria-label="unreadMessagesAriaLabel"
          role="heading"
          tab-index="0">
          {{ unreadMessagesLabel }}
        </p>
        <ul :id="$style['inboxMessages-unread']" :class="$style['nhs-app-message']">
          <li v-for="(message, index) in unreadMessages"
              :key="index"
              :class="$style['nhs-app-message__item']">
            <a :href="messagePath(message.id)"
               :class="$style['nhs-app-message__link']"
               :aria-label="messageLabel(message)"
               @click.stop.prevent="goToMessage(message.id)">
              <div :class="$style['flex-baseline-container']" aria-hidden="true">
                <span :id="'unreadIndicator' + index"
                      :class="$style['nhs-app-message__pill']" />
                <div :class="$style['flex-column-container']">
                  <formatted-date-time
                    :date-time="message.sentTime"
                    :class="{
                      [$style['nhs-app-message__date']]: true
                    }"
                    summary-time-format />
                  <p id="subject"
                     :class="{
                       'nhsuk-body-s': true,
                       [$style['nhs-app-message__summary']]: true
                     }">
                    {{ toPlainText(message.body) }}
                  </p>
                </div>
              </div>
            </a>
          </li>
        </ul>
      </div>
      <div v-if="readMessages.length > 0"
           :class="[$style['nhs-app-message__group'], unreadMessages.length > 0 ? $style['nhs-app-message__group-spaced'] : '']">
        <p
          id="readMessagesTitle"
          :class="$style['nhs-app-message__title']">Read messages</p>
        <ul :id="$style['inboxMessages-read']" :class="$style['nhs-app-message']">
          <li v-for="(message, index) in readMessages"
              :key="index"
              :class="$style['nhs-app-message__item']">
            <a :href="messagePath(message.id)"
               :class="$style['nhs-app-message__link']"
               :aria-label="messageLabel(message)"
               @click.stop.prevent="goToMessage(message.id)">
              <div :class="$style['flex-baseline-container']" aria-hidden="true">
                <span :class="[$style['nhs-app-message__pill'], $style['nhs-app-message__pill--read']]" />
                <div :class="$style['flex-column-container']">
                  <formatted-date-time
                    :date-time="message.sentTime"
                    :class="{
                      [$style['nhs-app-message__date']]: true
                    }"
                    summary-time-format />
                  <p id="subject"
                     :class="{
                       'nhsuk-body-s': true,
                       [$style['nhs-app-message__summary']]: true
                     }">
                    {{ toPlainText(message.body) }}
                  </p>
                </div>
              </div>
            </a>
          </li>
        </ul>
      </div>

      <desktop-generic-back-link v-if="!isNativeApp"
                                 data-purpose="back-link"
                                 :path="backLink"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorButton from '@/components/errors/ErrorButton';
import ShutterContainer from '@/components/shutters/ShutterContainer';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import FormattedDateTime from '@/components/widgets/FormattedDateTime';
import PageTitle from '@/components/widgets/PageTitle';
import { first, get } from 'lodash/fp';
import { HEALTH_INFORMATION_UPDATES_PATH, HEALTH_INFORMATION_UPDATES_MESSAGE_PATH } from '@/router/paths';
import { formatInboxMessageTime, formatMessageDayWise, redirectTo } from '@/lib/utils';
import { toPlainText } from '@/lib/markdown';

export default {
  name: 'AppMessagingSenderMessagesPage',
  components: {
    DesktopGenericBackLink,
    ErrorButton,
    ShutterContainer,
    ErrorParagraph,
    ErrorTitle,
    FormattedDateTime,
    PageTitle,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
      backLink: HEALTH_INFORMATION_UPDATES_PATH,
      toPlainText,
    };
  },
  computed: {
    loaded() {
      return this.$store.state.messaging.senderMessagesLoaded;
    },
    error() {
      return this.$store.state.messaging.error;
    },
    messages() {
      return get('messages')(first(this.$store.state.messaging.senderMessages)) || [];
    },
    unreadMessages() {
      return this.messages.filter((message) => !message.read) || [];
    },
    readMessages() {
      return this.messages.filter((message) => message.read) || [];
    },
    unreadMessagesLabel() {
      const { unreadCount } = this.$store.state.messaging.senderMessages[0];

      if (unreadCount && unreadCount > 0) {
        return this.$t('messages.youHaveCountUnreadMessagePlural')
          .replace('{count}', unreadCount)
          .replace('{plural}', unreadCount > 1 ? 's' : '')
          .replace('.', '');
      }

      return '';
    },
    unreadMessagesAriaLabel() {
      const { unreadCount } = this.$store.state.messaging.senderMessages[0];

      if (unreadCount && unreadCount > 0) {
        return this.$t('messages.youHaveCountUnreadMessagePluralFromSender')
          .replace('{count}', unreadCount)
          .replace('{plural}', unreadCount > 1 ? 's' : '')
          .replace('{sender}', this.senderName);
      }

      return '';
    },
    ifProblemContinuesText() {
      return this.$t('messages.error.ifTheProblemContinuesContactSenderDirectly', { sender: this.senderName });
    },
    senderName() {
      return get('sender')(first(this.$store.state.messaging.senderMessages)) || '';
    },
  },
  watch: {
    '$route.query.ts': function watchTimestamp() {
      this.loadMessages();
    },
    loaded: function watchLoaded(value) {
      if (value && !this.error && !this.messages.length) {
        redirectTo(this, this.backLink);
      }
    },
  },
  mounted() {
    this.loadMessages();
  },
  methods: {
    loadMessages() {
      this.$store.dispatch('messaging/clear');

      const { senderId } = this.$route.query;

      if (!senderId) {
        redirectTo(this, this.backLink);
        return;
      }

      this.$store.dispatch('messaging/load', { senderId });
    },
    messageLabel(message) {
      let timePrefix = this.$t('generic.on');
      const date = formatInboxMessageTime(message.sentTime, this.$t.bind(this));
      const day = formatMessageDayWise(message.sentTime, this.$t.bind(this));

      if (day === 'Yesterday') {
        timePrefix = '';
      } else if ((date === 'Midday') || (date === 'Midnight') || (day === 'Today')) {
        timePrefix = this.$t('generic.at');
      }

      const labelPath = message.read ? 'messages.readMessageReceived' : 'messages.unreadMessageReceived';

      return this.$t(labelPath, { date, timePrefix, body: toPlainText(message.body) });
    },
    messagePath(messageId) {
      return `${HEALTH_INFORMATION_UPDATES_MESSAGE_PATH}?messageId=${messageId}`;
    },
    goToMessage(messageId) {
      redirectTo(this, HEALTH_INFORMATION_UPDATES_MESSAGE_PATH, { messageId });
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/settings/breakpoints';
  @import '~nhsuk-frontend/packages/core/settings/globals';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '@/style/_arrow';
  @import '@/style/_messaging';
  @import '@/style/custom/sender-messages';
</style>
