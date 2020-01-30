<template>
  <h1 v-if="shouldShowDesktopVersion" :key="titleKey"
      aria-live="polite"
      :class="cssClass">
    <slot/>
  </h1>
</template>
<script>
export default {
  name: 'PageTitle',
  props: {
    shouldShowDesktopVersion: {
      type: Boolean,
      default: true,
    },
    cssClass: {
      type: String,
      default: 'nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-0 nhsuk-u-padding-top-3',
    },
    titleKey: {
      type: String,
      default: undefined,
    },
  },
};
</script>
