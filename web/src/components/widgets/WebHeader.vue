<template>
  <div>
    <cookie-banner v-if="!loggedIn" id="cookie-banner"/>
    <skip-link v-if="!$store.state.device.isNativeApp" />
    <header class="nhsuk-header" role="banner">
      <div class="nhsuk-width-container nhsuk-header__container">
        <div class="nhsuk-header__logo nhsuk-header__logo--only">
          <NhsHeaderLogo id="nhs-header-logo" :index-path="indexPath"
                         :is-logo-link="isLogoLink"/>
        </div>
        <div id="web-content-header" class="nhsuk-header__content">
          <div v-if="showMenu" class="nhsuk-header__menu">
            <button id="toggle-menu"
                    class="nhsuk-header__menu-toggle"
                    :class="$style.menuButton"
                    aria-controls="header-navigation"
                    :aria-label="$t('navigation.header.openMenu')"
                    @click.prevent="toggleMiniMenu"
                    @keyup.enter="toggleMiniMenu">{{ $t('navigation.header.menu') }}
            </button>
          </div>
          <header-links v-if="showLinks" :anchor-links="links"/>
        </div>
      </div>
      <header-menu v-if="showMenu" />
    </header>
  </div>
</template>

<script>

import CookieBanner from '@/components/CookieBanner';
import HeaderLinks from '@/components/widgets/HeaderLinks';
import HeaderMenu from '@/components/widgets/HeaderMenu';
import NhsHeaderLogo from '@/components/widgets/NhsHeaderLogo';
import SkipLink from '@/components/widgets/SkipLink';
import {
  LOGOUT_PATH,
  MORE_PATH,
  executeHomeNavigationRule,
} from '@/router/paths';
import { createRoutePathObject } from '@/lib/utils';

export default {
  name: 'WebHeader',
  components: {
    CookieBanner,
    HeaderLinks,
    HeaderMenu,
    NhsHeaderLogo,
    SkipLink,
  },
  props: {
    showHeaderButtons: {
      type: Boolean,
      default: true,
    },
    showLinks: {
      type: Boolean,
      default: true,
    },
    showMenu: {
      type: Boolean,
      default: true,
    },
    isLogoLink: {
      type: Boolean,
      default: true,
    },
  },
  data() {
    return {
      morePath: createRoutePathObject({
        path: MORE_PATH,
        store: this.$store,
      }),
      logoutPath: createRoutePathObject({
        path: LOGOUT_PATH,
        store: this.$store,
      }),
    };
  },
  computed: {
    indexPath() {
      return executeHomeNavigationRule(this.$route.name);
    },
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
    },
    links() {
      return [
        { name: this.$t('navigation.header.helpAndSupport'), value: this.$route.meta.helpUrl, id: 'help-and-support-link' },
        { name: this.$t('navigation.header.more'), value: this.morePath, id: 'more-link', internal: true },
        { name: this.$t('navigation.header.logout'), value: this.logoutPath, id: 'account-logout', internal: true },
      ];
    },
  },
  methods: {
    toggleMiniMenu() {
      this.$store.dispatch('header/toggleMiniMenu');
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/web-header";
</style>
