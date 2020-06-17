<template>
  <div v-if="anchorLinks.length>0" :class="$style['header-links']">
    <span v-for="(anchorLink, index) in anchorLinks" :key="index">
      <router-link :id="anchorLink.id"
                   :to="anchorLink.value"
                   tabindex="0">
        {{ anchorLink.name }}
      </router-link>
      <strong v-if="index<anchorLinks.length-1" aria-hidden="true">
        &nbsp;|&nbsp;
      </strong>
    </span>
  </div>
</template>

<script>
export default {
  name: 'HeaderLinks',
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
        text-decoration: none;
        display: inline-block;
        color: $white;
        padding: 0.5em;

        &:visited, &:active {
          color: $white;
        }

        &:hover {
          background: $hover_blue;
          box-shadow: none;
          color: $white;
          text-decoration: underline;
        }
      }

      b {
        @include account;
        display: inline-block;
        color: $white;
      }
    }

   @include fromLargeDesktop {
     display: inline-block;
   }
  }

</style>
