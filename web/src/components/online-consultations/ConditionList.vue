<template>
  <div v-if="shouldShowContent" :class="!isNativeApp && $style.desktopWeb">
    <div id="conditionInfo" data-purpose="info">
      <p id="conditionListTitle">
        {{ $t(`onlineConsultations.conditions.${conditionsListTitle}`) }}
      </p>
      <p v-if="generalGpAdviceServiceDefinition"
         class="nhsuk-u-margin-0 nhsuk-u-padding-0">
        <a id="cannotFindConditionLinkTop"
           class="cannotFindConditionLink"
           role="link" href="#"
           @click.prevent="onConditionClicked(generalGpAdviceServiceDefinition)">
          {{ $t(`onlineConsultations.conditions.${defaultConditionLinkText}`) }}</a>
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
    <p v-if="generalGpAdviceServiceDefinition"
       id="endConditionInfo"
       class="nhsuk-u-margin-0 nhsuk-u-padding-0">
      <a id="cannotFindConditionLinkBottom"
         class="cannotFindConditionLink"
         role="link" href="#"
         @click.prevent="onConditionClicked(generalGpAdviceServiceDefinition)">
        {{ $t(`onlineConsultations.conditions.${defaultConditionLinkText}`) }}
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
import { redirectTo, CHILD_DEFAULT_SERVICE_DEFINITION } from '@/lib/utils';
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
        this.$store.state.onlineConsultations.defaultCondition,
    };
  },
  computed: {
    shouldShowContent() {
      return this.serviceDefinitions.length > 1;
    },
    isChildJourney() {
      return this.$store.state.onlineConsultations.childJourneySelected;
    },
    defaultConditionLinkText() {
      return this.generalGpAdviceServiceDefinition === CHILD_DEFAULT_SERVICE_DEFINITION
        ? 'iCannotFindMyChildsCondition' : 'iCannotFindMyCondition';
    },
    conditionsListTitle() {
      return this.isChildJourney ? 'chooseChildsCondition' : 'chooseYourCondition';
    },
  },
  async created() {
    if (this.serviceDefinitions.length > 1) {
      this.$store.dispatch('pageLeaveWarning/shouldSkipDisplayingLeavingWarning', false);
    } else {
      const serviceDefinitionId = this.serviceDefinitions[0].items[0].id;

      this.$store.dispatch('onlineConsultations/setServiceDefinitionId', serviceDefinitionId);
      await this.$store.dispatch('onlineConsultations/evaluateServiceDefinition', {
        serviceDefinitionId,
        provider: this.provider,
        answeringConditionsQuestion: true,
      });
    }
  },
  methods: {
    async onConditionClicked(serviceDefinitionId) {
      this.$store.dispatch('onlineConsultations/setServiceDefinitionId', serviceDefinitionId);
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
  a.cannotFindConditionLink {
    display: inline-block;
    cursor: pointer;
  }
</style>
<style module lang="scss" scoped>
  @import "@/style/custom/condition-list";
</style>
