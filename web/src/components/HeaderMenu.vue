<template>
  <header :class="$style.header">
    <nuxt-link :class="$style['anchor-icon']" :to="indexPath">
      <home-icon/>
    </nuxt-link>
    <a id="help_icon" :class="$style['anchor-icon']" :href="helpAndSupportURL" target="_blank">
      <help-icon/>
    </a>
    <nuxt-link v-if="showAccountIcon" :class="$style['anchor-icon']" :to="accountPath">
      <account-icon/>
    </nuxt-link>
    <hr :class="$style.rule">
    <h1 :class="$style.title">{{ $store.state.header.headerText }}</h1>
    <div :class="$style['sr-only']" role="presentation"
         aria-live="polite" aria-relevant="additions" aria-atomic="false">
      {{ $store.state.pageTitle.pageTitle + ' page' }}
    </div>
  </header>
</template>

<script>
/* eslint-disable no-unused-vars */
import { ACCOUNT, INDEX } from '@/lib/routes';
import AccountIcon from '../components/icons/AccountIcon';
import HelpIcon from '../components/icons/HelpIcon';
import HomeIcon from '../components/icons/HomeIcon';

export default {
  components: {
    AccountIcon,
    HomeIcon,
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
  data(app) {
    return {
      helpAndSupportURL: app.$env.HELP_AND_SUPPORT_URL,
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
};
</script>

<style module lang="scss" scoped>
@import "../style/colours";
@import "../style/textstyles";
@import "../style/accessibility";

.header {
  background: $nhs_blue;
  color: $white;
  position: fixed;
  top: 0em;
  left: 0em;
  right: 0em;
  height: 6.250em;
  box-shadow: 0em 0em 0.313em rgba(0, 0, 0, .5);
  z-index: 4;
  box-sizing: border-box;
  a.anchor-icon {
    color: $white;
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
