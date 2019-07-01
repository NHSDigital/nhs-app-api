<template>
  <nav class="nhsuk-header__navigation" :class="[miniMenuExpanded && 'js-show',
                                                 'nojs-mini-menu-expanded']">
    <p class="nhsuk-header__navigation-title">
      <span id="label-navigation">Menu</span>
      <button id="close-menu" class="nhsuk-header__navigation-close"
              @click.prevent="closeMiniMenu"
              @keyup.13="closeMiniMenu">
        <svg class="nhsuk-icon nhsuk-icon__close" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" aria-hidden="true" focusable="false">
          <path
            d="M13.41 12l5.3-5.29a1 1 0 1 0-1.42-1.42L12 10.59l-5.29-5.3a1 1 0 0 0-1.42
                  1.42l5.3 5.29-5.3 5.29a1 1 0 0 0 0 1.42 1 1 0 0 0 1.42 0l5.29-5.3 5.29 5.3a1 1
                  0 0 0 1.42 0 1 1 0 0 0 0-1.42z"/>
        </svg>
        <span class="nhsuk-u-visually-hidden">Close menu</span>
      </button>
    </p>
    <ul class="nhsuk-header__navigation-list">
      <li class="nhsuk-header__navigation-item nhsuk-header__navigation-item--for-mobile">
        <a class="nhsuk-header__navigation-link" :href="symptomsPath"
           data-sid="symptoms-menu-item"
           data-purpose="symptomsPageLink"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.symptomsLabel') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item">
        <a class="nhsuk-header__navigation-link" :href="appointmentsPath"
           data-sid="appointments-menu-item"
           data-purpose="appointmentsPageLink"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.appointmentsLabel') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item">
        <a class="nhsuk-header__navigation-link" :href="prescriptionsPath"
           data-sid="prescriptions-menu-item"
           data-purpose="prescriptionsPageLink"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.prescriptionsLabel') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item">
        <a class="nhsuk-header__navigation-link" :href="recordPath"
           data-sid="myrecord-menu-item"
           data-purpose="myRecordPageLink"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.myRecordLabel') }}
        </a>
      </li>
      <!-- Hidden as not in scope for mvp, will be needed in future -->
      <li v-if="false" class="nhsuk-header__navigation-item">
        <a class="nhsuk-header__navigation-link" :href="morePath"
           data-sid="more-menu-item"
           data-purpose="morePageLink"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.moreLabel') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item" :class="$style.additionalMenuItem">
        <a class="nhsuk-header__navigation-link" :href="accountPath"
           data-sid="account-menu-item"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.accountLabel') }}
        </a>
      </li>
      <li :class="[$style.additionalMenuItem, 'nhsuk-header__navigation-item']">
        <a class="nhsuk-header__navigation-link" :href="logoutPath"
           data-sid="logout-menu-item"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.logoutLabel') }}
        </a>
      </li>
    </ul>
  </nav>
</template>

<script>
import { SYMPTOMS, APPOINTMENTS, PRESCRIPTIONS, MYRECORD, MORE, ACCOUNT, LOGOUT } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'HeaderMenuWidget',
  props: {
    showMiniMenuOnSmallMedia: {
      type: Boolean,
      default: false,
    },
  },
  head() {
    return {
      noscript: [
        {
          innerHTML: `
            <style type="text/css">
              @media (max-width: 767px) {
                .nojs-mini-menu-expanded {
                  display: block !important;
                }
              }
            </style>`,
          body: false,
        },
      ],
      __dangerouslyDisableSanitizers: ['noscript'],
    };
  },
  data() {
    return {
      symptomsPath: SYMPTOMS.path,
      appointmentsPath: APPOINTMENTS.path,
      prescriptionsPath: PRESCRIPTIONS.path,
      recordPath: MYRECORD.path,
      morePath: MORE.path,
      shouldShowMiniMenu: true,
      accountPath: ACCOUNT.path,
      logoutPath: LOGOUT.path,
    };
  },
  computed: {
    miniMenuExpanded() {
      return this.$store.state.header.miniMenuExpanded;
    },
  },
  methods: {
    isMenuItemSelected(menuItemIndex) {
      return this.$store.state.navigation.menuItemStatusAt[menuItemIndex];
    },
    setMenuitemState(event) {
      const a = event.currentTarget;
      this.$store.app.$analytics.trackButtonClick(a.pathname);
      if (a.target === '_blank') {
        window.open(a.href, '_blank');
      } else {
        redirectTo(this, a.pathname, null);
      }
      this.closeMiniMenu();
    },
    closeMiniMenu() {
      this.$store.dispatch('header/closeMiniMenu');
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../style/colours';
  @import '../../style/screensizes';
  @import '../../style/textstyles';
  @import "../../style/fonts";

  @mixin mini-menu-option {
    padding: 1em 0.8em 1em 1em;
    display: block;
    font-size: 1em;
    line-height: 1.5em;
    font-family: $frutiger-bold;
    border-bottom: 1px $background solid;
    color: $black;
    font-weight: 400;
    cursor: pointer;
  }

  nav{
    overflow: hidden;
    height: auto;
    display: none;
    :focus {
      outline-color: $focus_highlight;
      box-shadow: inset 0 0 0 4px $focus_highlight;
    }

    .menu-nojs-caption {
      display: none;
    }

   @include fromLargeDesktop() {
    & {
     display: block;
    }

    .additionalMenuItem{
     display: none;
    }

    & > ul {
     display: flex;
     flex-wrap: wrap;
     list-style: none;

     li {
      display: inline-block;
      margin: 0;
      color: $white;
      flex-grow: 1;
      text-align: center;
      :focus {
       outline-color: $focus_highlight;
       box-shadow: inset 0 0 0 4px $focus_highlight;
       outline-offset: -5px;
      }

      a {
       @include default_text_web;
       font-weight: normal;
       font-size: 1em;
       line-height: 1.5em;
       font-family: $default-web;
       color: $white;
       padding: 1em;

       &:visited,
       &:active {
        color: $white;
       }

       &:hover {
        background: #003d78;
        box-shadow: none;
        color: #FFFFFF;
        text-decoration: underline;
       }
      }
     }
    }
   }

    a.mini-menu-close-button {
      display: none;
      :focus {
        outline-color: $focus_highlight;
        box-shadow: inset 0 0 0 4px $focus_highlight;
        outline-offset: -5px;
      }
    }

    @include tabletAndBelow {

      hr {
        display: none;
      }

      background: $white;

      .menu-nojs-caption {
        @include mini-menu-option;
        cursor: default;
      }

    }
  }
</style>
