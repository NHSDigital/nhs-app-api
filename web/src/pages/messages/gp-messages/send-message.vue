<template>
  <div v-if="showTemplate && !hasSent">
    <div class="nhsuk-grid-row nhsuk-grid-column-full">
      <div v-if="showError" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <message-dialog id="errorDialog" message-type="error" role="alert" :focusable="true">
            <message-text data-purpose="error-heading">
              {{ $t('messages.thereIsAProblem') }}
            </message-text>
            <message-list>
              <li v-if="subjectError && subjectEnabled" data-purpose="subject-error">
                {{ $t('messages.enterASubject') }}
              </li>
              <li v-if="messageTextError" data-purpose="message-error">
                {{ $t('messages.enterAMessage') }}
              </li>
            </message-list>
          </message-dialog>
        </div>
      </div>
      <p id="subHeader"
         class="nhsuk-hint"
         :aria-label="$t('messages.forAdviceNowContactSurgeryOrOneOneOne')">
        {{ $t('messages.forAdviceNowContactSurgery') }}
        <a style="display:inline; vertical-align: baseline" href="https://111.nhs.uk">
          {{ $t('messages.nhs111Link') }}</a>
        {{ $t('messages.or') }}
        <a style="display:inline; vertical-align: baseline" href="tel:111">
          {{ $t('messages.call111Link') }}.
        </a>
      </p>
      <sjr-if journey="sendMessageSubject">
        <div :class="['nhsuk-u-padding-top-1', getErrorClass]">
          <label id="subjectTextLabel" for="subjectText" class="nhsuk-label">
            <strong>
              {{ $t('messages.subject') }}
            </strong>
            <div id="text-must-be-shorter-than" class="nhsuk-u-padding-top-2 nhsuk-hint">
              {{ $t('messages.textMustBeShorterThan64Characters') }}
            </div>
          </label>
          <generic-text-input id="subjectText"
                              v-model="subjectText"
                              maxlength="64"
                              :error-text="
                                $t('messages.enterASubject')"
                              aria-describedby="text-must-be-shorter-than-64"
                              :error="subjectError"
                              :required="true"/>
        </div>
      </sjr-if>
      <div :class="['nhsuk-u-padding-top-4', getErrorClass]">
        <label id="messageTextLabel" for="messageText" class="nhsuk-label">
          <strong>
            {{ $t('messages.message') }}
          </strong>
          <div id="text-must-be-shorter-than-450" class="nhsuk-u-padding-top-2 nhsuk-hint">
            {{ $t('messages.textMustBeShorterThan450Characters') }}
          </div>
        </label>
        <generic-text-area id="messageText"
                           v-model="messageText"
                           maxlength="450"
                           :rows="5"
                           :error="messageTextError"
                           aria-describedby="text-must-be-shorter-than-450"
                           :error-text="
                             $t('messages.enterAMessage')"
                           :required="true"/>
      </div>
    </div>
    <div>
      <generic-button id="send_message_btn"
                      :button-classes="['nhsuk-button']"
                      click-delay="medium"
                      @click.prevent="onSendMessageButtonClicked">
        {{ $t('messages.sendMessage') }}
      </generic-button>
    </div>
    <desktopGenericBackLink v-if="!this.$store.state.device.isNativeApp"
                            :path="backPath"
                            button-text="generic.back"
                            @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import SjrIf from '@/components/SjrIf';
import { redirectTo } from '@/lib/utils';
import srjIf from '@/lib/sjrIf';
import {
  GP_MESSAGES_PATH,
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_RECIPIENTS_PATH,
} from '@/router/paths';

export default {
  name: 'GpMessagesSendMessagePage',
  components: {
    GenericTextArea,
    GenericTextInput,
    GenericButton,
    MessageDialog,
    MessageText,
    MessageList,
    DesktopGenericBackLink,
    SjrIf,
  },
  data() {
    return {
      messageText: '',
      subjectText: '',
      messageTextError: false,
      subjectError: false,
    };
  },
  computed: {
    showError() {
      return (this.subjectError && this.subjectEnabled) || this.messageTextError;
    },
    backPath() {
      return GP_MESSAGES_RECIPIENTS_PATH;
    },
    getErrorClass() {
      return this.showError ? 'nhsuk-form-group--error' : '';
    },
    selectedMessageRecipient() {
      return this.$store.state.gpMessages.selectedMessageRecipient;
    },
    hasSent() {
      return this.$store.state.gpMessages.messageSent;
    },
    subjectEnabled() {
      return srjIf({ $store: this.$store, journey: 'sendMessageSubject' });
    },
  },
  created() {
    if (this.selectedMessageRecipient === undefined || this.hasSent) {
      redirectTo(this, GP_MESSAGES_PATH);
    }
  },
  methods: {
    async onSendMessageButtonClicked() {
      this.messageTextError = false;
      this.subjectError = false;

      if (this.messageText.trim() === '') {
        this.messageTextError = true;
      }

      if (this.subjectText.trim() === '') {
        this.subjectError = true;
      }

      if (this.showError) {
        window.scrollTo(0, 0);
        return;
      }
      const { messageText, subjectText } = this;

      this.$store.dispatch('gpMessages/setSelectedMessageID', '0');
      await this.$store.dispatch('gpMessages/sendMessage', { messageText, subjectText });

      if (this.$store.state.gpMessages.messageSent) {
        this.$store.dispatch('gpMessages/setMessageDetails', {
          messageDetails: {
            messageId: '0',
            subject: subjectText,
            content: messageText,
            sentDateTime: new Date(),
            outboundMessage: true,
            replies: [],
          } });
        redirectTo(this, GP_MESSAGES_VIEW_MESSAGE_PATH);
      }
    },
    onBackButtonClicked() {
      redirectTo(this, GP_MESSAGES_RECIPIENTS_PATH);
    },
  },
};
</script>
