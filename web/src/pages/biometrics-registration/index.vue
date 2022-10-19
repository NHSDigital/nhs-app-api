<template>
  <no-return-flow-layout>
    <div v-if="showTemplate">
      <form-error-summary v-if="showError"
                          :header-locale-ref="'biometricsRegistration.thereIsAProblem'"
                          :errors="biometricsErrorText"
                          :errors-ids="`biometricsRegistration-${getFirstChoiceValue('choices')}`"/>
      <p> {{ biometricText }} </p>
      <message-dialog-generic message-type="warning" :icon-text="$t('biometricsRegistration.ifYouShareThisDevice')">
        <message-text>
          <p> {{ biometricWarningText }}</p>
          <p> {{ biometricAdditionalWarningText }}</p>
        </message-text>
      </message-dialog-generic>
      <nhs-uk-radio-group
        v-model="selectedValue"
        name="biometricsRegistration"
        :heading="'<h2>'
          + $t(`biometricsRegistration.${biometricType}.doYouWantToTurnOn`)
          + '</h2>'"
        :heading-as-html="true"
        :legend-size="'m'"
        :error="showError"
        :error-heading-reference="'biometricsRegistration.thereIsAProblem'"
        :error-text="biometricsErrorText"
        :items="choices"/>
      <primary-button id="btn_continue" @click="onContinue">
        {{ $t('generic.continue') }}
      </primary-button>
    </div>
  </no-return-flow-layout>
</template>

<script>
import PrimaryButton from '@/components/PrimaryButton';
import NhsUkRadioGroup from '@/components/nhsuk-frontend/NhsUkRadioGroup';
import isUndefined from 'lodash/fp/isUndefined';
import MessageDialogGeneric from '@/components/widgets/MessageDialogGeneric';
import MessageText from '@/components/widgets/MessageText';
import { UPDATE_HEADER, UPDATE_TITLE, EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';
import FormErrorSummary from '@/components/FormErrorSummary';
import RedirectMixin from '@/components/RedirectMixin';
import NoReturnFlowLayout from '@/layouts/no-return-flow-layout';
import get from 'lodash/fp/get';

export default {
  name: 'BiometricsRegistrationPage',
  components: {
    PrimaryButton,
    NhsUkRadioGroup,
    MessageDialogGeneric,
    MessageText,
    FormErrorSummary,
    NoReturnFlowLayout,
  },
  mixins: [RedirectMixin],
  data() {
    return {
      hasTriedToContinue: false,
      selectedValue: undefined,
      biometricType: this.$store.getters['loginSettings/biometricType'],
      biometricsRegistered: this.$store.getters['loginSettings/biometricRegistered'],
      biometricsSupported: this.$store.getters['loginSettings/biometricSupported'],
    };
  },
  computed: {
    showError() {
      return this.hasTriedToContinue && isUndefined(this.selectedValue);
    },
    choices() {
      return [
        { label: this.$t(`biometricsRegistration.${this.biometricType}.yes`), value: true },
        { label: this.$t(`biometricsRegistration.${this.biometricType}.no`), value: false },
      ];
    },
    biometricText() {
      return this.$t(`biometricsRegistration.${this.biometricType}.text`);
    },
    biometricWarningText() {
      return this.$t(`biometricsRegistration.${this.biometricType}.warningText`);
    },
    biometricAdditionalWarningText() {
      return this.$t('biometricsRegistration.thisMeansTheyCanSee');
    },
    biometricsErrorText() {
      return this.$t(`biometricsRegistration.${this.biometricType}.errorText`);
    },
    pageHeaderAndTitle() {
      return this.$t(`biometricsRegistration.${this.biometricType}.title`);
    },
  },
  mounted() {
    if (this.isBiometricsRegistered() || !this.isBiometricsSupported()) {
      this.conditionalRedirect();
    }
    EventBus.$emit(UPDATE_HEADER, this.pageHeaderAndTitle, true);
    EventBus.$emit(UPDATE_TITLE, this.pageHeaderAndTitle, true);
  },
  methods: {
    async onContinue() {
      this.hasTriedToContinue = true;
      if (this.showError) {
        EventBus.$emit(FOCUS_ERROR_ELEMENT);
        return;
      }

      if (!this.showError) {
        await this.$store.dispatch('biometrics/showBiometricSpinner', true);
        await this.$store.dispatch('biometrics/addBiometricsCookie');

        if (this.selectedValue === true) {
          await this.$store.dispatch('loginSettings/updateRegistration');
        } else {
          try {
            await this.$store.app.$http.postV1ApiMetricsBiometricsOptOut();
          } catch {
            // do nothing as this is just logging
          }
          this.conditionalRedirect();
        }
      }
    },
    onSelection(value) {
      this.selectedValue = value;
    },
    isBiometricsRegistered() {
      return this.biometricsRegistered;
    },
    isBiometricsSupported() {
      return this.biometricsSupported;
    },
    getFirstChoiceValue(choicesName) {
      if (get(`${choicesName}[0].value`, this) !== undefined && get(`${choicesName}[0].value`, this) !== '') {
        return get(`${choicesName}[0].value`, this);
      }
      if (get(`${choicesName}[0].code`, this) !== undefined && get(`${choicesName}[0].code`, this) !== '') {
        return get(`${choicesName}[0].code`, this);
      }
      return '';
    },
  },
};
</script>
