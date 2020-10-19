<template>
  <h1 :key="titleKey" aria-live="polite" :class="[cssClass, 'break']">
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
};
</script>
<style>
.break {
  word-break: break-word;
}
</style>
