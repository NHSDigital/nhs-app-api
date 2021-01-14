<template>
  <div class="nhsuk-action-link">
    <analytics-tracked-tag v-if="isAnalyticsTracked"
                           :id="id"
                           class="nhsuk-action-link__link"
                           :click-func="clickFunc"
                           :href="linkUrl"
                           :target="openNewWindow ? '_blank' : undefined"
                           :text="bannerText"
                           tag="a">
      <nhs-arrow-circle />
      <span class="nhsuk-action-link__text break">{{ bannerText }}</span>
    </analytics-tracked-tag>
    <a v-else
       :id="id"
       class="nhsuk-action-link__link"
       :href="linkUrl"
       :target="openNewWindow ? '_blank' : undefined"
       @click="doClick"
       @keypress.enter="doClick" >
      <nhs-arrow-circle />
      <span class="nhsuk-action-link__text break">{{ bannerText }}</span>
    </a>
  </div>
</template>
<script>
import isFunction from 'lodash/fp/isFunction';
import isString from 'lodash/fp/isString';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import NhsArrowCircle from '@/components/icons/NHSArrowCircle';
import { getDynamicStyle, exchangeStyle } from '@/lib/desktop-experience';

export default {
  name: 'SymptomBanner',
  components: {
    AnalyticsTrackedTag,
    NhsArrowCircle,
  },
  props: {
    bannerText: {
      type: String,
      default: () => '',
    },
    clickAction: {
      type: [Function, String],
      required: true,
    },
    id: {
      type: String,
      default: undefined,
    },
    isAnalyticsTracked: {
      type: Boolean,
      default: () => false,
    },
    openNewWindow: {
      type: Boolean,
      default: () => true,
    },
  },
  computed: {
    linkUrl() {
      return isString(this.clickAction) ? this.clickAction : '#';
    },
    clickFunc() {
      return isFunction(this.clickAction) ? this.clickAction : undefined;
    },
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args, exchangeStyle({ button: 'btn_home_symptoms-desktop' }));
    },
    doClick(e) {
      if (isFunction(this.clickAction)) {
        e.preventDefault();
        this.clickAction(e);
      }
    },
  },
};
</script>

<style lang="scss">
  @import "@/style/custom/break";
</style>
