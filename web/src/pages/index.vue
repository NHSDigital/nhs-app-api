<template>
  <div v-if="showTemplate" :class="[$style['no-padding'], 'pull-content']">
    <beta-banner data-sid="beta-flag"/>
    <hr :class="$style.rule" aria-hidden="true">
    <h2 :class="$style.header" data-purpose="greeting">{{ greetingMessage }}</h2>
    <welcome-section />
    <navigation-list-menu
      nhs-number="nhsNumber"
      dob="dob"
    />
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import BetaBanner from '../components/BetaBanner';
import WelcomeSection from '../components/WelcomeSection';
import NavigationListMenu from '../components/NavigationListMenu';

export default {
  components: {
    BetaBanner,
    WelcomeSection,
    NavigationListMenu,
  },
  computed: {
    greetingMessage() {
      const message = this.$t('homeLoggedIn.welcome');
      const { user } = this.$store.state.session;
      return `${message}, ${user}`;
    },
  },
  mounted() {
    window.scrollTo(0, 0);
    if (this.$store.state.device.isNativeApp) {
      window.nativeApp.showHeader();
      window.nativeApp.hideWhiteScreen();
    }
  },
};
</script>

<style module lang="scss">
.no-padding {
  width: 102%;
  height: 100%;
  margin-left: -16px;
  margin-right: 0px;
  margin-top: -8px;
}
.header {
  margin-left: 0.7em;
  margin-top: 0.5em;
}
.rule {
  height: 0.063em;
  border: none;
  background-color: #D8DDE0;
}

</style>
