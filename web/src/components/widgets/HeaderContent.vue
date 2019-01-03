<template>
  <span :class="$style['header-content']">
    <div :class="$style.nhsLogo">
      <HomeLink ref="headerHomeLink" :index-path="indexPath"/>
    </div>
    <a v-if="showMenuButton"
       :class="$style['mini-menu-toggler']"
       @click.prevent="toggleMiniMenu"
       @keyup.enter="toggleMiniMenu">
      Menu
    </a>
    <header-links :anchor-links="links"/>
    <header-menu v-if="showMenu" ref="menu"/>
  </span>
</template>
<script>
import HeaderLinks from '@/components/widgets/HeaderLinks';
import HeaderMenu from '@/components/widgets/HeaderMenu';
import { ACCOUNT, INDEX, LOGOUT } from '@/lib/routes';
import HomeLink from './HomeLink';

export default {
  name: 'HeaderContent',
  components: { HeaderLinks, HeaderMenu, HomeLink },
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
      default: false,
    },
  },
  data() {
    return {
      helpAndSupportURL: this.$store.app.$env.HELP_AND_SUPPORT_URL,
      links: [
        { name: 'Account', value: ACCOUNT.path },
        { name: 'Log out', value: LOGOUT.path },
      ],
      showMenuButton: false,
    };
  },
  computed: {
    accountPath() {
      return ACCOUNT.path;
    },
    indexPath() {
      return INDEX.path;
    },
    shouldShowDesktopVersion() {
      return (this.$store.state.device.source !== 'android' && this.$store.state.device.source !== 'ios');
    },
  },
  mounted() {
    if (process.client) {
      this.$refs.menu.toggleMiniMenu();
      this.showMenuButton = !this.showMenuButton;
    }
  },
  methods: {
    toggleMiniMenu() {
      this.$refs.menu.toggleMiniMenu();
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

 span {
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

  .header-content {
   max-width: 960px;
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

  @include tablet() {
   .header-content {
    margin: 0 32px;
   }
  }

  @include phone {
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
</style>
