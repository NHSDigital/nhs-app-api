<template>
  <div v-if="anchorLinks.length>0" :class="$style['header-links']">
    <span v-for="(anchorLink, index) in anchorLinks" :key="index">
      <nuxt-link :id="anchorLink.id"
                 :to="anchorLink.value"
                 tabindex="0">
        {{ anchorLink.name }}
      </nuxt-link>
      <b v-if="index<anchorLinks.length-1" aria-hidden="true">
        &nbsp;|&nbsp;
      </b>
    </span>
  </div>
</template>

<script>
/* eslint-disable no-unused-vars */
export default {
  props: {
    anchorLinks: {
      type: Array,
      default: () => [],
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/colours";
  @import "../../style/textstyles";
  @import "../../style/fonts";
  @import "../../style/screensizes";

  .header-links {
    font-family: $default-web;
    font-weight: 100;
    float: right;
    display: none;
    margin-top: 1.5em;
    :focus {
      outline-color: $focus_highlight;
      box-shadow: inset 0 0 0 4px $focus_highlight;
      outline-offset: -5px;
    }

    span {
      margin: 0;

      a {
        @include account;
        font-family: $default-web;
        font-weight: 100;
        font-size: 1em;
        text-decoration: underline;
        display: inline-block;
        color: $white;
        padding: 0.5em;

        &:visited, &:active {
          color: $white;
        }

        &:hover {
          background: #003d78;
          box-shadow: none;
          color: $white;
          text-decoration: underline;
        }
      }

      b {
        @include account;
        font-size: 1em;
        display: inline-block;
        color: $white;
      }
    }

   @include tablet-and-above {
     display: inline-block;
   }
  }

</style>
