<template>
  <div :class="!isNativeApp && $style.desktopWeb">
    <div id="conditionInfo" data-purpose="info">
      <p>{{ $t('onlineConsultations.conditions.chooseYourCondition') }}</p>
      <p class="nhsuk-u-margin-0 nhsuk-u-padding-0">
        <a id="cannotFindConditionLink"
           role="link"
           @click.prevent="onConditionClicked(generalGpAdviceServiceDefinition)">
          {{ $t('onlineConsultations.conditions.iCannotFindMyCondition') }}</a>
      </p>
    </div>
    <div v-for="serviceDefinition in serviceDefinitions"
         :key="serviceDefinition.category"
         class="conditionCategory">
      <h2>{{ serviceDefinition.category }}</h2>
      <menu-item-list>
        <menu-item v-for="serviceDefinitionItem in serviceDefinition.items"
                   :id="serviceDefinitionItem.id"
                   :key="`${serviceDefinitionItem.id}-${serviceDefinitionItem.title}`"
                   :header-tag="headerTag"
                   :text="serviceDefinitionItem.title"
                   tag="button"
                   :click-func="onConditionClicked"
                   :click-param="serviceDefinitionItem.id"/>
      </menu-item-list>
    </div>
    <p id="endConditionInfo" class="nhsuk-u-margin-0 nhsuk-u-padding-0">
      <a id="cannotFindConditionLink"
         role="link"
         @click.prevent="onConditionClicked(generalGpAdviceServiceDefinition)">
        {{ $t('onlineConsultations.conditions.iCannotFindMyCondition') }}
      </a>
    </p>
    <generic-button id="endMyConsultationButton"
                    :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                    @click.prevent="endMyConsultationClicked">
      {{ $t('onlineConsultations.orchestrator.endMyConsultationButton') }}
    </generic-button>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import GenericButton from '@/components/widgets/GenericButton';
import { INDEX_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';

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
    headerTag: {
      type: String,
      default: 'h2',
    },
  },
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
      indexPath: INDEX_PATH,
      provider: this.$store.state.serviceJourneyRules.rules.cdssAdvice.provider,
      generalGpAdviceServiceDefinition:
        this.$store.state.serviceJourneyRules.rules.cdssAdvice.serviceDefinition,
    };
  },
  created() {
    this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);
  },
  methods: {
    async onConditionClicked(serviceDefinitionId) {
      await this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', {
        serviceDefinitionId,
        provider: this.provider,
        answeringConditionsQuestion: true,
      });
      this.$store.dispatch('onlineConsultations/setGpAdviceServiceDefinitionId', serviceDefinitionId);
      EventBus.$emit(FOCUS_NHSAPP_TITLE);
      window.scrollTo(0, 0);
    },
    endMyConsultationClicked() {
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', true);
      redirectTo(this, this.indexPath);
    },
  },
};
</script>
<style>
  a#cannotFindConditionLink {
    display: inline-block;
    cursor: pointer;
  }
</style>
<style module lang="scss" scoped>
  @import '../../style/textstyles';
  @import '../../style/nhsukoverrides';
  @import '../../style/buttons';

  div.desktopWeb {
    .cannotFindConditionLink {
      margin-bottom: 1.5em !important;
    }
  }

  button {
    margin-top: 1.5em;
  }

</style>
