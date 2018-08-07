<template>
  <nav :class="$style.menu">
    <ul>
      <li :class="[isMenuItemSelected(0) ? $style.active : undefined]"
          :data-selected="[isMenuItemSelected(0) ? true : false]">
        <a href="/symptoms"
           data-sid="symptoms-menu-item"
           @click="setMenuitemState($event)">
          <symptoms-icon :selected="isMenuItemSelected(0)"/>
          <span>{{ $t('navigationMenu.symptomsLabel') }}</span>
        </a>
      </li>
      <li :class="[isMenuItemSelected(1) ? $style.active : undefined]"
          :data-selected="[isMenuItemSelected(1) ? true : false]">
        <a href="/appointments"
           data-sid="appointments-menu-item"
           @click="setMenuitemState($event)">
          <appointments-icon :selected="isMenuItemSelected(1)"/>
          <span>{{ $t('navigationMenu.appointmentsLabel') }}</span>
        </a>
      </li>
      <li :class="[isMenuItemSelected(2) ? $style.active : undefined]"
          :data-selected="[isMenuItemSelected(2) ? true : false]">
        <a href="/prescriptions"
           data-sid="prescriptions-menu-item"
           @click="setMenuitemState($event)">
          <prescriptions-icon :selected="isMenuItemSelected(2)"/>
          <span>{{ $t('navigationMenu.prescriptionsLabel') }}</span>
        </a>
      </li>
      <li :class="[isMenuItemSelected(3) ? $style.active : undefined]"
          :data-selected="[isMenuItemSelected(3) ? true : false]">
        <a href="/my-record-warning"
           data-sid="myrecord-menu-item"
           @click="setMenuitemState($event)">
          <record-icon :selected="isMenuItemSelected(3)"/>
          <span>{{ $t('navigationMenu.myRecordLabel') }}</span>
        </a>
      </li>
      <li :class="[isMenuItemSelected(4) ? $style.active : undefined]"
          :data-selected="[isMenuItemSelected(4) ? true : false]">
        <a href="/more"
           data-sid="more-menu-item"
           @click="setMenuitemState($event)">
          <more-icon :selected="isMenuItemSelected(4)"/>
          <span>{{ $t('navigationMenu.moreLabel') }}</span>
        </a>
      </li>
    </ul>
  </nav>
</template>

<script>
import SymptomsIcon from '@/components/icons/SymptomsIcon';
import AppointmentsIcon from '@/components/icons/AppointmentsIcon';
import PrescriptionsIcon from '@/components/icons/PrescriptionsIcon';
import RecordIcon from '@/components/icons/MyRecordIcon';
import MoreIcon from '@/components/icons/MoreIcon';

export default {
  components: {
    SymptomsIcon,
    AppointmentsIcon,
    PrescriptionsIcon,
    RecordIcon,
    MoreIcon,
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

      event.preventDefault();
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../style/navmenu';

</style>
