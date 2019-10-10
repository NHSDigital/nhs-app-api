<template>
  <span :class="$style.pageTitle">
    <h1 v-if="shouldShowDesktopVersion" :key="titleKey"
        aria-live="polite"
        class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-0 nhsuk-u-padding-top-3">
      <slot/>
    </h1>
  </span>
</template>
<script>
export default {
  name: 'PageTitle',
  props: {
    titleKey: {
      type: String,
      default: undefined,
    },
    shouldShowDesktopVersion: {
      type: Boolean,
      default: true,
    },
  },
};
</script>
<style module lang="scss" scoped>
 @import "../../style/fonts";
 @import "../../style/textstyles";

 .pageTitle {
   display: block;

   .homePageTitle {
     color: #212B32;
     text-align: left;
     padding-top: 16px;
     padding-bottom: 16px;
   }
 }
</style>
