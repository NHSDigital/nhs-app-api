<template>
  <a :class="$style.desktopBackLink"
     :href="path"
     data-purpose="main-back-button"
     @click.prevent="$emit('clickAndPrevent', $event)">
    {{ $t(getBackButtonText) }}</a>
</template>
<script>
export default {
  name: 'DesktopGenericBackButton',
  props: {
    path: {
      type: String,
      default: undefined,
    },
    buttonText: {
      type: String,
      default: 'generic.backButton.text',
    },
  },
  computed: {
    getBackButtonText() {
      return this.buttonText;
    },
  },
};
</script>
<style module lang="scss" scoped>
 @import "../../style/textstyles";
 @import "../../style/fonts";
 @import "../../style/colours";

 .desktopBackLink {
  font-family: $default-web;
  color: $nhs_blue;
  font-size: 1.125em;
  line-height: 1.125em;
  font-weight: normal;
  vertical-align: middle;
  cursor: pointer;
  display: inline-block;
  border: none;
  background: none;
  outline: none;
  text-decoration: underline;
  margin-top: 1em;
  margin-bottom: 2em;
 }

 .desktopBackLink:focus{
  box-sizing: content-box;
  outline-color: $focus_highlight;
  box-shadow: 0 0 0 4px $focus_highlight;
  outline-width: 2em;
 }
 .desktopBackLink:hover{
  background: #ffcd60;
  outline: none;
  box-sizing: border-box;
  text-decoration: underline;
  background-clip: content-box;
 }

</style>
