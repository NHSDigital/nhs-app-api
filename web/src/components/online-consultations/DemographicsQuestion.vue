<template>
  <question>
    <div slot="question-slot" class="demographicsQuestion">
      <slot/>
    </div>
    <no-js-form :value="noJsState" method="post">
      <input :name="answeringDemographicsName" value="true" type="hidden">
      <generic-checkbox :key="code"
                        checkbox-id="demographics-checkbox"
                        :name="name"
                        :required="false"
                        :value="code"
                        @input="selectValueChanged()">
        <span>
          {{ $t('onlineConsultations.demographics.checkboxLabel') }}
          <span>
            <!-- opening and closing tag must be on one line to
            avoid the inline-block white space issue - inline block
            prevents font boosting - accessibility issue
            -->
            <a :href="this.$store.app.$env.PRIVACY_POLICY_URL"
               target="_blank">{{ $t('onlineConsultations.demographics.checkboxLink') }}.</a>
          </span>
        </span>
      </generic-checkbox>
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
import GenericButton from '@/components/widgets/GenericButton';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
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
    GenericCheckbox,
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
  },
  data() {
    return {
      code: DEMOGRAPHICS_QUESTION_OPTION,
      answeringDemographicsName: ANSWERING_DEMOGRAPHICS_NAME,
      name: DEMOGRAPHICS_QUESTION_NAME,
      isDemographicsAccepted: false,
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
  },
  methods: {
    selectValueChanged() {
      this.isDemographicsAccepted = !this.isDemographicsAccepted;
      const code = (this.isDemographicsAccepted) ? this.code : undefined;
      this.$store.dispatch('onlineConsultations/setAnswer', code);
    },
    stopProp(event) {
      event.stopPropagation();
    },
    async demographicsContinueClicked() {
      document.activeElement.blur();
      const consentGiven = this.isDemographicsAccepted;
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
  .nhsuk-button {
    margin-top: 1em;
  }
</style>
