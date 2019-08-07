<template>
  <div :class="[!isNativeApp && $style.container, 'pull-content']">
    <div v-if="question && isDataRequired" id="questionnaire-container">
      <message-dialog v-if="isValidationError"
                      :key="questionKey"
                      :class="$style.errorDialog"
                      message-type="error"
                      role="alert">
        <message-text>
          {{ $t('appointments.admin_help.errors.validation.header') }}
        </message-text>
        <message-list>
          <li>{{ validationErrorMessage }}</li>
          <li v-for="error in validationErrorMessageFromResponse" :key="error">{{ error }}</li>
        </message-list>
      </message-dialog>

      <no-js-form :value="noJsState" method="post">
        <question :id="question.id"
                  :is-legend="question.isLegend"
                  :label-for="question.name"
                  :question-tag="question.tag"
                  :text="question.text"
                  :required="question.required"
                  :error="isValidationError">
          <component :is="questionComponent"
                     :id="question.name"
                     :key="questionKey"
                     v-model="answer"
                     :type="question.type"
                     :name="question.name"
                     :required="question.required"
                     :all-options-required="question.allOptionsRequired"
                     :options="question.options"
                     :min="question.min"
                     :max="question.max"
                     :source="question.source"
                     :accept="question.accept"
                     :max-size="question.maxSize"
                     :max-value="question.maxValue"
                     :max-length="question.maxLength"
                     :error="isValidationError"
                     :error-text="
                       validationErrorMessages(
                         validationErrorMessage, validationErrorMessageFromResponse)"
                     :render-as-html="true"
                     @validate="onAnswerValidate"/>
        </question>
        <generic-button :button-classes="['button', 'green']"
                        :class="$style.button"
                        click-delay="short"
                        @click.prevent="continueClicked">
          {{ $t('appointments.admin_help.orchestrator.continueButton') }}
        </generic-button>
      </no-js-form>
    </div>

    <div v-else-if="isSuccess && (carePlans || referralRequests)" id="result-container">

      <div v-for="referralRequest in referralRequests" :key="referralRequest.id">
        <question question-tag="div" :text="referralRequest.description"/>
      </div>

      <div v-for="carePlan in carePlans" :key="carePlan.id">
        <question question-tag="div" :text="carePlan.title"/>
        <question v-for="(activity, index) in carePlan.activities"
                  :key="`${carePlan.id}-activity-${index}`"
                  question-tag="div"
                  :text="activity"/>
      </div>
    </div>

    <desktopGenericBackLink v-if="showDesktopBackLink"
                            :path="indexPath"
                            :button-text="backButtonText"
                            data-purpose="back-to-home-button"
                            @clickAndPrevent="backToHomeClicked"/>
    <no-js-form v-else-if="showBackButton" :value="noJsState" method="post">
      <input type="hidden" name="direction" value="back">
      <generic-button :button-classes="['button', 'grey']"
                      :class="$style.button"
                      click-delay="short"
                      @click.prevent="backClicked">
        {{ $t('appointments.admin_help.orchestrator.backButton') }}
      </generic-button>
    </no-js-form>
    <form v-else :action="indexPath">
      <back-button :goto-path="indexPath"
                   :class="$style.button"
                   :text="$t(backButtonText)"/>
    </form>
  </div>
</template>

<script>
import { get } from 'lodash/fp';
import BackButton from '@/components/BackButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import NoJsForm from '@/components/no-js/NoJsForm';
import Question from '@/components/online-consultations/Question';
import QuestionAttachment from '@/components/online-consultations/QuestionAttachment';
import QuestionBoolean from '@/components/online-consultations/QuestionBoolean';
import QuestionChoice from '@/components/online-consultations/QuestionChoice';
import QuestionDate from '@/components/online-consultations/QuestionDate';
import QuestionDateTime from '@/components/online-consultations/QuestionDateTime';
import QuestionImage from '@/components/online-consultations/QuestionImage';
import QuestionMultipleChoice from '@/components/online-consultations/QuestionMultipleChoice';
import QuestionNumber from '@/components/online-consultations/QuestionNumber';
import QuestionQuantity from '@/components/online-consultations/QuestionQuantity';
import QuestionString from '@/components/online-consultations/QuestionString';
import QuestionText from '@/components/online-consultations/QuestionText';
import QuestionTime from '@/components/online-consultations/QuestionTime';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';
import { DATA_REQUIRED, SUCCESS } from '@/lib/online-consultations/constants/status-types';
import { INDEX } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import NativeApp from '@/services/native-app';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

