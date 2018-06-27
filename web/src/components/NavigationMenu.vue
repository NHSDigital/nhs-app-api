<template>
  <nav class="menu">
    <ul>
      <li :class="getMenuitemState(0)" @click="setMenuitemState(0)">
        <symptoms-icon/>
        <span>{{ $t('navigationMenu.symptomsLabel') }}</span>
      </li>
      <li :class="getMenuitemState(1)" @click="setMenuitemState(1)">
        <appointments-icon/>
        <span>{{ $t('navigationMenu.appointmentsLabel') }}</span>
      </li>
      <li :class="getMenuitemState(2)" @click="setMenuitemState(2)">
        <prescriptions-icon/>
        <span>{{ $t('navigationMenu.prescriptionsLabel') }}</span>
      </li>
      <li :class="getMenuitemState(3)" @click="setMenuitemState(3)">
        <record-icon/>
        <span>{{ $t('navigationMenu.myRecordLabel') }}</span>
      </li>
      <li :class="getMenuitemState(4)" @click="setMenuitemState(4)">
        <more-icon/>
        <span>{{ $t('navigationMenu.moreLabel') }}</span>
      </li>
    </ul>
  </nav>
</template>

<script>
/* eslint-disable import/extensions */
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
  data() {
    return {
      symptomsCheckerUrl: process.env.SYMPTOM_CHECKER_URL,
    };
  },
  methods: {
    getMenuitemState(menuItemIndex) {
      // eslint-disable-next-line
      return {
        active: this.$store.state.navigation.menuItemStatusAt[menuItemIndex],
      };
    },
    setMenuitemState(menuItemIndex) {
      switch (menuItemIndex) {
        case 0:
          window.open(this.symptomsCheckerUrl, '_blank');
          break;
        case 1:
          this.$router.push('/appointments');
          break;
        case 2:
          this.$router.push('/prescriptions');
          break;
        case 3:
          this.$router.push('/my-record/myrecordwarning');
          break;
        case 4:
          this.$router.push('/more');
          break;
        default:
          this.$router.push('/');
      }
    },
  },
};
</script>

<style lang="scss" scoped>
@import "../style/menu";
</style>
