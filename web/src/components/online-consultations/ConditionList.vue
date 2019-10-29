<template>
  <div :class="!isNativeApp && $style.desktopWeb">
    <div id="conditionInfo" :class="$style.info" data-purpose="info">
      <p>{{ $t('appointments.gp_advice.conditions.paragraph') }}</p>
      <form method="post">
        <input type="hidden"
               :name="selectedConditionName"
               :value="generalGpAdviceServiceDefinition">
        <input v-if="demographicsConsentGiven"
               type="hidden"
               :name="demographicsConsentName"
               :value="demographicsConsentGiven">
        <button role="link"
                :class="$style.link"
                @click.prevent="onConditionClicked(generalGpAdviceServiceDefinition)">
          {{ $t('appointments.gp_advice.conditions.link') }}
        </button>
      </form>
    </div>
    <div v-for="serviceDefinition in serviceDefinitions"
         :key="serviceDefinition.category"
         class="conditionCategory">
      <h2>{{ serviceDefinition.category }}</h2>
      <menu-item-list>
        <form v-for="serviceDefinitionItem in serviceDefinition.items"
              :key="`${serviceDefinitionItem.id}-${serviceDefinitionItem.title}`"
              method="post"
              role="listitem">
          <input type="hidden"
                 :name="selectedConditionName"
                 :value="serviceDefinitionItem.id">
          <input v-if="demographicsConsentGiven"
                 type="hidden"
                 :name="demographicsConsentName"
                 :value="demographicsConsentGiven">
          <menu-item :id="serviceDefinitionItem.id"
                     :text="serviceDefinitionItem.title"
                     tag="button"
                     :click-func="onConditionClicked"
                     :click-param="serviceDefinitionItem.id"/>
        </form>
      </menu-item-list>
    </div>
    <form method="post">
      <input type="hidden"
             :name="selectedConditionName"
             :value="generalGpAdviceServiceDefinition">
      <input v-if="demographicsConsentGiven"
             type="hidden"
             :name="demographicsConsentName"
             :value="demographicsConsentGiven">
      <button role="link"
              :class="$style.link"
              @click.prevent="onConditionClicked(generalGpAdviceServiceDefinition)">
        {{ $t('appointments.gp_advice.conditions.link') }}
      </button>
    </form>
    <form :action="indexPath">
      <generic-button id="endMyConsultationButton"
                      :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                      @click.prevent="endMyConsultationClicked">
        {{ $t('onlineConsultations.orchestrator.endMyConsultationButton') }}
      </generic-button>
    </form>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import GenericButton from '@/components/widgets/GenericButton';
import { INDEX } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import {
  DEMOGRAPHICS_QUESTION_NAME,
  NHSAPP_SELECTED_CONDITION,
} from '@/lib/online-consultations/constants/nojsInputNames';
import NativeApp from '@/services/native-app';
import { EventBus, FOCUS_NHSAPP_ROOT } from '@/services/event-bus';

export default {
  name: 'ConditionList',
  components: {
    MenuItem,
    MenuItemList,
    GenericButton,
  },
  props: {
    serviceDefinitions: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
      indexPath: INDEX.path,
      selectedConditionName: NHSAPP_SELECTED_CONDITION,
      demographicsConsentName: DEMOGRAPHICS_QUESTION_NAME,
      provider: this.$store.state.serviceJourneyRules.rules.cdssAdvice.provider,
      generalGpAdviceServiceDefinition:
        this.$store.state.serviceJourneyRules.rules.cdssAdvice.serviceDefinition,
      demographicsConsentGiven: this.$store.state.onlineConsultations.demographicsConsentGiven,
    };
  },
  methods: {
    async onConditionClicked(serviceDefinitionId) {
      await this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', {
        serviceDefinitionId,
        provider: this.provider,
        answeringConditionsQuestion: true,
      });
      this.$store.dispatch('onlineConsultations/setGpAdviceServiceDefinitionId', serviceDefinitionId);
      if (this.isNativeApp) {
        NativeApp.resetPageFocus();
      } else {
        EventBus.$emit(FOCUS_NHSAPP_ROOT);
      }
      window.scrollTo(0, 0);
    },
    endMyConsultationClicked() {
      redirectTo(this, this.indexPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/accessibility';
  @import '../../style/listmenu';
  @import '../../style/webshared';
  @import '../../style/info';
  @import '../../style/textstyles';
  @import '../../style/fonts';
  @import '../../style/nhsukoverrides';
  @import '../../style/fonts';
  @import '../../style/buttons';

  div.desktopWeb {
    .cannotFindConditionLink {
      margin-bottom: 1.5em !important;
    }
  }

  .warningText {
    font-family: $default_web;
    font-weight: normal;
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
