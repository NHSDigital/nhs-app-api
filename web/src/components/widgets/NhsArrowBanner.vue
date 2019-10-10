<template>
  <div class="nhsuk-action-link nhsuk-action-link__text">
    <analytics-tracked-tag
      v-if="isAnalyticsTracked"
      :id="id"
      class="nhsuk-action-link__link"
      :href="linkUrl"
      :text="bannerText"
      :target="openNewWindow? '_blank' : ''"
      tag="a">
      <Nhs-Arrow-Circle />{{ bannerText }}
    </analytics-tracked-tag>
    <a v-else :id="id" class="nhsuk-action-link__link nhsuk-action-link__text" :href="linkUrl"
       :target="openNewWindow? '_blank' : ''">
      <Nhs-Arrow-Circle />{{ bannerText }}
    </a>
  </div>
</template>
<script>

import NhsArrowCircle from '@/components/icons/NHSArrowCircle';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { getDynamicStyle, exchangeStyle } from '@/lib/desktop-experience';

export default {
  name: 'SymptomBanner',
  components: {
    AnalyticsTrackedTag,
    NhsArrowCircle,
  },
  props: {
    isAnalyticsTracked: {
      type: Boolean,
      default: () => false,
    },
    id: {
      type: String,
      default: () => '',
    },
    linkUrl: {
      type: String,
      default: () => '/',
    },
    bannerText: {
      type: String,
      default: () => '',
    },
    openNewWindow: {
      type: Boolean,
      default: () => true,
    },
  },
  methods: {
    dynamicStyle(...args) {
      return getDynamicStyle(this, args, exchangeStyle({ button: 'btn_home_symptoms-desktop' }));
    },
  },
};
</script>
