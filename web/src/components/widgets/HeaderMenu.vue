<template>
  <nav id="header-navigation"
       class="nhsuk-header__navigation" :class="[miniMenuExpanded && 'js-show',
                                                 'nojs-mini-menu-expanded']">
    <p class="nhsuk-header__navigation-title">
      <span id="label-navigation" style="color:#212b32;">Menu</span>
      <button id="close-menu" class="nhsuk-header__navigation-close"
              @click.prevent="closeMiniMenu"
              @keyup.enter="closeMiniMenu">
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
        <a class="nhsuk-header__navigation-link" :href="advicePath"
           data-sid="advice-menu-item"
           data-purpose="advicePageLink"
           @click.prevent="setMenuitemState(advicePath)">
          {{ $t('navigation.advice') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item">
        <a class="nhsuk-header__navigation-link" :href="appointmentsPath"
           data-sid="appointments-menu-item"
           data-purpose="appointmentsPageLink"
           @click.prevent="setMenuitemState(appointmentsPath)">
          {{ $t('navigation.appointments') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item">
        <a class="nhsuk-header__navigation-link" :href="prescriptionsPath"
           data-sid="prescriptions-menu-item"
           data-purpose="prescriptionsPageLink"
           @click.prevent="setMenuitemState(prescriptionsPath)">
          {{ $t('navigation.prescriptions') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item">
        <a class="nhsuk-header__navigation-link" :href="yourHealthPath"
           data-sid="your-health-menu-item"
           data-purpose="myRecordPageLink"
           @click.prevent="setMenuitemState(yourHealthPath)">
          {{ $t('navigation.yourHealth') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item">
        <a class="nhsuk-header__navigation-link" :href="messagesPath"
           data-sid="messages-menu-item"
           data-purpose="messagesPageLink"
           @click.prevent="setMenuitemState(messagesPath)">
          {{ $t('navigation.messages') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item" :class="$style.additionalMenuItem">
        <a class="nhsuk-header__navigation-link" :href="helpAndSupportUrl"
           data-sid="help-and-support-menu-item" target="_blank" rel="noopener noreferrer"
           @click="closeMiniMenu">
          {{ $t('navigation.helpAndSupport') }}
        </a>
      </li>
      <li class="nhsuk-header__navigation-item" :class="$style.additionalMenuItem">
        <a class="nhsuk-header__navigation-link" :href="accountPath"
           data-sid="account-menu-item"
           @click.prevent="setMenuitemState(accountPath)">
          {{ $t('navigation.account') }}
        </a>
      </li>
      <li :class="[$style.additionalMenuItem, 'nhsuk-header__navigation-item']">
        <a class="nhsuk-header__navigation-link" :href="logoutPath"
           data-sid="logout-menu-item"
           @click.prevent="setMenuitemState(logoutPath)">
          {{ $t('navigation.logout') }}
        </a>
      </li>
    </ul>
  </nav>
</template>

<script>
import {
  APPOINTMENTS_PATH,
  MESSAGES_PATH,
  ADVICE_PATH,
  PRESCRIPTIONS_PATH,
  HEALTH_RECORDS_PATH,
  ACCOUNT_PATH,
  LOGOUT_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'HeaderMenuWidget',
  props: {
    showMiniMenuOnSmallMedia: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      advicePath: ADVICE_PATH,
      appointmentsPath: APPOINTMENTS_PATH,
      prescriptionsPath: PRESCRIPTIONS_PATH,
      yourHealthPath: HEALTH_RECORDS_PATH,
      messagesPath: MESSAGES_PATH,
      shouldShowMiniMenu: true,
      accountPath: ACCOUNT_PATH,
      logoutPath: LOGOUT_PATH,
    };
  },
  computed: {
    miniMenuExpanded() {
      return this.$store.state.header.miniMenuExpanded;
    },
    helpAndSupportUrl() {
      return this.$route.meta.helpUrl;
    },
  },
  methods: {
    isMenuItemSelected(menuItemIndex) {
      return this.$store.state.navigation.menuItemStatusAt[menuItemIndex];
    },
    setMenuitemState(path) {
      redirectTo(this, path);
      this.closeMiniMenu();
    },
    closeMiniMenu() {
      this.$store.dispatch('header/closeMiniMenu');
    },
  },
  head() {
    return {
      noscript: [
        {
          innerHTML: `
            <style type="text/css">
              @media (max-width: 989px) {
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
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/header-menu";
</style>
