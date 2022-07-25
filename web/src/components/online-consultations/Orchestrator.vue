<template>
  <div :class="[!isNativeApp && $style.container, 'pull-content']">
    <div v-if="question && isDataRequired" id="questionnaire-container">
      <form-error-summary v-if="isValidationError"
                          :key="questionKey"
                          :header-locale-ref="'onlineConsultations.validationErrors.thereIsAProblem'"
                          :errors="getErrors"
                          :errors-ids="getErrorId()"/>

      <form @submit.prevent="continueClicked">
        <component :is="questionWrapper"
                   :id="question.id"
                   :key="questionKey"
                   :is-legend="question.isLegend"
                   :label-for="question.name"
                   :question-tag="question.tag"
                   :text="question.text"
                   :required="question.required"
                   :error="isValidationError">
          <component :is="questionComponent"
                     :id="question.name"
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
        </component>
        <generic-button id="continueButton"
                        :button-classes="['nhsuk-button']"
                        click-delay="short"
                        @click.prevent="continueClicked">
          {{ $t('onlineConsultations.orchestrator.continueButton') }}
        </generic-button>
      </form>
    </div>

    <div v-else-if="isSuccess && (carePlans || referralRequests)" id="result-container">

      <div v-for="referralRequest in referralRequests" :key="referralRequest.id">
        <generic-question-wrapper question-tag="div" :text="referralRequest.description"/>
      </div>

      <div v-for="carePlan in carePlans" :key="carePlan.id">
        <generic-question-wrapper :id="`${carePlan.id}-title`"
                                  question-tag="div"
                                  :text="carePlan.title"/>
        <generic-question-wrapper v-for="(activity, index) in carePlan.activities"
                                  :key="`${carePlan.id}-activity-${index}`"
                                  question-tag="div"
                                  :text="activity"/>
      </div>
    </div>

    <desktop-generic-back-link v-if="showBackToHomeButton"
                               :path="indexPath"
                               :button-text="backButtonText"
                               data-purpose="back-to-home-button"
                               @clickAndPrevent="backToHomeClicked"/>
    <desktop-generic-back-link v-else-if="showBackButton && !isNativeApp"
                               id="desktopBackLink"
                               data-purpose="back-to-home-button"
                               :path="indexPath"
                               :button-text="'onlineConsultations.orchestrator.backButton'"
                               @clickAndPrevent="backClicked"/>
    <generic-button v-else-if="!showBackButton"
                    id="endMyConsultationButton"
                    :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                    @click.prevent="goBack">
      {{ $t('onlineConsultations.orchestrator.endMyConsultationButton') }}
    </generic-button>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import BackButton from '@/components/BackButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import FieldsetQuestionWrapper from '@/components/online-consultations/FieldsetQuestionWrapper';
