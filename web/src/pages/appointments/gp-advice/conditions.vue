<template>
  <div v-if="showTemplate" :class="!isNativeApp && $style.desktopWeb">
    <message-dialog v-if="isError" role="alert">
      <message-text data-purpose="error-heading"
                    :is-header="true">
        {{ $t('appointments.admin_help.errors.header') }}
      </message-text>
      <message-text data-purpose="reason-error"
                    :aria-label="$t('appointments.admin_help.errors.message.label')">
        {{ $t('appointments.admin_help.errors.message.text') }}
      </message-text>
    </message-dialog>
    <div v-else>
      <div id="conditionInfo" :class="$style.info" data-purpose="info">
        <p>{{ $t('appointments.gp_advice.conditions.paragraph') }}</p>
        <a :href="generalAdvicePath"
           role="link"
           @click.prevent="onConditionClicked()">
          {{ $t('appointments.gp_advice.conditions.link') }}
        </a>
      </div>
      <div v-for="serviceDefinition in serviceDefinitions"
           :key="serviceDefinition.category"
           :class="$style['no-padding']">
        <h2>{{ serviceDefinition.category }}</h2>
        <ul :class="$style['list-menu']">
          <menu-item v-for="serviceDefinitionItem in serviceDefinition.items"
                     :id="serviceDefinitionItem.id"
                     :key="serviceDefinitionItem.title"
                     :href="getConditionHref(serviceDefinitionItem.id)"
                     :text="serviceDefinitionItem.title"
                     :aria-label="serviceDefinitionItem.title"
                     :click-func="onConditionClicked"
                     :click-param="serviceDefinitionItem.id"
                     :class="$style.serviceDefinitionList"/>
        </ul>
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
      {{ $t('appointments.admin_help.orchestrator.backButton') }}
    </generic-button>
  </div>
</template>
<script>
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import { APPOINTMENT_BOOKING_GUIDANCE, APPOINTMENT_GP_ADVICE } from '@/lib/routes';
import MenuItem from '@/components/MenuItem';
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'Conditions',
  components: {
    MessageDialog,
    MessageText,
    MenuItem,
    GenericButton,
    DesktopGenericBackLink,
  },
  computed: {
    serviceDefinitions() {
      return this.$store.state.onlineConsultations.serviceDefinitions;
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

  div.desktopWeb {
    max-width: 540px;

    .cannotFindConditionLink {
      margin-bottom: 1.5em !important;
    }
  }

  button {
    margin-top: 1.5em;
  }

  .info {
    font-size: 1em;
    margin-bottom: 1em;
    padding-top: 1em;
    max-width: 540px;

    p {
      font-family: $default_web;
      font-weight: normal;
    }
  }

  .no-padding {
    margin-top: -0.5em;
    margin-left: -1em;
    margin-right: -1em;
    padding-bottom: 1em;

    p,
    h2 {
      margin-left: 0.7em;
    }
  }

  .serviceDefinitionList {
   padding-bottom: 10px
  }

</style>
