<template>
  <ul :class="$style.listMenu" data-sid="navigation-list-menu">
    <li :class="$style.listMenuItem">
      <a :class="$style.listMenuAnchor"
         href="/symptoms"
         data-sid="symptoms-list-item"
         @click="setMenuitemState($event)">
        {{ $t('navigationMenuList.symptoms') }}
      </a>
    </li>
    <li :class="$style.listMenuItem">
      <a :class="$style.listMenuAnchor"
         href="/appointments"
         data-sid="appointments-menu-item"
         @click="setMenuitemState($event)">
        {{ $t('navigationMenuList.appointments') }}
      </a>
    </li>
    <li :class="$style.listMenuItem">
      <a :class="$style.listMenuAnchor"
         href="/prescriptions"
         data-sid="prescriptions-menu-item"
         @click="setMenuitemState($event)">
        {{ $t('navigationMenuList.prescriptions') }}
      </a>
    </li>
    <li :class="$style.listMenuItem">
      <a :class="$style.listMenuAnchor"
         href="/my-record/myrecordwarning"
         data-sid="myrecord-menu-item"
         @click="setMenuitemState($event)">
        {{ $t('navigationMenuList.myRecord') }}
      </a>
    </li>
    <li :class="$style.listMenuItem">
      <a :class="$style.listMenuAnchor"
         :href="organDonationUrl"
         data-sid="organ-donation-menu-item"
         target="_blank"
         @click="setMenuitemState($event)">
        {{ $t('navigationMenuList.organDonation') }}
      </a>
    </li>
  </ul>
</template>

<script>
export default {
  name: 'NavigationListMenu',
  data() {
    return {
      organDonationUrl: process.env.ORGAN_DONATION_URL,
      dataSharingUrl: process.env.DATA_SHARING_URL,
    };
  },
  methods: {
    setMenuitemState(event) {
      const a = event.currentTarget;
      this.$store.app.$analytics.trackButtonClick(a.pathname);
      if (a.target === '_blank') {
        window.open(a.href, '_blank');
      } else {
        this.$router.push(a.pathname);
      }

      event.preventDefault();
    },
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
    padding: 0.5em 3em 0.5em 1em;
    border-bottom: 1px #D8DDE0 solid;
    font-size: 1em;
    font-weight: normal;
    line-height: 1.5em;
    color: #212B32;
  }
  .listMenuAnchor {
    font-size: 1em;
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
