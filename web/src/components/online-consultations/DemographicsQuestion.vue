<template>
  <div>
    <message-dialog id="demographicsWarning"
                    message-type="warning"
                    :icon-text="$t('messageIconText.important')">
      <p class="nhsuk-u-margin-3 nhsuk-u-padding-left-2 nhsuk-u-margin-bottom-0">
        {{ $t('onlineConsultations.warning.warningText',
              { providerName: providerName }) }}
        <span><a id="online_consultations_help_link"
                 :href="onlineConsultationsUrl"
                 target="_blank" rel="noopener noreferrer">
          {{ $t('onlineConsultations.warning.warningLink') }}</a>
        </span>
      </p>
    </message-dialog>
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
      </no-js-form>
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
import Question from '@/components/online-consultations/Question';
import NoJsForm from '@/components/no-js/NoJsForm';
import GenericButton from '@/components/widgets/GenericButton';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';
import {
  ANSWERING_DEMOGRAPHICS_NAME,
  DEMOGRAPHICS_QUESTION_NAME,
  DEMOGRAPHICS_QUESTION_OPTION,
} from '@/lib/online-consultations/constants/nojsInputNames';
import { APPOINTMENT_BOOKING_GUIDANCE } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';
import NativeApp from '@/services/native-app';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

export default {
  name: 'DemographicsQuestion',
  components: {
    MessageDialog,
    Question,
    NoJsForm,
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
      answeringDemographicsName: ANSWERING_DEMOGRAPHICS_NAME,
      name: DEMOGRAPHICS_QUESTION_NAME,
      isDemographicsAccepted: false,
      privacyPolicyUrl: this.$store.app.$env.PRIVACY_POLICY_URL,
      onlineConsultationsUrl: this.$store.app.$env.ONLINE_CONSULTATIONS_URL,
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
    bookingGuidancePath() {
      return APPOINTMENT_BOOKING_GUIDANCE.path;
    },
    backLink() {
      const { previousPaths } = this.$router.history.router;
      return previousPaths[previousPaths.length - 1];
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
