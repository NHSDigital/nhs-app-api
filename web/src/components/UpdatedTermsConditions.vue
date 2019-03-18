<template>
  <div v-if="showTemplate">
    <div v-if="hasTriedToContinue && !areTermsAccepted" id="error_msg">
      <message-dialog :class="$style.customErrorBox" message-type="error" icon-text="Error">
        <p :class="$style.customErrorText">
          {{ $t('updatedTermsAndConditions.errorMsgHeader') }}
        </p>
        <ul>
          <li> {{ $t('updatedTermsAndConditions.errorMsgText') }} </li>
        </ul>
      </message-dialog>
    </div>
    <div id="text_body" :class="$style.info">
      <p> {{ $t('updatedTermsAndConditions.body1') }}
        <a :href="termsAndConditionsURL" target="_blank">
          {{ $t('updatedTermsAndConditions.link1') }}</a>,
        <a :href="privacyPolicyURL" target="_blank">
          {{ $t('updatedTermsAndConditions.link2') }}</a> and
        <a :href="cookiesPolicyURL" target="_blank">
          {{ $t('updatedTermsAndConditions.link3') }}</a>.
      </p>
      <p> {{ $t('updatedTermsAndConditions.body2') }} </p>
    </div>
    <div :class="getErrorState()">
      <error-message v-if="hasTriedToContinue && !areTermsAccepted"
                     id="error_txt"
                     :class="$style.validationText">
        {{ $t('termsAndConditions.checkBoxError') }}
      </error-message>
      <generic-checkbox v-model="areTermsAccepted"
                        :selected="areTermsAccepted"
                        :check-box-classes="[$style.hideDefaultCheckbox]"
                        :a-labelled-by="getAriaLabel"
                        checkbox-id="agree_checkbox"
                        name="termsAndConditions"
                        @click="check">
        <label id="termsAndConditionsCheckboxLabel" @click="check">
          {{ $t('updatedTermsAndConditions.checkBoxText1') }}
          <a :href="termsAndConditionsURL" style="display: inline-block;"
             target="_blank" @click="stopProp($event)" >
            {{ $t('updatedTermsAndConditions.link1') }}</a> and
          <a :href="privacyPolicyURL" style="display: inline-block;"
             target="_blank" @click="stopProp($event)" >
            {{ $t('updatedTermsAndConditions.link2') }}</a>.
          {{ $t('updatedTermsAndConditions.checkBoxText2') }}
          <a :href="cookiesPolicyURL" style="display: inline-block;"
             target="_blank" @click="stopProp($event)" >
            {{ $t('updatedTermsAndConditions.link3') }}</a>.
        </label>
      </generic-checkbox>
    </div>
    <generic-button id="btn_accept" :class="[$style.button, $style.green]"
                    @click="onConfirmButtonClicked">
      {{ $t('updatedTermsAndConditions.btnAccept') }}
    </generic-button>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import CheckedIcon from '@/components/icons/CheckedIcon';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import GenericButton from '@/components/widgets/GenericButton';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import { INDEX } from '@/lib/routes';

export default {
  components: {
    CheckedIcon,
    GenericButton,
    ErrorMessage,
    MessageDialog,
    GenericCheckbox,
  },
  data() {
    return {
      termsAndConditionsURL: this.$store.app.$env.TERMS_AND_CONDITIONS_URL,
      privacyPolicyURL: this.$store.app.$env.PRIVACY_POLICY_URL,
      cookiesPolicyURL: this.$store.app.$env.COOKIES_POLICY_URL,
      areTermsAccepted: false,
      hasTriedToContinue: false,
    };
  },
  computed: {
    getAriaLabel() {
      return this.hasTriedToContinue && !this.areTermsAccepted ?
        'error_msg termsAndConditionsCheckboxLabel' : 'termsAndConditionsCheckboxLabel';
    },
  },
  methods: {
    check() {
      this.areTermsAccepted = !this.areTermsAccepted;
    },
    stopProp(event) {
      event.stopPropagation();
    },
    async onConfirmButtonClicked() {
      this.hasTriedToContinue = true;
      if (this.areTermsAccepted) {
        const consentRequest = {
          ConsentGiven: true,
          UpdatingConsent: true,
        };

        await this.$store.dispatch('termsAndConditions/acceptTerms', { consentRequest });

        if (this.$store.state.termsAndConditions.areAccepted) {
          const sourceValue = this.$store.state.device.source;
          window.location = `${window.location.origin}${INDEX.path}?source=${sourceValue}`;
        }
      }
    },
    getErrorState() {
      if (this.hasTriedToContinue && !this.areTermsAccepted) {
        return this.$style.validationBorderLeft;
      }
      return null;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../style/termsConditions";
</style>
