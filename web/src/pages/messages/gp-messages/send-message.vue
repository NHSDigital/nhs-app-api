<template>
  <div v-if="showTemplate && !hasSent">
    <div class="nhsuk-grid-row nhsuk-grid-column-full">
      <form-error-summary v-if="showError"
                          :header-locale-ref="'prescriptions.repeatCourses.errors.thereIsAProblem'"
                          :errors="getErrors"/>

      <p id="subHeader"
         class="nhsuk-hint"
         :aria-label="$t('messages.forAdviceNowContactSurgeryOrOneOneOne')">
        {{ $t('messages.forAdviceNowContactSurgery') }}
        <a style="display:inline; vertical-align: baseline" :href="oneOneOneUrl">
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
                              :a-described-by="'text-must-be-shorter-than-64'"
                              :error="subjectError"
                              :required="false"/>
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
                           :a-described-by="'text-must-be-shorter-than-450'"
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
    <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                               :path="backPath"
                               button-text="generic.back"/>
  </div>
</template>

<script>
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import FormErrorSummary from '@/components/FormErrorSummary';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import SjrIf from '@/components/SjrIf';
import { redirectTo } from '@/lib/utils';
import srjIf from '@/lib/sjrIf';
import {
  GP_MESSAGES_PATH,
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_RECIPIENTS_PATH,
} from '@/router/paths';
import { FOCUS_ERROR_ELEMENT, EventBus } from '@/services/event-bus';

export default {
  name: 'GpMessagesSendMessagePage',
  components: {
    GenericTextArea,
    GenericTextInput,
    GenericButton,
    FormErrorSummary,
    DesktopGenericBackLink,
    SjrIf,
  },
  data() {
    return {
      messageText: '',
      subjectText: '',
      messageTextError: false,
      subjectError: false,
      oneOneOneUrl: this.$store.$env.SYMPTOM_CHECKER_URL,
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
    getErrors() {
      const errors = [];
      if (this.subjectError && this.subjectEnabled) {
        errors.push(this.$t('messages.enterASubject'));
      }
      if (this.messageTextError) {
        errors.push(this.$t('messages.enterAMessage'));
      }
      return errors;
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

      this.$nextTick(async () => {
        if (this.messageText.trim() === '') {
          this.messageTextError = true;
        }

        if (this.subjectText.trim() === '') {
          this.subjectError = true;
        }

        if (this.showError) {
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
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
      });
    },
  },
};
</script>
