<template>
  <div>
    <message-dialog id="demographicsWarning"
                    message-type="warning"
                    :icon-text="$t('generic.important')">
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
                              :button-text="'onlineConsultations.orchestrator.backButton'"
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
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import {
  PRIVACY_POLICY_URL,
  ONLINE_CONSULTATIONS_PRIVACY_URL,
} from '@/router/externalLinks';
import last from 'lodash/fp/last';

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
    backLink() {
      return last(this.$router.history.router.previousPaths);
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

      EventBus.$emit(FOCUS_NHSAPP_TITLE);
      window.scrollTo(0, 0);
    },
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/demographics-question";
</style>
