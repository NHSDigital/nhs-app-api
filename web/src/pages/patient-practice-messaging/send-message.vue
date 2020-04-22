<template>
  <div v-if="showTemplate && !hasSent">
    <div class="nhsuk-grid-row nhsuk-grid-column-full">
      <div v-if="showError" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <message-dialog id="errorDialog" message-type="error" role="alert">
            <message-text data-purpose="error-heading">
              {{ $t('appointments.confirmation.errorDialog') }}
            </message-text>
            <div data-purpose="error-dialog-list">
              <message-list>
                <li v-if="subjectError && subjectEnabled" data-purpose="subject-error">
                  <p class="nhsuk-u-margin-left-2">
                    {{ $t('patient_practice_messaging.createMessage.subjectTextError') }}
                  </p>
                </li>
                <li v-if="messageTextError" data-purpose="message-error">
                  <p class="nhsuk-u-margin-left-2">
                    {{ $t('patient_practice_messaging.createMessage.messageTextError') }}
                  </p>
                </li>
              </message-list>
            </div>
          </message-dialog>
        </div>
      </div>
      <p id="subHeader" class="nhsuk-hint" :aria-label="$t('im03.info.paragraph2.ariaLabel')">
        {{ $t('patient_practice_messaging.createMessage.subHeader') }}
        <a style="display:inline; vertical-align: baseline" href="https://111.nhs.uk">
          {{ $t('patient_practice_messaging.createMessage.nhs111Link') }}</a>
        {{ $t('patient_practice_messaging.createMessage.or') }}
        <a style="display:inline; vertical-align: baseline" href="tel:111">
          {{ $t('patient_practice_messaging.createMessage.call111Link') }}.
        </a>
      </p>
      <sjr-if journey="sendMessageSubject">
        <div :class="['nhsuk-u-padding-top-1', getErrorClass]">
          <label id="subjectTextLabel" for="subjectText" class="nhsuk-label">
            <strong>
              {{ $t('patient_practice_messaging.createMessage.subjectLabelText') }}
            </strong>
            <div class="nhsuk-u-padding-top-2 nhsuk-hint">
              {{ $t('patient_practice_messaging.createMessage.subjectHintText') }}
            </div>
          </label>
          <generic-text-input id="subjectText"
                              v-model="subjectText"
                              maxlength="64"
                              :error-text="
                                $t('patient_practice_messaging.createMessage.subjectTextError')"
                              :error="subjectError"
                              :required="true"/>
        </div>
      </sjr-if>
      <div :class="['nhsuk-u-padding-top-4', getErrorClass]">
        <label id="messageTextLabel" for="messageText" class="nhsuk-label">
          <strong>
            {{ $t('patient_practice_messaging.createMessage.messageLabelText') }}
          </strong>
          <div class="nhsuk-u-padding-top-2 nhsuk-hint">
            {{ $t('patient_practice_messaging.createMessage.messageHintText') }}
          </div>
        </label>
        <generic-text-area id="messageText"
                           v-model="messageText"
                           maxlength="450"
                           :rows="5"
                           :error="messageTextError"
                           :error-text="
                             $t('patient_practice_messaging.createMessage.messageTextError')"
                           :required="true"/>
      </div>
    </div>
    <div>
      <generic-button id="send_message_btn"
                      :button-classes="['nhsuk-button']"
                      click-delay="medium"
                      @click.prevent="onSendMessageButtonClicked">
        {{ $t('patient_practice_messaging.createMessage.sendButtonText') }}
      </generic-button>
    </div>
    <desktopGenericBackLink v-if="!this.$store.state.device.isNativeApp"
                            :path="backPath"
                            button-text="appointments.guidance.backDesktopLinkText"
                            @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import { redirectTo } from '@/lib/utils';
import { PATIENT_PRACTICE_MESSAGING,
  PATIENT_PRACTICE_MESSAGING_RECIPIENTS,
  PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE,
} from '@/lib/routes';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import srjIf from '@/lib/sjrIf';
import SjrIf from '@/components/SjrIf';

export default {
  layout: 'nhsuk-layout',
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
      return PATIENT_PRACTICE_MESSAGING_RECIPIENTS.path;
    },
    getErrorClass() {
      return this.showError ? 'nhsuk-form-group--error' : '';
    },
    selectedMessageRecipient() {
      return this.$store.state.patientPracticeMessaging.selectedMessageRecipient;
    },
    hasSent() {
      return this.$store.state.patientPracticeMessaging.messageSent;
    },
    subjectEnabled() {
      return srjIf({ $store: this.$store, journey: 'sendMessageSubject' });
    },
  },
  fetch({ store, redirect }) {
    if (store.state.patientPracticeMessaging.selectedMessageRecipient === undefined) {
      redirect(PATIENT_PRACTICE_MESSAGING.path);
    }
  },
  created() {
    if (this.selectedMessageRecipient === undefined || this.hasSent) {
      redirectTo(this, PATIENT_PRACTICE_MESSAGING.path);
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

      this.$store.dispatch('patientPracticeMessaging/setSelectedMessageID', '0');
      await this.$store.dispatch('patientPracticeMessaging/sendMessage', { messageText, subjectText });

      if (this.$store.state.patientPracticeMessaging.messageSent) {
        this.$store.dispatch('patientPracticeMessaging/setMessageDetails', {
          messageDetails: {
            messageId: '0',
            subject: subjectText,
            content: messageText,
            sentDateTime: new Date(),
            outboundMessage: true,
            replies: [],
          } });
        redirectTo(this, PATIENT_PRACTICE_MESSAGING_VIEW_MESSAGE.path);
      }
    },
    onBackButtonClicked() {
      redirectTo(this, PATIENT_PRACTICE_MESSAGING_RECIPIENTS.path);
    },
  },
};
</script>

<style></style>
