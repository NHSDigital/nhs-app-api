<template>
  <header :class="$style.header">
    <nuxt-link ref="homeLogoEl" :class="$style['anchor-icon']" :to="indexPath" tabindex="-1">
      <home-icon/>
    </nuxt-link>
    <a id="help_icon" :class="[$style['anchor-icon'], $style['fixed-right']]"
       :href="helpAndSupportURL" target="_blank"
       tabindex="-1">
      <help-icon/>
    </a>
    <nuxt-link v-if="showAccountIcon" :class="[$style['anchor-icon'], $style['fixed-right']]"
               :to="accountPath" tabindex="-1">
      <account-icon/>
    </nuxt-link>
    <hr :class="$style.rule">
    <h1 :class="$style.title">{{ $store.state.header.headerText }}</h1>
  </header>
</template>

<script>
/* eslint-disable no-unused-vars */
import { ACCOUNT, INDEX } from '@/lib/routes';
import AccountIcon from '../components/icons/AccountIcon';
import HelpIcon from '../components/icons/HelpIcon';

export default {
  name: 'HeaderMenu',
  components: {
    AccountIcon,
    HelpIcon,
  },
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - NHS App`,
    };
  },
  props: {
    showAccountIcon: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      helpAndSupportURL: this.$store.app.$env.HELP_AND_SUPPORT_URL,
    };
  },
  computed: {
    accountPath() {
      return ACCOUNT.path;
    },
    indexPath() {
      return INDEX.path;
    },
  },
  methods: {
    resetFocusToNhsLogo() {
      this.$refs.homeLogoEl.$el.focus();
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../style/colours";
@import "../style/textstyles";

.header {
  background: $nhs_blue;
  color: $white;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 6.250em;
  box-shadow: 0 0 0.313em rgba(0, 0, 0, .5);
  z-index: 4;
  box-sizing: border-box;
  a {
   &.anchor-icon {
     color: $white;
   }
   &.fixed-right {
     position: fixed;
     top: 0;
     right: 0;
     &:nth-of-type(2) {
       right: 2.4em;
     }
   }
 }
  h1.title {
    @include screen_title;
    text-align: center;
    box-sizing: border-box;
    width: 100%;
  }

  .rule {
    margin: 54px 16px 9px 16px;
    height: 1px;
    border: none;
    background-color: $white;
    opacity: 0.25;
  }
}

</style>
