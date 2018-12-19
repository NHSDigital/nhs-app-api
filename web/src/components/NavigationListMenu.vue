<template>
  <ul :class="$style.listMenu" data-sid="navigation-list-menu">
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.symptoms')"
                             :href="symptomsPath"
                             data-sid="symptoms-list-item"
                             tag="a">
        {{ $t('navigationMenuList.symptoms') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.appointments')"
                             :href="appointmentsPath"
                             data-sid="appointments-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.appointments') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.prescriptions')"
                             :href="prescriptionsPath"
                             data-sid="prescriptions-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.prescriptions') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.myRecord')"
                             :href="myRecordPath"
                             data-sid="myrecord-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.myRecord') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <organ-donation-link id="organ-donation-link"
                           :class-name="$style.listMenuAnchor"
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
@import "../style/colours";
  .listMenu {
    border-top: 1px #D8DDE0 solid;
    list-style: none;
    font-size: 1em;
    margin-bottom: 1em;
  }
  .listMenuItem {
    box-sizing: border-box;
    background-repeat: no-repeat;
    background-position: right 1em center;
    background-image: url('~/assets/icon_arrow_left.svg');
    background-color: #f0f4f5;
    position: relative;
    margin-bottom: 0em;
    margin-left: 0em;
    padding: 0.5em 0em 0.5em 1em;
    border-bottom: 1px #D8DDE0 solid;
    font-size: 1em;
    font-weight: normal;
    line-height: 1.5em;
    color: #212B32;
    :focus {
      outline-color: $focus_highlight;
    }
  }
  .listMenuAnchor {
    font-size: 1em;
    padding-right: 3em;
    padding-bottom: 0.5em;
    padding-top: 0.5em;
    display: block;
    font-weight: normal;
    font-size: 1em;
    line-height: 1.5em;
    color: #212B32;
    color: #005EB8;
    font-size: 1em;
    line-height: 1em;
    font-weight: bold;
    text-decoration: none;
    // vertical-align: middle;
  }
</style>
