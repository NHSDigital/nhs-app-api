<template>
  <header>
    <span :class="$style['header-content']">
      <nuxt-link ref="homeLogoEl" :class="$style['anchor-icon']" :to="indexPath" tabindex="-1">
        <home-icon />
      </nuxt-link>
      <a v-if="showMenu"
         :class="$style['mini-menu-toggler']"
         data-sid="mini-menu"
         @click.prevent="toggleMiniMenu()">
        {{ $t('navigationMenu.menuLabel') }}
      </a>
    </span>
  </header>
</template>

<script>
/* eslint-disable no-unused-vars */
import { ACCOUNT, LOGOUT, INDEX } from '@/lib/routes';
import HomeIcon from '@/components/icons/HomeIcon';

export default {
  components: {
    HomeIcon,
  },
  head() {
    return {
      htmlAttrs: {
        lang: `${this.$t('language')}`,
      },
      title: this.$t('webHeader.title', {
        pageTitle: this.$store.state.pageTitle.pageTitle,
      }),
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
  },
  data() {
    return {
      helpAndSupportURL: this.$store.app.$env.HELP_AND_SUPPORT_URL,
      links: [
        { name: 'Account', value: ACCOUNT.path },
        { name: 'Log out', value: LOGOUT.path },
      ],
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
    toggleMiniMenu() {
      this.$refs.menu.toggleMiniMenu();
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/colours';
  @import '../../../style/screensizes';
  @import '../../../style/textstyles';

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

    .header-content {
      max-width: 960px;
      display: block;
      margin: 0 auto;
      padding: 0 16px;

      a.anchor-icon {
        color: $white;
        display: inline-block;
        margin: 1em 0;
        height: 2.5em;

        svg {
          margin: 0;
          height: 40px;
          width: 99px;
        }

        &:focus {
          box-shadow: 0 0 0 4px $focus_highlight;
        }
      }

      a.mini-menu-toggler {
        display: none;
      }
    }
  }

  @media (min-width: 48.0625em) {
    header {
      .header-content {
        margin-left: 32px;
      }
    }
  }

  @media (min-width: 1024px) {
    header {
      .header-content {
        margin: 0 auto;
      }
    }
  }
</style>
