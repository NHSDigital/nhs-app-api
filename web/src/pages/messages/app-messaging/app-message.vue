<template>
  <div v-if="showTemplate && loaded">
    <div v-if="errorReply">
      <keyword-replies-error :error-count="errorReplyCount"/>
    </div>
    <div v-else-if="error">
      <shutter-container>
        <error-title title="messages.error.cannotShowMessage" />
        <error-paragraph from="messages.error.errorOpeningYourMessage" />
        <error-paragraph from="messages.error.youCanTryOpeningYourMessageAgain" />
        <error-button from="generic.tryAgain" @click="reload" />
        <error-paragraph from="messages.error.ifTheProblemContinues" />
      </shutter-container>
    </div>
    <div v-else>
      <div class="nhsuk-u-visually-hidden" role="status" tabindex="-1"/>
      <div class="nhsuk-grid-column-full">
        <form-error-summary v-if="showErrors"
                            :header-locale-ref="'messages.appMessage.errors.title'"
                            :errors="validationErrorMessage"
                            :errors-ids="validationErrorId"/>
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
            <page-title css-class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-3">
              <span class="nhsuk-caption-l nhsuk-u-margin-bottom-0">
                {{ $t('messages.messageFrom') }}
              </span>
              {{ senderName }}
            </page-title>
          </div>
        </div>

        <div v-if="showSpinner">
          <spinner :always-show="true" />
        </div>
        <div v-else>
          <div :class="$style['message-panel__item']">
            <formatted-date-time :class="$style['message-panel__time']"
                                 :date-time="message.sentTime" />
            <div :class="$style['message-panel__content']">
              <markdown-content v-if="isMarkdown" class="panel-content" :content="message.body"
                                :message-id="message.id" />
              <linkify-content v-else class="panel-content" :content="message.body" tag="p" />
            </div>
          </div>

          <message-reply v-if="hasReplyOptions"
                         :show-options="shouldShowOptions()"
                         :radio-value="getPreviousChoice()"
                         :message-reply="messageReply"
                         :sender-name="senderName"
                         @send_clicked="sendClicked" />

          <desktop-generic-back-link v-if="!isNativeApp"
                                     data-purpose="back-link"
                                     :path="backLink"
                                     @clickAndPrevent="backClicked"/>

        </div>
      </div>
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
import LinkifyContent from '@/components/widgets/LinkifyContent';
import MarkdownContent from '@/components/widgets/MarkdownContent';
import PageTitle from '@/components/widgets/PageTitle';
import get from 'lodash/fp/get';
import { HEALTH_INFORMATION_UPDATES_PATH, HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH, LOGIN_PATH } from '@/router/paths';
import { messageVersion, redirectTo } from '@/lib/utils';
import MessageReply from '@/components/messaging/MessageReply';
import FormErrorSummary from '@/components/FormErrorSummary';
import KeywordRepliesError from '@/components/errors/pages/messages/KeywordRepliesError';
import genericStatus from '@/components/errors/statusCodes/GenericStatusCodes';
import Spinner from '@/components/widgets/Spinner';

