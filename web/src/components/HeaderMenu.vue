<template>
  <header :class="$style.header">
    <nuxt-link to="/">
      <home-icon/>
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
      title: `${this.$store.state.header.headerText} screen`,
    };
  },
};
</script>

<style lang="scss" module>
@import "../style/colours";
@import "../style/textstyles";

.header {
  background: $nhs_blue;
  position: fixed;
  top: 0px;
  left: 0px;
  right: 0px;
  height: 100px;
  z-index: 1000;
  box-shadow: 0px 0px 5px rgba(0, 0, 0, 0.5);
}

.title {
  @include screen_title;
  text-align: center;
}

.rule {
  margin: 54px 16px 9px 16px;
  height: 1px;
  border: none;
  background-color: $white;
  opacity: 0.4;
}
</style>
