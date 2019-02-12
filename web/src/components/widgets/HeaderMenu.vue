<template>
  <nav :class="[$style.menu, miniMenuExpanded ? $style.expanded : $style.collapsed,
                'nojs-mini-menu-expanded']">
    <hr aria-hidden="true">
    <a v-if="miniMenuExpanded" :class="$style['mini-menu-close-button']"
       role="button" tabindex="0"
       @click.prevent="closeMiniMenu"
       @keyup.enter="closeMiniMenu">Menu</a>

    <noscript inline-template>
      <div :class="$style['menu-nojs-caption']">Menu</div>
    </noscript>
    <ul>
      <li>
        <a :class="$style['navMenuItem']" :href="symptomsPath"
           data-sid="symptoms-menu-item"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.symptomsLabel') }}
        </a>
      </li>
      <li>
        <a :class="$style['navMenuItem']" :href="appointmentsPath"
           data-sid="appointments-menu-item"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.appointmentsLabel') }}
        </a>
      </li>
      <li>
        <a :class="$style['navMenuItem']" :href="prescriptionsPath"
           data-sid="prescriptions-menu-item"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.prescriptionsLabel') }}
        </a>
      </li>
      <li>
        <a :class="$style['navMenuItem']" :href="recordPath"
           data-sid="myrecord-menu-item"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.myRecordLabel') }}
        </a>
      </li>
      <li>
        <a :class="$style['navMenuItem']" :href="morePath"
           data-sid="more-menu-item"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.moreLabel') }}
        </a>
      </li>
      <li :class="$style.additionalMenuItem">
        <a :class="$style['navMenuItem']" :href="accountPath"
           data-sid="account-menu-item"
           @click.prevent="setMenuitemState($event)">
          {{ $t('navigationMenu.accountLabel') }}
        </a>
      </li>
      <li :class="$style.additionalMenuItem">
        <a :class="$style['navMenuItem']" :href="logoutPath"
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

export default {
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
        this.$router.push(a.pathname);
      }
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

  nav.menu {
    overflow: hidden;
    height: auto;
    display: none;
    :focus {
      outline-color: $focus_highlight;
      box-shadow: inset 0 0 0 4px $focus_highlight;
    }

    @include tablet-and-above() {
      & {
        display: block;
      }

     .additionalMenuItem{
      display: none;
     }
    }

    .menu-nojs-caption {
      display: none;
    }

    a.mini-menu-close-button {
      display: none;
      :focus {
        outline-color: $focus_highlight;
        box-shadow: inset 0 0 0 4px $focus_highlight;
        outline-offset: -5px;
      }
    }

    & > ul {
      display: flex;
      flex-wrap: wrap;

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

    @include phone-and-below {
      &.expanded {
        display: block;
      }

      &.collapsed {
        display: none;
      }

      hr {
        display: none;
      }

      $parent-left-right-padding: 16px;

      background: $white;
      margin: 0 (-1 * $parent-left-right-padding);

      .menu-nojs-caption {
        @include mini-menu-option;
        cursor: default;
      }

      a.mini-menu-close-button {
        @include mini-menu-option;
        background: $white url('~/assets/close-menu.svg') no-repeat center right;
        background-position: right 1em center;

        &:visited,
        &:active {
          color: $black;
        }

        &:hover {
          background: $white url('~/assets/close-menu-hover.svg') no-repeat center right;
          background-position: right 1em center;
          box-shadow: none;
          color: $black;
          text-decoration: underline;
        }
      }

      & > ul {
        display: block;
        border-bottom: 3px $background solid;

        li {
          display: block;
          margin: 0;
          color: $black;
          border-bottom: 1px $background solid;
          text-align: center;

          :focus {
            outline-color: $focus_highlight;
            box-shadow: inset 0 0 0 4px $focus_highlight;
          }

          a {
            @include default_text;
            text-align: left;
            font-family: $default-web;
            color: $nhs_blue;
            padding: 1em;
            background: $white url('~/assets/icon_arrow_left.svg') no-repeat center right;
            background-position: right 1em center;

            &:visited,
            &:active {
              color: $nhs_blue;
            }

            &:hover {
              box-shadow: none;
              background: $nhs_blue;
              color: $white;
              text-decoration: underline;
              background-repeat: no-repeat;
              background-image: url('~/assets/icon_arrow_white_left.svg');
              background-position: right 1em center;
            }
          }
        }
      }
    }
  }
</style>
