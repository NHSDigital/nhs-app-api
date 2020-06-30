<template>
  <div>
    <cookie-banner v-if="!loggedIn" id="cookie-banner"/>
    <skip-link v-if="!$store.state.device.isNativeApp" />
    <header class="nhsuk-header" role="banner">
      <div class="nhsuk-width-container nhsuk-header__container">
        <div class="nhsuk-header__logo nhsuk-header__logo--only">
          <NhsHeaderLogo id="nhs-header-logo" :index-path="indexPath"/>
        </div>
        <div id="web-content-header" class="nhsuk-header__content">
          <div v-if="showMenu" class="nhsuk-header__menu">
            <button id="toggle-menu"
                    class="nhsuk-header__menu-toggle"
                    :class="$style.menuButton"
                    aria-controls="header-navigation"
                    :aria-label="$t('webHeader.toggleMenu.ariaLabel')"
                    @click.prevent="toggleMiniMenu"
                    @keyup.enter="toggleMiniMenu">{{ $t('webHeader.toggleMenu.buttonText') }}
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
  ACCOUNT,
  executeHomeNavigationRule,
  LOGOUT,
} from '@/lib/routes';

export default {
  name: 'WebHeader',
  components: {
    CookieBanner,
    HeaderLinks,
    HeaderMenu,
    NhsHeaderLogo,
    SkipLink,
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
  },
  data() {
    return {
      helpAndSupportURL: this.$store.app.$env.HELP_AND_SUPPORT_URL,
      links: [
        { name: this.$t('webHeader.links.account'), value: ACCOUNT.path, id: 'account-link' },
        { name: this.$t('webHeader.links.logout'), value: LOGOUT.path, id: 'account-logout' },
      ],
      showMenuButton: false,
    };
  },
  computed: {
    indexPath() {
      return executeHomeNavigationRule(this.$route.name);
    },
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
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
    min-width: 300px;

    .header-content {
     display: block;

     .nhsLogo {
      color: $white;
      display: inline-block;

      a.anchor-icon {
       color: $white;
       display: inline-block;

      }
     }

     a.mini-menu-toggler {
      display: none;

      :focus {
       background-color: transparent;
       border-color: $focus_highlight;
       box-shadow: 0 0 0 3px $focus_highlight;
      }
     }
    }
    .menuButton {
      margin-right: 0;
    }

    @include fromTablet() {
      .menuButton {
        margin-right: 0;
      }
    }

    @include tabletAndBelow() {
      .header-content {
        a.mini-menu-toggler {
          float: right;
          margin-top: 0.8em;
          display: inline-block;
          font-family: $frutiger-roman;
          font-weight: 700;
          line-height: 2em;
          color: $white;
          font-size: 1.125em;
          border: 1px $white solid;
          border-radius: 0.5em;
          padding: 0.125em 0.8em;
          margin-left: 1em;
          cursor: pointer;

          &:focus {
            box-shadow: 0 0 0 4px $focus_highlight;
          }

          &:hover {
            background: $hover_blue;
            box-shadow: 0 0 0 4px $focus_highlight;
            text-decoration: none;
          }
        }
      }
    }
  }
</style>
