<template>
  <span :class="$style.pageTitle">
    <h1 v-if="shouldShowDesktopVersion" :key="titleKey"
        :class="$style.homePageTitle" aria-live="polite">
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
     font-family: $frutiger-roman;
     font-size: 2em;
     font-weight: 600;
     line-height: 1.250em;
     padding-top: 16px;
     padding-bottom: 16px;
   }
 }
</style>