export default {
  name: 'AppMessagingAppMessagePage',
  components: {
    DesktopGenericBackLink,
    ErrorButton,
    ShutterContainer,
    ErrorParagraph,
    ErrorTitle,
    FormattedDateTime,
    LinkifyContent,
    MarkdownContent,
    PageTitle,
    MessageReply,
    FormErrorSummary,
    KeywordRepliesError,
    Spinner,
  },
  mixins: [ErrorPageMixin],
  beforeRouteLeave(to, from, next) {
    let shouldContinue = true;

    if (to.path === LOGIN_PATH) {
      next(shouldContinue);
    }

    if (this.$store.getters['pageLeaveWarning/shouldShowLeavingModal']) {
      this.$store.dispatch('pageLeaveWarning/setAttemptedRedirectRoute', to.fullPath);
      this.showModal();

      shouldContinue = false;
    }

    if (shouldContinue && typeof window === 'object') {
      window.onbeforeunload = null;
    }

    next(shouldContinue);
  },
  data() {
    return {
      backLink: HEALTH_INFORMATION_UPDATES_SENDER_MESSAGES_PATH,
      isNativeApp: this.$store.state.device.isNativeApp,
      loaded: false,
      hasValidationErrors: false,
      isCheckbox: false,
      showSpinner: false,
      polling: null,
      messageReplyIntervalTime: this.$store.$env.MESSAGE_REPLY_INTERVAL_TIME,
    };
  },
  computed: {
    error() {
      return this.$store.state.messaging.error;
    },
    errorReply() {
      return this.$store.state.messaging.errorReply && this.$store.state.messaging.errorReply.status !== genericStatus.NO_CONTENT;
    },
    errorReplyCount() {
      return this.$store.state.messaging.errorReplyCount;
    },
    isMarkdown() {
      return get('version')(this.message) === messageVersion.Markdown;
    },
    isUnread() {
      return get('read')(this.message) === false;
    },
    message() {
      return this.$store.state.messaging.message;
    },
    messageReply() {
      return get('reply')(this.message);
    },
    noInternet() {
      return this.$store.state.messaging.errorReply && this.$store.state.messaging.errorReply.status === '';
    },
    messageStatus() {
      return get('status')(this.messageReply);
    },
    messageId() {
      return get('messageId')(this.$route.query);
    },
    responseSentDt() {
      return get('responseSentDateTime')(this.messageReply);
    },
    responseCompleteDt() {
      return get('responseCompletedDateTime')(this.messageReply);
    },
    hasReplyOptions() {
      if (get('options')(this.messageReply) === null || get('options')(this.messageReply) === undefined) {
        return false;
      }

      return get('options')(this.messageReply).length > 0;
    },
    sender() {
      return get('senderId')(this.message);
    },
    senderName() {
      return get('sender')(this.message) || '';
    },
    showErrors() {
      return this.hasValidationErrors;
    },
    validationErrorMessage() {
      return (this.isCheckbox) ?
        this.$t('messages.appMessage.errors.checkbox.message') :
        this.$t('messages.appMessage.errors.radioButton.message');
    },
    validationErrorId() {
      return (this.isCheckbox) ? 'checkboxOptions' : 'radioOptions';
    },
  },
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await this.loadMessage();
    },
    isUnread: async function watchRead(value) {
      if (value) {
        this.$store.dispatch('messaging/markAsRead', this.message.id);
      }
    },
  },
  async created() {
    await this.loadMessage();
  },
  methods: {
    async loadMessage(shouldSkipGettingMessage = false) {
      this.loaded = false;

      if (shouldSkipGettingMessage) {
        this.$store.dispatch('messaging/clearAllExceptMessageObj');
      } else {
        this.$store.dispatch('messaging/clear');
      }

      this.$store.dispatch('pageLeaveWarning/reset');
      clearInterval(this.polling);

      const { messageId } = this;

      if (!messageId) {
        redirectTo(this, HEALTH_INFORMATION_UPDATES_PATH);
        return;
      }

      if (!shouldSkipGettingMessage) {
        await this.$store.dispatch('messaging/loadMessage', { messageId });
      }

      if (!this.error && !this.message) {
        redirectTo(this, HEALTH_INFORMATION_UPDATES_PATH);
        return;
      }

      this.loaded = true;
      this.showSpinner = false;
    },
    backClicked() {
      redirectTo(this, this.backLink, { senderId: this.sender });
    },
    async sendClicked(response, isValid, isCheckbox) {
      await this.$store.dispatch('messaging/setPreviousChoice', response);

      this.hasValidationErrors = !isValid;
      this.isCheckbox = isCheckbox;

      if (isValid) {
        await this.$store.dispatch('messaging/recordMessageResponse', { messageId: this.message.id, response });

        if (this.errorReplyCount > 1) {
          this.$store.dispatch('pageLeaveWarning/reset');
        }

        if (this.noInternet) {
          this.$store.dispatch('pageLeaveWarning/reset');
          redirectTo(this, HEALTH_INFORMATION_UPDATES_PATH);
        }

        if (!this.errorReply) {
          await this.pollStatus();
        }
      }
    },
    beforeDestroy() {
      this.$store.dispatch('pageLeaveWarning/reset');
      this.$store.dispatch('messaging/clear');
      clearInterval(this.polling);
    },
    showModal() {
      this.$store.dispatch('pageLeaveWarning/showKeywordReplyLeavingModal');
    },
    shouldShowOptions() {
      return this.$store.state.messaging.errorReplyCount > 0;
    },
    getPreviousChoice() {
      return this.$store.state.messaging.previousChoice;
    },
    async checkStatus() {
      const { messageId } = this;
      await this.$store.dispatch('messaging/loadMessage', { messageId });

      if (this.messageStatus === 'Succeeded') {
        clearInterval(this.polling);
        await this.loadMessage(true);
      }

      if (this.messageStatus === 'Failed' && (this.responseCompleteDt > this.responseSentDt)) {
        clearInterval(this.polling);
        this.showSpinner = false;

        this.$store.dispatch('messaging/addErrorReply', 'Supplier outcome status has failed');
      }

      if (this.error || this.errorReply) {
        clearInterval(this.polling);
        this.showSpinner = false;
      }
    },
    async pollStatus() {
      this.showSpinner = true;

      this.polling = setInterval(() => {
        this.checkStatus();
      }, this.messageReplyIntervalTime);
    },
  },
};
</script>

<style lang="scss">
p.panel-content > a{
  display: inline;
  font-weight: normal;
  vertical-align: unset;
}

div.panel-content{
  ol{
    padding-left:1.5em;
  }

  p > a{
    display: inline;
    font-weight: normal;
    vertical-align: unset;
  }

  img{
    display: block;
    max-width: 100%;
  }
}
</style>

<style module lang='scss' scoped>
  @import "@/style/custom/app-message";
  @import "@/style/custom/message";
</style>
