<template>
  <nav :class="$style.menu">
    <ul role="tablist">
      <li :class="[isMenuItemSelected(0) ? $style.active : undefined]"
          :data-selected="isMenuItemSelected(0) ? true : false"
          role="presentation">
        <a :aria-selected="isMenuItemSelected(0) ? 'true' : 'false'"
           :href="symptomsPath"
           data-sid="symptoms-menu-item"
           role="tab"
           @click.prevent="setMenuitemState($event)">
          <symptoms-icon :selected="isMenuItemSelected(0)" aria-hidden="true"/>
          <span>{{ $t('navigationMenu.symptomsLabel') }}</span>
        </a>
      </li>
      <li :class="[isMenuItemSelected(1) ? $style.active : undefined]"
          :data-selected="isMenuItemSelected(1) ? true : false"
          role="presentation">
        <a :aria-selected="isMenuItemSelected(1) ? 'true' : 'false'"
           :href="appointmentsPath"
           data-sid="appointments-menu-item"
           role="tab"
           @click.prevent="setMenuitemState($event)">
          <appointments-icon :selected="isMenuItemSelected(1)" aria-hidden="true"/>
          <span>{{ $t('navigationMenu.appointmentsLabel') }}</span>
        </a>
      </li>
      <li :class="[isMenuItemSelected(2) ? $style.active : undefined]"
          :data-selected="isMenuItemSelected(2) ? true : false"
          role="presentation">
        <a :aria-selected="isMenuItemSelected(2) ? 'true' : 'false'"
           :href="prescriptionsPath"
           data-sid="prescriptions-menu-item"
           role="tab"
           @click.prevent="setMenuitemState($event)">
          <prescriptions-icon :selected="isMenuItemSelected(2)" aria-hidden="true"/>
          <span>{{ $t('navigationMenu.prescriptionsLabel') }}</span>
        </a>
      </li>
      <li :class="[isMenuItemSelected(3) ? $style.active : undefined]"
          :data-selected="isMenuItemSelected(3) ? true : false"
          role="presentation">
        <a :aria-selected="isMenuItemSelected(3) ? 'true' : 'false'"
           :href="recordWarningPath"
           data-sid="myrecord-menu-item"
           role="tab"
           @click.prevent="setMenuitemState($event)">
          <record-icon :selected="isMenuItemSelected(3)" aria-hidden="true"/>
          <span>{{ $t('navigationMenu.myRecordLabel') }}</span>
        </a>
      </li>
      <li :class="[isMenuItemSelected(4) ? $style.active : undefined]"
          :data-selected="isMenuItemSelected(4) ? true : false"
          role="presentation">
        <a :aria-selected="isMenuItemSelected(4) ? 'true' : 'false'"
           :href="morePath"
           data-sid="more-menu-item"
           role="tab"
           @click.prevent="setMenuitemState($event)">
          <more-icon :selected="isMenuItemSelected(4)" aria-hidden="true"/>
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
import { SYMPTOMS, APPOINTMENTS, PRESCRIPTIONS, MYRECORD, MORE } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    SymptomsIcon,
    AppointmentsIcon,
    PrescriptionsIcon,
    RecordIcon,
    MoreIcon,
  },
  data() {
    return {
      symptomsPath: SYMPTOMS.path,
      appointmentsPath: APPOINTMENTS.path,
      prescriptionsPath: PRESCRIPTIONS.path,
      recordWarningPath: MYRECORD.path,
      morePath: MORE.path,
    };
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
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../style/navmenu';

</style>