export default {
  name: 'Orchestrator',
  components: {
    BackButton,
    DesktopGenericBackLink,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    NoJsForm,
    Question,
    QuestionBoolean,
    QuestionChoice,
    QuestionDate,
    QuestionDateTime,
    QuestionImage,
    QuestionMultipleChoice,
    QuestionNumber,
    QuestionQuantity,
    QuestionString,
    QuestionText,
    QuestionTime,
    QuestionAttachment,
  },
  props: {
    journey: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    isValidationError() {
      return this.$store.state.onlineConsultations.validationError;
    },
    validationErrorMessage() {
      return this.$t(this.$store.state.onlineConsultations.validationErrorMessage,
        { value: this.$store.state.onlineConsultations.latestAdditionalValue });
    },
    validationErrorMessageFromResponse() {
      return this.$store.state.onlineConsultations.validationErrorMessageFromResponse;
    },
    isDataRequired() {
      return this.$store.state.onlineConsultations.status === DATA_REQUIRED;
    },
    question() {
      return this.$store.state.onlineConsultations.question;
    },
    questionComponent() {
      switch (get('question.type', this)) {
        case QuestionTypes.ATTACHMENT:
          return QuestionAttachment;
        case QuestionTypes.BOOLEAN:
          return QuestionBoolean;
        case QuestionTypes.CHOICE:
          return QuestionChoice;
        case QuestionTypes.DATE:
          return QuestionDate;
        case QuestionTypes.DATETIME:
          return QuestionDateTime;
        case QuestionTypes.DECIMAL:
        case QuestionTypes.INTEGER:
          return QuestionNumber;
        case QuestionTypes.IMAGE:
          return QuestionImage;
        case QuestionTypes.MULTIPLE_CHOICE:
          return QuestionMultipleChoice;
        case QuestionTypes.QUANTITY:
          return QuestionQuantity;
        case QuestionTypes.STRING:
          return QuestionString;
        case QuestionTypes.TEXT:
          return QuestionText;
        case QuestionTypes.TIME:
          return QuestionTime;
        default:
          this.$store.dispatch('onlineConsultations/clearAndSetError');
          return undefined;
      }
    },
    questionKey() {
      return `${get('question.id', this)}-${this.requestId}`;
    },
    requestId() {
      return this.$store.state.onlineConsultations.requestId;
    },
    isSuccess() {
      return this.$store.state.onlineConsultations.status === SUCCESS;
    },
    carePlans() {
      return this.$store.state.onlineConsultations.carePlans;
    },
    referralRequests() {
      return this.$store.state.onlineConsultations.referralRequests;
    },
    answer: {
      get() {
        return this.$store.state.onlineConsultations.answer;
      },
      set(value) {
        this.$store.dispatch('onlineConsultations/setAnswer', value);
      },
    },
    noJsState() {
      return {
        onlineConsultations: this.$store.state.onlineConsultations,
      };
    },
    indexPath() {
      return INDEX.path;
    },
    showDesktopBackLink() {
      return !this.isNativeApp && this.isSuccess;
    },
    backButtonText() {
      return this.isSuccess
        ? 'appointments.admin_help.orchestrator.backToHomeButton'
        : 'appointments.admin_help.orchestrator.endMyConsultationButton';
    },
    nothingToDisplay() {
      if (this.isSuccess) {
        return (!this.carePlans || this.carePlans.length === 0) &&
               (!this.referralRequests || this.referralRequests.length === 0);
      }
      if (this.isDataRequired) {
        return !this.question;
      }
      return true;
    },
    showBackButton() {
      return this.$store.state.onlineConsultations.previousQuestion !== undefined;
    },
  },
  watch: {
    nothingToDisplay(to) {
      if (to) {
        this.$store.dispatch('onlineConsultations/clearAndSetError');
      }
    },
  },
  methods: {
    onAnswerValidate(validation) {
      this.$store.dispatch('onlineConsultations/setAnswerIsValid', validation);
    },
    async continueClicked() {
      if (this.$store.state.onlineConsultations.isLoadingFile) {
        return;
      }
      await this.$store.dispatch('onlineConsultations/clearClientErrors');
      await this.$store.dispatch('onlineConsultations/setValidationError');
      if (!this.isValidationError) {
        document.activeElement.blur();
        await this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', this.journey);
        if (this.isNativeApp) {
          NativeApp.resetPageFocus();
        } else {
          EventBus.$emit(FOCUS_NHSAPP_ROOT);
        }
      }
      window.scrollTo(0, 0);
    },
    async backClicked() {
      document.activeElement.blur();
      await this.$store.dispatch('onlineConsultations/setPrevious');
      await this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', this.journey);
      if (this.isNativeApp) {
        NativeApp.resetPageFocus();
      } else {
        EventBus.$emit(FOCUS_NHSAPP_ROOT);
      }
      window.scrollTo(0, 0);
    },
    backToHomeClicked() {
      redirectTo(this, INDEX.path, null);
    },
    validationErrorMessages(localError, responseErrors) {
      const errorMessages = [];
      errorMessages.push(localError);
      if (responseErrors !== undefined) {
        responseErrors.forEach(c => errorMessages.push(c));
      }
      return errorMessages;
    },
  },
};
</script>

<style lang="scss">
  .nhsuk-form-group ul {
    margin-left: 1em;
  }
</style>


<style module lang="scss" scoped>
  .errorDialog {
    margin-bottom: 2em;
  }

  .container {
    margin-top: 1em;

    .button {
      min-width: 16.875em;
      padding-left: 2em;
      padding-right: 2em;
      max-width: 960px;
      width: auto;
    }
  }
</style>
