<template>
  <div>
    <cookie-banner v-if="!loggedIn"/>
    <header>
      <span :class="$style['header-content']">
        <div :class="$style.nhsLogo">
          <HomeLink ref="headerHomeLink" :index-path="indexPath"/>
        </div>
        <a v-if="showMenuButton"
           :class="$style['mini-menu-toggler']"
           tabindex="0"
           data-sid="mini-menu"
           @click.prevent="toggleMiniMenu()"
           @keyup.enter="toggleMiniMenu()">
          Menu
        </a>
        <header-links v-if="showLinks" :anchor-links="links"/>
        <header-menu v-if="showMenu" />
      </span>
    </header>

    <bread-crumb-trail :routes="currentBreadCrumbs"/>

    <div :class="$style.headerLowerSection">
      <page-title v-if="!isLoginPage"
                  :should-show-desktop-version="!$store.state.device.isNativeApp"/>
      <header-companion-button/>
    </div>
  </div>
</template>

<script>
/* eslint-disable no-unused-vars */
import {
  executeHomeNavigationRule,
  getCrumbTrailForRoute,
  findByName,

  ACCOUNT,
  LOGIN,
  LOGOUT,
} from '@/lib/routes';
import HeaderLinks from '@/components/widgets/HeaderLinks';
import HeaderMenu from '@/components/widgets/HeaderMenu';
import HeaderCompanionButton from '@/components/widgets/HeaderCompanionButton';
import HomeLink from './HomeLink';
import PageTitle from './PageTitle';
import CookieBanner from '../CookieBanner';
import BreadCrumbTrail from '@/components/widgets/BreadCrumbTrail';

export default {
  components: {
    BreadCrumbTrail,
    HomeLink,
    HeaderLinks,
    HeaderMenu,
    PageTitle,
    CookieBanner,
    HeaderCompanionButton,
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
    showMenu: {
      type: Boolean,
      default: true,
    },
    showLinks: {
      type: Boolean,
      default: true,
    },
    showHeaderButtons: {
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
    currentBreadCrumbs() {
      return getCrumbTrailForRoute(findByName(this.$route.name));
    },
    isLoginPage() {
      return this.$route.name === LOGIN.name;
    },
    accountPath() {
      return ACCOUNT.path;
    },
    indexPath() {
      return executeHomeNavigationRule(this.$route.name);
    },
    loggedIn() {
      return !!this.$store.state.session.csrfToken;
    },
  },
  mounted() {
    this.showMenuButton = process.client && this.showMenu;
  },
  methods: {
    toggleMiniMenu() {
      this.$store.dispatch('header/toggleMiniMenu');
    },
    resetFocusToNhsLogo() {
      const headerHomeLinkCompt = this.$refs.headerHomeLink;
      if (headerHomeLinkCompt) {
        headerHomeLinkCompt.resetFocusToNhsLogo();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/colours';
  @import '../../style/screensizes';
  @import '../../style/textstyles';
  @import "../../style/fonts";
  @import "../../style/desktopcomponentsizes";
div {
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
   @include main-container-width;
   display: block;
   margin: 0 auto;
   padding: 0 16px;

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
     outline-color: $focus_highlight;
     background-color: transparent;
     border-color: $focus_highlight;
     box-shadow: 0 0 0 3px $focus_highlight;
    }
   }
  }

   @include tablet-and-above {
   .header-content {
    margin-left: 32px;
   }
  }

  @include desktop {
   .header-content {
    margin: 0 auto;
   }
  }


  @include phone-and-below() {
   .header-content {
    a.mini-menu-toggler {
     float: right;
     margin-top: 0.8em;
     display: inline-block;
     font-family: $frutiger-roman;
     font-weight: 700;
     line-height: 2em;
     color: $white;
     @include kerneliOS;
     font-size: 1.125em;
     border: 1px $white solid;
     border-radius: 0.5em;
     padding: 0.125em 0.8em;
     margin-left: 1em;
     cursor: pointer;

     &:focus {
      box-shadow: 0 0 0 4px $focus_highlight;
      outline-color: $focus_highlight;
      outline-width: thick;
     }

     &:hover {
      background: #003d78;
      box-shadow: 0 0 0 4px $focus_highlight;
      text-decoration: none;
     }
    }
   }
  }
 }

 .headerLowerSection{
  display: block;
  @include main-container-width;
  width: auto;
  position: relative;
 }

  @include tablet-and-above {
  .headerLowerSection{
   margin-left: 2em;
  }
 }

 @include desktop {
  .headerLowerSection{
   margin: 0 auto;
  }
 }
}
</style>
