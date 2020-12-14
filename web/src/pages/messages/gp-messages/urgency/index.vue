<template>
  <div v-if="showTemplate && !loading" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <div v-if="messageRecipients && messageRecipients.length > 0">
        <nhs-uk-radio-group id="messagingUrgency"
                            v-model="answer"
                            :error="isError"
                            :error-text="$t('messages.youNeedToSelectYesOrNo')"
                            :items="questionOptions"
                            :required="true"
                            :heading="$t('navigation.pages.titles.gpMessagesUrgency')"
                            name="messagingUrgency"
                            @validate="onAnswerValidate"/>
        <generic-button id="continueButton"
                        :button-classes="['nhsuk-button']"
                        @click="continueButtonClicked">
          {{ $t('generic.continue') }}
        </generic-button>
      </div>
      <div v-else id="noRecipients">
        <p id="subHeader"
           class="nhsuk-hint"
           :aria-label="$t('messages.contactSurgeryForMoreInformationOrGoToOneOneOne')">
          {{ $t('messages.contactSurgeryForMoreInformationOr') }}
          <a style="display:inline" href="https://111.nhs.uk">
            {{ $t('messages.nhs111Link') }}
          </a>
          {{ $t('messages.or') }}
          <a style="display:inline" href="tel:111">
            {{ $t('messages.call111Link') }}.
          </a>
        </p>
      </div>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="messagingPath"
                                 @clickAndPrevent="backLinkClicked"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import { redirectTo, isEmptyArray } from '@/lib/utils';
import {
  GP_MESSAGES_PATH,
  GP_MESSAGES_URGENCY_CONTACT_GP_PATH,
  GP_MESSAGES_RECIPIENTS_PATH,
} from '@/router/paths';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus } from '@/services/event-bus';

const YES = 'yes';
const NO = 'no';

export default {
  name: 'GpMessagesUrgencyPage',
  components: {
    DesktopGenericBackLink,
    GenericButton,
    NhsUkRadioGroup,
  },
  data() {
    return {
      isError: false,
      loading: true,
      questionOptions: [{
        value: YES,
        label: this.$t('messages.iNeedAdviceNow'),
      }, {
        value: NO,
        label: this.$t('messages.myMessageIsNotUrgent'),
      }],
      messagingPath: GP_MESSAGES_PATH,
    };
  },
  computed: {
    answer: {
      get() {
        return this.$store.state.gpMessages.urgencyChoice;
      },
      set(value) {
        this.$store.dispatch('gpMessages/setUrgencyChoice', value);
      },
    },
    messageRecipients() {
      return Array.isArray(this.$store.state.gpMessages.messageRecipients) ?
        this.$store.state.gpMessages.messageRecipients : [];
    },
  },
  async created() {
    await this.$store.dispatch('gpMessages/loadRecipients');
    const { messageRecipients } = this.$store.state.gpMessages;

    if (!messageRecipients || isEmptyArray(messageRecipients)) {
      EventBus.$emit(UPDATE_HEADER, 'messages.youCannotSendMessages');
      EventBus.$emit(UPDATE_TITLE, 'messages.youCannotSendMessages');
    }

    this.loading = false;
  },
  methods: {
    onAnswerValidate(validation) {
      this.isValid = validation.isValid;
    },
    continueButtonClicked() {
      this.isError = false;
      this.$nextTick(() => {
        if (!this.isValid) {
          this.isError = true;
          return;
        }
        redirectTo(this, this.answer === YES
          ? GP_MESSAGES_URGENCY_CONTACT_GP_PATH
          : GP_MESSAGES_RECIPIENTS_PATH);
      });
    },
    backLinkClicked() {
      redirectTo(this, this.$store.state.navigation.backLinkOverride || this.messagingPath);
    },
  },
};
</script>
