<template>
  <div class="nhsuk-action-link nhsuk-action-link__text">
    <analytics-tracked-tag
      v-if="isAnalyticsTracked"
      :id="id"
      class="nhsuk-action-link__link"
      :click-func="clickFunc"
      :href="linkUrl"
      :target="openNewWindow? '_blank' : ''"
      :text="bannerText"
      tag="a">
      <Nhs-Arrow-Circle tabindex="-1" />{{ bannerText }}
    </analytics-tracked-tag>
    <a v-else :id="id" class="nhsuk-action-link__link nhsuk-action-link__text" :href="linkUrl"
       :target="openNewWindow? '_blank' : ''"
       @click="doClick"
       @keypress.enter="doClick" >
      <Nhs-Arrow-Circle tabindex="-1" />{{ bannerText }}
    </a>
  </div>
</template>
<script>
import { isFunction, isString } from 'lodash/fp';
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
