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
        <span>
          <!-- opening and closing tag must be on one line to
          avoid the inline-block white space issue
          -->
          <a :href="termsAndConditionsURL"
             target="_blank">{{ $t('updatedTermsAndConditions.link1') }}</a>
        </span>,
        <span>
          <a :href="privacyPolicyURL"
             target="_blank">{{ $t('updatedTermsAndConditions.link2') }}</a>
        </span>
        and
        <span>
          <a :href="cookiesPolicyURL"
             target="_blank">{{ $t('updatedTermsAndConditions.link3') }}</a>
        </span>.
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
                        checkbox-id="termsAndConditions-agree_checkbox">
        <span :class="$style.termsAndConditionsCaption">
          {{ $t('updatedTermsAndConditions.checkBoxText1') }}
          <span>
            <!-- opening and closing tag must be on one line
            to avoid the inline-block white space issue -->
            <a :href="termsAndConditionsURL" target="_blank"
               @click="stopProp($event)">{{ $t('updatedTermsAndConditions.link1') }}</a>
          </span>
          and
          <span>
            <a :href="privacyPolicyURL" target="_blank"
               @click="stopProp($event)">{{ $t('updatedTermsAndConditions.link2') }}</a>
          </span>.
          {{ $t('updatedTermsAndConditions.checkBoxText2') }}
          <span>
            <a :href="cookiesPolicyURL" target="_blank"
               @click="stopProp($event)">{{ $t('updatedTermsAndConditions.link3') }}</a>
          </span>.
        </span>
      </generic-checkbox>
    </div>
    <generic-button id="btn_accept" :class="[$style.button, $style.green]"
                    @click="onConfirmButtonClicked">
      {{ $t('updatedTermsAndConditions.btnAccept') }}
    </generic-button>
  </div>
</template>

<script>
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import MessageDialog from '@/components/widgets/MessageDialog';
import { INDEX } from '@/lib/routes';

export default {
  name: 'UpdatedTermsConditions',
  components: {
    ErrorMessage,
    GenericButton,
    GenericCheckbox,
    MessageDialog,
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
  methods: {
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
