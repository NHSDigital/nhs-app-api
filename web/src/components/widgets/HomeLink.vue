<template>
  <div tabindex="-1">
    <nuxt-link ref="homeLogoEl"
               :class="$style['anchor-icon']"
               :to="indexPath">
      <home-icon tabindex="-1"/>
    </nuxt-link>
  </div>
</template>
<script>
import HomeIcon from '@/components/icons/HomeIcon';
import { INDEX } from '@/lib/routes';

export default {
  name: 'HomeLink',
  components: { HomeIcon },
  props: {
    indexPath: {
      default: INDEX.path,
      type: String,
    },
  },
  methods: {
    resetFocusToNhsLogo() {
    // No javascript handles the removing of focus fine
    // this line is for javascript side to handle it
      document.activeElement.blur();
    },
  },
};
</script>
<style module lang="scss" scoped>
    @import '../../style/colours';
    @import '../../style/screensizes';
    @import '../../style/textstyles';
    @import "../../style/fonts";

    header {
        background: $nhs_blue;
        color: $white;
        position: relative;
        height: auto;
        display: block;
        top: 0;
        left: 0;
        right: 0;
        z-index: 4;
        box-sizing: border-box;

        @include tablet() {
        }

        @include phone {
        }

        @include phone-and-below() {
        }
    }

  div {
    :focus {
      outline-style:none;
      box-shadow:none;
      border-color:transparent;
    }
    :focus:focus-within{
      outline-color: $focus_highlight !important;
      outline-width: thick;
      outline-style: solid !important;
      border-bottom-width: 5em;
      outline-offset: -5px;
    }
  }
</style>
