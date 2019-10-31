<template>
  <div v-if="showTemplate">
    <message-dialog v-if="isError" role="alert">
      <message-text data-purpose="error-heading"
                    :is-header="true">
        {{ $t('appointments.gp_advice.errors.header') }}
      </message-text>
      <message-text data-purpose="reason-error"
                    :aria-label="$t('appointments.gp_advice.errors.message.label')">
        {{ $t('appointments.gp_advice.errors.message.text') }}
      </message-text>
    </message-dialog>
    <div v-else>
      <message-dialog v-if="!demographicsQuestionAnswered"
                      id="conditionWarning"
                      message-type="warning" icon-text="Important">
        <span :class="[$style.warningText, $style.msgText]">
          {{ $t('appointments.admin_help.warning.warningText',
                { providerName: getProviderName }) }}
          <span>
            <a id="online_consultations_help_link"
               :href="onlineConsultationsURL"
               target="_blank">{{ $t('appointments.admin_help.warning.warningLink') }}</a>
          </span>
        </span>
      </message-dialog>
      <div id="conditionInfo" :class="$style.info" data-purpose="info">
        <p>{{ $t('appointments.gp_advice.conditions.paragraph') }}</p>
        <a :href="generalAdvicePath"
           role="link"
           @click.prevent="onConditionClicked()">
          {{ $t('appointments.gp_advice.conditions.link') }}
        </a>
      </div>
      <div v-for="serviceDefinition in serviceDefinitions"
           :key="serviceDefinition.category">
        <h2>{{ serviceDefinition.category }}</h2>
        <menu-item-list>
          <menu-item v-for="serviceDefinitionItem in serviceDefinition.items"
                     :id="serviceDefinitionItem.id"
                     :key="serviceDefinitionItem.title"
                     :href="getConditionHref(serviceDefinitionItem.id)"
                     :text="serviceDefinitionItem.title"
                     :aria-label="serviceDefinitionItem.title"
                     :click-func="onConditionClicked"
                     :click-param="serviceDefinitionItem.id"/>
        </menu-item-list>
      </div>
    </div>
    <a :href="generalAdvicePath"
       :class="$style.cannotFindConditionLink"
       role="link"
       @click.prevent="onConditionClicked()">
      {{ $t('appointments.gp_advice.conditions.link') }}
    </a>
    <desktopGenericBackLink v-if="!isNativeApp"
                            :path="backPath"
                            data-purpose="back-button"
                            @clickAndPrevent="backClicked"/>
    <generic-button v-else
                    :button-classes="['button', 'grey']"
                    click-delay="short"
                    @click.prevent="backClicked">
      {{ $t('onlineConsultations.orchestrator.backButton') }}
    </generic-button>
  </div>
</template>
<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { APPOINTMENT_BOOKING_GUIDANCE, APPOINTMENT_GP_ADVICE } from '@/lib/routes';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  name: 'Conditions',
  components: {
    MenuItem,
    MenuItemList,
    MessageDialog,
    MessageText,
    GenericButton,
    DesktopGenericBackLink,
  },
  computed: {
    serviceDefinitions() {
      return this.$store.state.onlineConsultations.serviceDefinitions;
    },
    onlineConsultationsURL() {
      return this.$store.app.$env.ONLINE_CONSULTATIONS_URL;
    },
    isError() {
      return this.$store.state.onlineConsultations.error;
    },
    isNativeApp() {
      return this.$store.state.device.isNativeApp;
    },
    generalAdviceServiceDefinitionId() {
      return this.$store.state.serviceJourneyRules.rules.cdssAdvice.serviceDefinition;
    },
    generalAdvicePath() {
      return `${APPOINTMENT_GP_ADVICE.path}?serviceDefinitionId=${this.generalAdviceServiceDefinitionId}`;
    },
    getProviderName() {
      return this.$store.state.onlineConsultations.adviceProviderName;
    },
    backPath() {
      return this.$store.state.onlineConsultations.previousRoute ||
        APPOINTMENT_BOOKING_GUIDANCE.path;
    },
  },
  async asyncData({ store }) {
    await store.dispatch('onlineConsultations/getServiceDefinitions', {
      provider: store.state.serviceJourneyRules.rules.cdssAdvice.provider,
    });
  },
  mounted() {
    document.activeElement.blur();
  },
  methods: {
    getConditionHref(id) {
      return `${APPOINTMENT_GP_ADVICE.path}?serviceDefinitionId=${id}`;
    },
    backClicked() {
      redirectTo(this, this.backPath, null);
    },
    onConditionClicked(serviceDefinitionId) {
      this.$store.dispatch(
        'onlineConsultations/setGpAdviceServiceDefinitionId',
        serviceDefinitionId || this.generalAdviceServiceDefinitionId,
      );
      this.$router.push(APPOINTMENT_GP_ADVICE.path);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../../style/accessibility";
  @import "../../../style/listmenu";
  @import "../../../style/webshared";
  @import "../../../style/info";
  @import '../../../style/textstyles';
  @import '../../../style/fonts';
  @import "../../../style/nhsukoverrides";
  @import '../../../style/fonts';
  @import "../../../style/textstyles";

  .msgText {
    padding: 1em 1em 0.150em 1em;
    @include message;
  }

  div.desktopWeb {
    .cannotFindConditionLink {
      margin-bottom: 1.5em !important;
    }
  }

  .warningText {
    font-family: $default_web;
    font-weight: normal;

    a {
      display: inline;
    }
  }

  button {
    margin-top: 1.5em;
  }

  .info {
    font-size: 1em;
    margin-bottom: 1em;
    padding-top: 1em;

    p {
      font-family: $default_web;
      font-weight: normal;
    }
  }

</style>
