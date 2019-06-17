<template>
  <span :class="$style.pageTitle">
    <h1 v-if="shouldShowDesktopVersion" :key="$store.state.header.headerText"
        :class="$style.homePageTitle" aria-live="polite">
      {{ $store.state.header.headerText }}
    </h1>
  </span>
</template>
<script>
export default {
  name: 'PageTitle',
  props: {
    shouldShowDesktopVersion: {
      type: Boolean,
      default: () => false,
    },
  },
};
</script>
<style module lang="scss" scoped>
 @import "../../style/fonts";
 @import "../../style/textstyles";
 @import '../../style/screensizes';

.pageTitle {
 display: block;
 @include main-container-width;

 .homePageTitle {
  color: #212B32;
  text-align: left;
  font-family: $frutiger-roman;
  font-size: 2.250em;
  font-weight: 600;
  line-height: 1.250em;
  padding: 1em 16px 16px;

 }
}
</style>
