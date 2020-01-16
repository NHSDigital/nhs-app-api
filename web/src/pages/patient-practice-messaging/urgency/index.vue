<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <message-dialog v-if="isError" message-type="error" role="alert">
        <message-text>
          {{ $t('im02.noOptionSelectedErrorHeader') }}
        </message-text>
        <message-list>
          <li>
            <p class="nhsuk-u-margin-left-2">
              {{ $t('im02.noOptionSelectedErrorText') }}</p>
          </li>
        </message-list>
      </message-dialog>
      <question :error="isError" :required="true">
        <question-choice id="messagingUrgency"
                         v-model="answer"
                         :error="isError"
                         :error-text="[$t('im02.noOptionSelectedErrorText')]"
                         :options="questionOptions"
                         :required="true"
                         :legend="$t('pageTitles.patientPracticeMessagingUrgency')"
                         name="messagingUrgency"
                         @validate="onAnswerValidate"/>
      </question>
      <generic-button id="continueButton"
                      :button-classes="['nhsuk-button']"
                      @click="continueButtonClicked">
        {{ $t('im02.continueButtonText') }}
      </generic-button>
      <desktop-generic-back-link v-if="!$store.state.device.isNativeApp"
                                 :path="messagingPath"
                                 @clickAndPrevent="backLinkClicked"/>
    </div>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import Question from '@/components/online-consultations/Question';
import QuestionChoice from '@/components/online-consultations/QuestionChoice';
import {
  INDEX,
  PATIENT_PRACTICE_MESSAGING,
  PATIENT_PRACTICE_MESSAGING_URGENCY_CONTACT_GP,
  PATIENT_PRACTICE_MESSAGING_RECIPIENTS,
} from '@/lib/routes';
import { isFalsy, redirectTo } from '@/lib/utils';

const YES = 'yes';
const NO = 'no';

export default {
  layout: 'nhsuk-layout',
  components: {
    DesktopGenericBackLink,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    Question,
    QuestionChoice,
  },
  data() {
    return {
      isError: false,
      questionOptions: [{
        code: YES,
        label: this.$t('im02.isUrgentChoiceLabel'),
      }, {
        code: NO,
        label: this.$t('im02.isNotUrgentChoiceLabel'),
      }],
      messagingPath: PATIENT_PRACTICE_MESSAGING.path,
    };
  },
  computed: {
    answer: {
      get() {
        return this.$store.state.patientPracticeMessaging.urgencyChoice;
      },
      set(value) {
        this.$store.dispatch('patientPracticeMessaging/setUrgencyChoice', value);
      },
    },
  },
  fetch({ store, redirect }) {
    if (isFalsy(store.app.$env.PATIENT_PRACTICE_MESSAGING_ENABLED)) {
      redirect(INDEX.path);
    }
  },
  methods: {
    onAnswerValidate(validation) {
      this.isValid = validation.isValid;
    },
    continueButtonClicked() {
      if (!this.isValid) {
        this.isError = true;
        return;
      }
      redirectTo(this, this.answer === YES
        ? PATIENT_PRACTICE_MESSAGING_URGENCY_CONTACT_GP.path
        : PATIENT_PRACTICE_MESSAGING_RECIPIENTS.path);
    },
    backLinkClicked() {
      redirectTo(this, this.messagingPath);
    },
  },
};
</script>