import GenericButton from '@/components/widgets/GenericButton';
import GenericQuestionWrapper from '@/components/online-consultations/GenericQuestionWrapper';
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
import FormErrorSummary from '@/components/FormErrorSummary';
import QuestionTypes from '@/lib/online-consultations/constants/question-types';
import { DATA_REQUIRED, SUCCESS } from '@/lib/online-consultations/constants/status-types';
import { INDEX_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';
import isArray from 'lodash/fp/isArray';

export default {
  name: 'Orchestrator',
  components: {
    BackButton,
    DesktopGenericBackLink,
    FieldsetQuestionWrapper,
    GenericButton,
    GenericQuestionWrapper,
    FormErrorSummary,
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
    provider: {
      type: String,
      required: true,
    },
    serviceDefinitionId: {
      type: String,
      required: true,
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
    getErrors() {
      const errors = [];
      errors.push(this.validationErrorMessage);
      if (typeof (this.validationErrorMessageFromResponse) !== 'undefined' && this.validationErrorMessageFromResponse !== null) {
        if (isArray(this.validationErrorMessageFromResponse)) {
          errors.push(this.validationErrorMessageFromResponse[0]);
        } else {
          errors.push(this.validationErrorMessageFromResponse);
        }
      }
      return errors;
    },
    isDataRequired() {
      return this.$store.state.onlineConsultations.status === DATA_REQUIRED;
    },
    question() {
      return this.$store.state.onlineConsultations.question;
    },
    questionWrapper() {
      switch (get('question.type', this)) {
        case QuestionTypes.BOOLEAN:
        case QuestionTypes.CHOICE:
        case QuestionTypes.MULTIPLE_CHOICE:
          return FieldsetQuestionWrapper;
        default:
          return GenericQuestionWrapper;
      }
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
    indexPath() {
      return INDEX_PATH;
    },
    showBackToHomeButton() {
      if (!this.isSuccess) {
        return false;
      }
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);
      return true;
    },
    backButtonText() {
      return this.isSuccess
        ? 'onlineConsultations.orchestrator.backToHomeButton'
        : 'onlineConsultations.orchestrator.endMyConsultationButton';
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
      if (this.$store.state.onlineConsultations.previousQuestion !== undefined) {
        this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);
        return true;
      }
      return false;
    },
    enctype() {
      return this.question.type === QuestionTypes.ATTACHMENT
        ? 'multipart/form-data'
        : undefined;
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
      await this.$store.dispatch('onlineConsultations/clearValidation');
      this.$nextTick(async () => {
        await this.$store.dispatch('onlineConsultations/setValidationError');
        if (!this.isValidationError) {
          document.activeElement.blur();
          await this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', {
            provider: this.provider,
            serviceDefinitionId: this.serviceDefinitionId,
          });
          EventBus.$emit(FOCUS_NHSAPP_TITLE);
        }
        window.scrollTo(0, 0);
      });
    },
    async backClicked() {
      document.activeElement.blur();
      await this.$store.dispatch('onlineConsultations/setPrevious');
      await this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', {
        provider: this.provider,
        serviceDefinitionId: this.serviceDefinitionId,
      });
      EventBus.$emit(FOCUS_NHSAPP_TITLE);
      window.scrollTo(0, 0);
    },
    backToHomeClicked() {
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);
      redirectTo(this, INDEX_PATH);
    },
    validationErrorMessages(localError, responseErrors) {
      const errorMessages = [];
      errorMessages.push(localError);
      if (responseErrors !== undefined) {
        responseErrors.forEach(c => errorMessages.push(c));
      }
      return errorMessages;
    },
    goBack() {
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);
      redirectTo(this, this.indexPath);
    },
    getErrorId() {
      switch (get('question.type', this)) {
        case QuestionTypes.ATTACHMENT:
        case QuestionTypes.DECIMAL:
        case QuestionTypes.INTEGER:
        case QuestionTypes.IMAGE:
        case QuestionTypes.STRING:
        case QuestionTypes.TEXT:
          return get('question.name', this);
        case QuestionTypes.BOOLEAN:
          return `${get('question.name', this)}-true`;
        case QuestionTypes.CHOICE:
          return this.getFirstChoiceId();
        case QuestionTypes.DATE:
        case QuestionTypes.DATETIME:
          return `${get('question.name', this)}-day`;
        case QuestionTypes.MULTIPLE_CHOICE:
          return `checkbox-${get('question.options[0].code', this)}`;
        case QuestionTypes.QUANTITY:
          return `${get('question.name', this)}-quantity`;
        case QuestionTypes.TIME:
          return `${get('question.name', this)}-hour`;
        default:
          return get('question.id', this);
      }
    },
    getFirstChoiceId() {
      if (get('question.options[0].value', this) !== undefined && get('question.options[0].value', this) !== '') {
        return `${get('question.name', this)}-${get('question.options[0].value', this)}`;
      }
      if (get('question.options[0].code', this) !== undefined && get('question.options[0].code', this) !== '') {
        return `${get('question.name', this)}-${get('question.options[0].code', this)}`;
      }
      return get('question.id', this);
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
  @import "@/style/custom/orchestrator";
</style>
