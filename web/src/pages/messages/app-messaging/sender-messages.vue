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
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <page-title css-class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
            <span class="nhsuk-caption-l nhsuk-u-margin-bottom-0">
              {{ $t('messages.messagesFrom') }}
            </span>
            {{ sender }}
          </page-title>
        </div>
      </div>
      <ul :id="$style['inboxMessages']" :class="$style['nhs-app-message']">
        <li v-for="(message, index) in messages"
            :key="index"
            :class="$style['nhs-app-message__item']">
          <a :href="messagePath(message.id)"
             :class="$style['nhs-app-message__link']"
             :aria-label="messageLabel(message)"
             tabindex="0"
             @click.stop.prevent="goToMessage(message.id)">
            <div :class="$style['flex-baseline-container']" aria-hidden="true">
              <p id="subject"
                 :class="{
                   'nhsuk-body-s': true,
                   [$style['nhs-app-message__summary']]: true,
                   [$style['nhs-app-message__summary--unread']]: !message.read
                 }">
                {{ toPlainText(message.body) }}
              </p>
              <div :class="$style['flex-column-container']">
                <formatted-date-time :date-time="message.sentTime"
                                     :class="{
                                       [$style['nhs-app-message__date']]: true,
                                       [$style['nhs-app-message__date--unread']]: !message.read
                                     }"
                                     summary-time-format />
                <span v-if="!message.read" :class="$style['nhs-app-message__meta']">
                  <span :id="'unreadIndicator' + index"
                        :class="$style['nhs-app-message__count']">{{ $t('messages.unread') }}</span>
                </span>
              </div>
            </div>
          </a>
        </li>
      </ul>

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
    ErrorContainer,
    ErrorParagraph,
    ErrorTitle,
    FormattedDateTime,
    PageTitle,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      loaded: false,
      isNativeApp: this.$store.state.device.isNativeApp,
      backLink: HEALTH_INFORMATION_UPDATES_PATH,
      sender: this.$store.state.messaging.selectedSender,
      toPlainText,
    };
  },
  computed: {
    error() {
      return this.$store.state.messaging.error;
    },
    messages() {
      return get('messages')(first(this.$store.state.messaging.senderMessages)) || [];
    },
  },
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await this.loadMessage();
    },
  },
  async created() {
    await this.loadMessage();
  },
  methods: {
    async loadMessage() {
      this.loaded = false;
      this.$store.dispatch('messaging/clear');
      const sender = get('sender')(this.$route.query);

      if (sender) {
        this.$store.dispatch('messaging/selectSender', sender);
        this.sender = sender;
      }

      if (!this.sender) {
        redirectTo(this, this.backLink);
        return;
      }

      await this.$store.dispatch('messaging/load', { sender: this.sender });

      if (!this.error && !this.messages.length) {
        redirectTo(this, this.backLink);
      }

      this.loaded = true;
    },
    backClicked() {
      redirectTo(this, this.backLink);
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
