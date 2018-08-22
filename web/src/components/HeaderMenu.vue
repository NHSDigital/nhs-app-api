<template>
  <header :class="$style.header">
    <nuxt-link to="/">
      <home-icon/>
    </nuxt-link>
    <nuxt-link :class="$style.account" to="/account">
      Account
    </nuxt-link>
    <nuxt-link to="/account">
      <account-icon/>
    </nuxt-link>
    <hr :class="$style.rule">
    <h1 :class="$style.title">{{ $store.state.header.headerText }}</h1>
  </header>
</template>

<script>
/* eslint-disable import/extensions */
import AccountIcon from '../components/icons/AccountIcon';
import HomeIcon from '../components/icons/HomeIcon';

export default {
  components: {
    AccountIcon,
    HomeIcon,
  },
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: `${this.$store.state.pageTitle.pageTitle} - NHS App`,
    };
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
  top: 0em;
  left: 0em;
  right: 0em;
  height: 6.250em;
  box-shadow: 0em 0em 0.313em rgba(0, 0, 0, .5);
  z-index: 4;
  box-sizing: border-box;
  a.account {
    @include account;
    margin: 1em;
    display: inline-block;
    height: 1.375em;
    position: absolute;
    right: 2.3em;
    line-height: 2.2em;
    cursor: pointer;
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
