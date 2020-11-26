<template>
  <h1 :key="titleKey"
      ref="nhsAppHeader"
      :class="[cssClass, 'break']"
      tabindex="-1">
    <span v-if="caption"
          :key="caption"
          :class="[captionSizeCss, 'nhsuk-caption--top']"
          data-purpose="header-caption">
      {{ caption }}
    </span>
    <slot/>
  </h1>
</template>
<script>
import CaptionSize from '@/lib/caption-size';
import { EventBus, FOCUS_NHSAPP_TITLE } from '@/services/event-bus';

export default {
  name: 'PageTitle',
  props: {
    cssClass: {
      type: String,
      default: 'nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-0 nhsuk-u-padding-top-3',
    },
    titleKey: {
      type: String,
      default: undefined,
    },
    caption: {
      type: String,
      default: undefined,
    },
    captionSize: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    captionSizeCss() {
      if (this.captionSize) {
        return this.captionSize;
      }
      return CaptionSize.Large;
    },
  },
  beforeMount() {
    EventBus.$on(FOCUS_NHSAPP_TITLE, this.focusNhsAppHeader);
  },
  beforeDestroy() {
    EventBus.$off(FOCUS_NHSAPP_TITLE, this.focusNhsAppHeader);
  },
  updated() {
    this.focusNhsAppHeader();
  },
  mounted() {
    this.focusNhsAppHeader();
  },
  methods: {
    focusNhsAppHeader() {
      if (document.activeElement !== null) {
        document.activeElement.blur();
      }
      this.$refs.nhsAppHeader.focus();
    },
  },
};
</script>
<style>
.break {
  word-break: break-word;
}
</style>
