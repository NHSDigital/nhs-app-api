<template>
  <div v-if="serviceDefinitionId" :class="!isNativeApp && $style.container">
    <div v-if="question">
      <question :id="question.id"
                :is-legend="question.isLegend"
                :question-tag="question.tag"
                :text="question.text">
        <question-choice v-if="question.type === questionTypes.CHOICE"
                         :options="question.options"
                         :name="question.name"/>
      </question>
      <generic-button :button-classes="['button', 'green']">
        {{ $t('onlineConsultations.orchestrator.continueButton') }}
      </generic-button>
    </div>
    <form :action="backRoute">
      <generic-button :button-classes="['button', 'grey']"
                      click-delay="medium"
                      @click.stop.prevent="onBackButtonClicked()">
        {{ $t('onlineConsultations.orchestrator.backButton') }}
      </generic-button>
    </form>
  </div>
</template>

<script>
import GenericButton from '@/components/widgets/GenericButton';
import Question from '@/components/online-consultations/Question';
import QuestionChoice from '@/components/online-consultations/QuestionChoice';
import { CHOICE } from '@/lib/online-consultations/question-types';

export default {
  name: 'Orchestrator',
  components: {
    GenericButton,
    Question,
    QuestionChoice,
  },
  computed: {
    serviceDefinitionId() {
      return this.$store.state.onlineConsultations.serviceDefinitionId;
    },
    question() {
      return this.$store.state.onlineConsultations.question;
    },
    questionTypes() {
      return {
        CHOICE,
      };
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    backRoute() {
      return this.$store.state.onlineConsultations.previousRoute;
    },
  },
  methods: {
    onBackButtonClicked() {
      this.$router.goBack();
    },
  },
};
</script>

<style module lang="scss" scoped>
  .container {
    margin-top: 1em;
  }
</style>
