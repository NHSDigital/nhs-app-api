<template>
  <ul :class="$style.listMenu" data-sid="navigation-list-menu">
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.symptoms')"
                             href="/symptoms"
                             data-sid="symptoms-list-item"
                             tag="a">
        {{ $t('navigationMenuList.symptoms') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.appointments')"
                             href="/appointments"
                             data-sid="appointments-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.appointments') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.prescriptions')"
                             href="/prescriptions"
                             data-sid="prescriptions-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.prescriptions') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :text="$t('navigationMenuList.myRecord')"
                             href="/my-record-warning"
                             data-sid="myrecord-menu-item"
                             tag="a">
        {{ $t('navigationMenuList.myRecord') }}
      </analytics-tracked-tag>
    </li>
    <li :class="$style.listMenuItem">
      <analytics-tracked-tag :class="$style.listMenuAnchor"
                             :href="organDonationUrl"
                             :text="$t('navigationMenuList.organDonation')"
                             data-sid="organ-donation-menu-item"
                             tag="a"
                             target="_blank">
        {{ $t('navigationMenuList.organDonation') }}
      </analytics-tracked-tag>
    </li>
  </ul>
</template>

<script>
/* eslint-disable import/extensions */
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'NavigationListMenu',
  components: {
    AnalyticsTrackedTag,
  },
  data() {
    return {
      organDonationUrl: process.env.ORGAN_DONATION_URL,
      dataSharingUrl: process.env.DATA_SHARING_URL,
    };
  },
};
</script>

<style module lang="scss">
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
