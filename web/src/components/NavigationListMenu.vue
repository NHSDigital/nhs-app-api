<template>
  <ul :class="[$style.listMenu, !$store.state.device.isNativeApp && $style.desktopWeb]"
      data-sid="navigation-list-menu">
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.symptoms')"
                             :href="symptomsPath"
                             :click-func="goToUrl"
                             :click-param="symptomsPath"
                             data-sid="symptoms-list-item"
                             tag="a">
        {{ $t('navigationMenuList.symptoms') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.appointments')"
                             :href="appointmentsPath"
                             :click-func="goToUrl"
                             :click-param="appointmentsPath"
                             data-sid="appointments-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.appointments') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.prescriptions')"
                             :href="prescriptionsPath"
                             :click-func="goToUrl"
                             :click-param="prescriptionsPath"
                             data-sid="prescriptions-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.prescriptions') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.myRecord')"
                             :href="myRecordPath"
                             :click-func="goToUrl"
                             :click-param="myRecordPath"
                             data-sid="myrecord-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.myRecord') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <organ-donation-link id="organ-donation-link"
                           :class-name="[$style.listMenuAnchor]"
                           data-sid="organ-donation-menu-item"
                           tag="a">
        {{ $t('navigationMenuList.organDonation') }}
      </organ-donation-link>
    </li>
  </ul>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import OrganDonationLink from '@/components/organ-donation/OrganDonationLink';
import { SYMPTOMS, PRESCRIPTIONS, APPOINTMENTS, MYRECORD } from '@/lib/routes';

export default {
  name: 'NavigationListMenu',
  components: {
    OrganDonationLink,
    AnalyticsTrackedTag,
  },
  data() {
    return {
      organDonationUrl: this.$store.app.$env.ORGAN_DONATION_URL,
    };
  },
  computed: {
    symptomsPath() {
      return SYMPTOMS.path;
    },
    appointmentsPath() {
      return APPOINTMENTS.path;
    },
    prescriptionsPath() {
      return PRESCRIPTIONS.path;
    },
    myRecordPath() {
      return MYRECORD.path;
    },
  },
};
</script>

<style module lang="scss">
  @import "../style/accessibility";
  @import "../style/colours";
  @import "../style/fonts";
  @import "../style/nhsukoverrides";

  .listMenu {
    border-top: 1px #D8DDE0 solid;
    list-style: none;
    font-size: 1em;
    margin-bottom: 1em;

    &.desktopWeb {
      .listMenuItem {
        font-family: $default-web;
        font-weight: lighter;
        margin: 0;
        padding: 0.25em 0 0.25em 0;
        @extend .focusBorder;
      }

      .listMenuAnchor {
        font-family: $default-web;
        font-weight: normal;
        line-height: 1.5em;

        &:hover {
          color: $black;
        }
      }
    }
  }

  .listMenuItem {
    font-weight: normal;
    padding: 0.5em 0;
    box-sizing: border-box;
    background-repeat: no-repeat;
    background-position: right 1em center;
    background-image: url('~assets/icon_arrow_left.svg');
    background-color: #f0f4f5;
    position: relative;
    margin-bottom: 0;
    margin-left: 0;
    border-bottom: 1px #D8DDE0 solid;
    font-size: 1em;
    line-height: 1.5em;
    color: #212B32;
  }

  .listMenuAnchor {
    &.web {
      font-weight: bold;
      line-height: 1em;
    }
    padding-right: 3em;
    padding-bottom: 0.5em;
    padding-top: 0.5em;
    display: block;
    color: #005EB8;
    font-size: 1em;
    text-decoration: none;
  }
</style>
