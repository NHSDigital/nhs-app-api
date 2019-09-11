<template>
  <question>
    <div slot="question-slot" class="demographicsQuestion">
      <slot/>
    </div>
    <no-js-form :value="noJsState" method="post">
      <input :name="answeringDemographicsName" value="true" type="hidden">
      <question-multiple-choice v-model="demographicsAnswer"
                                :options="demographicsQuestion.options"
                                :name="demographicsQuestion.name"/>
      <generic-button :button-classes="['nhsuk-button']"
                      click-delay="short"
                      @click.prevent="demographicsContinueClicked">
        {{ $t('onlineConsultations.orchestrator.continueButton') }}
      </generic-button>
    </no-js-form>
  </question>
</template>

<script>
import Question from '@/components/online-consultations/Question';
import NoJsForm from '@/components/no-js/NoJsForm';
import QuestionMultipleChoice from '@/components/online-consultations/QuestionMultipleChoice';
import GenericButton from '@/components/widgets/GenericButton';
import {
  ANSWERING_DEMOGRAPHICS_NAME,
  DEMOGRAPHICS_QUESTION_NAME,
  DEMOGRAPHICS_QUESTION_OPTION,
} from '@/lib/online-consultations/constants/demographics';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import NativeApp from '@/services/native-app';

export default {
  name: 'DemographicsQuestion',
  components: {
    Question,
    NoJsForm,
    QuestionMultipleChoice,
    GenericButton,
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
    checkboxLabel: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      answeringDemographicsName: ANSWERING_DEMOGRAPHICS_NAME,
      demographicsQuestion: {
        name: DEMOGRAPHICS_QUESTION_NAME,
        options: [{
          code: DEMOGRAPHICS_QUESTION_OPTION,
          label: this.checkboxLabel,
          required: false,
        }],
      },
    };
  },
  computed: {
    noJsState() {
      return {
        onlineConsultations: this.$store.state.onlineConsultations,
      };
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    demographicsAnswer: {
      get() {
        return this.$store.state.onlineConsultations.answer;
      },
      set(value) {
        this.$store.dispatch('onlineConsultations/setAnswer', value);
      },
    },
  },
  methods: {
    async demographicsContinueClicked() {
      document.activeElement.blur();
      const consentGiven = (this.demographicsAnswer || []).includes(DEMOGRAPHICS_QUESTION_OPTION);
      await this.$store.dispatch('onlineConsultations/setDemographicsConsentGiven', consentGiven);
      await this.$store.dispatch('onlineConsultations/getServiceDefinition', {
        provider: this.provider,
        serviceDefinitionId: this.serviceDefinitionId,
      });
      if (this.isNativeApp) {
        NativeApp.resetPageFocus();
      } else {
        EventBus.$emit(FOCUS_NHSAPP_ROOT);
      }
      window.scrollTo(0, 0);
    },
  },
};
</script>

<style lang="scss">
  .demographicsQuestion p:not(:last-of-type) {
    margin-bottom: 1em;
  }
</style>
