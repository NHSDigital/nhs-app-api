<template>
  <div>
    <message-dialog id="demographicsWarning"
                    message-type="warning"
                    :icon-text="$t('messageIconText.important')">
      <message-text>
        {{ $t('onlineConsultations.warning.text',
              { providerName: providerName }) }}
        <span><a id="online_consultations_help_link"
                 :href="onlineConsultationsUrl"
                 target="_blank" rel="noopener noreferrer">
          {{ $t('onlineConsultations.warning.link') }}</a>
        </span>
      </message-text>
    </message-dialog>
    <question>
      <div slot="question-slot" class="demographicsQuestion">
        <slot/>
      </div>
      <form @submit.prevent="demographicsContinueClicked">
        <generic-checkbox :key="code"
                          checkbox-id="demographics-checkbox"
                          :name="name"
                          :required="false"
                          :value="code"
                          @input="selectValueChanged()">
          <span>
            {{ $t('onlineConsultations.demographics.checkboxLabel') }}
            <span>
              <a :href="privacyPolicyUrl"
                 target="_blank" rel="noopener noreferrer">
                {{ $t('onlineConsultations.demographics.checkboxLink') }}.</a>
            </span>
          </span>
        </generic-checkbox>
        <generic-button id="demographicsContinueButton"
                        :button-classes="['nhsuk-button']"
                        click-delay="short"
                        @click.prevent="demographicsContinueClicked">
          {{ $t('onlineConsultations.orchestrator.continueButton') }}
        </generic-button>
      </form>
      <desktopGenericBackLink v-if="!isNativeApp"
                              data-purpose="back-link"
                              :path="backLink"
                              :button-text="$t('onlineConsultations.orchestrator.backButton')"
                              @clickAndPrevent="backClicked"/>
    </question>
  </div>
</template>

<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import Question from '@/components/online-consultations/Question';
import GenericButton from '@/components/widgets/GenericButton';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import {
  DEMOGRAPHICS_QUESTION_NAME,
  DEMOGRAPHICS_QUESTION_OPTION,
} from '@/lib/online-consultations/constants/nojsInputNames';
import {
  APPOINTMENT_BOOKING_GUIDANCE_PATH,
  APPOINTMENTS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import NativeApp from '@/services/native-app';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import {
  PRIVACY_POLICY_URL,
  ONLINE_CONSULTATIONS_PRIVACY_URL,
} from '@/router/externalLinks';

export default {
  name: 'DemographicsQuestion',
  components: {
    MessageDialog,
    MessageText,
    Question,
    GenericCheckbox,
    GenericButton,
    DesktopGenericBackLink,
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
    providerName: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      code: DEMOGRAPHICS_QUESTION_OPTION,
      name: DEMOGRAPHICS_QUESTION_NAME,
      isDemographicsAccepted: false,
      privacyPolicyUrl: PRIVACY_POLICY_URL,
      onlineConsultationsUrl: ONLINE_CONSULTATIONS_PRIVACY_URL,
    };
  },
  computed: {
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    bookingGuidancePath() {
      return APPOINTMENT_BOOKING_GUIDANCE_PATH;
    },
    backLink() {
      // TODO: revert to using previouspaths when routing plugin moved
      // const { previousPaths } = this.$router.history.router;
      // return previousPaths[previousPaths.length - 1];
      return APPOINTMENTS_PATH;
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
    backClicked() {
      redirectTo(this, this.backLink);
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

  #online_consultations_help_link {
    display: inline;
  }

  #demographicsContinueButton {
    margin-top: 1em;
  }
</style>
