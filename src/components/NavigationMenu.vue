<template>
  <nav class="menu">
    <ul>
      <li @click="onSymptomsMenuClicked">
        <symptoms-icon/>
        <span>{{ $t('navigationMenu.symptomsLabel') }}</span>
      </li>
      <li @click="onAppointmentsMenuClicked">
        <appointments-icon/>
        <span>{{ $t('navigationMenu.appointmentsLabel') }}</span>
      </li>
      <li @click="onPrescriptionsMenuClicked">
        <prescriptions-icon/>
        <span>{{ $t('navigationMenu.prescriptionsLabel') }}</span>
      </li>
      <li @click="onMyRecordMenuClicked">
        <record-icon/>
        <span>{{ $t('navigationMenu.myRecordLabel') }}</span>
      </li>
      <li @click="onMoreMenuClicked">
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
    onSymptomsMenuClicked() {
      window.open(this.symptomsCheckerUrl, '_blank');
    },

    onAppointmentsMenuClicked(event) {
      const appointmentsMenu = event.currentTarget;
      this.handleMenuSelection(appointmentsMenu);
      this.$router.push('/appointments');
    },

    onPrescriptionsMenuClicked(event) {
      const prescriptionsMenu = event.currentTarget;
      this.handleMenuSelection(prescriptionsMenu);
      this.$router.push('/prescriptions');
    },

    onMyRecordMenuClicked(event) {
      const myRecordMenuItem = event.currentTarget;
      this.handleMenuSelection(myRecordMenuItem);
    },

    onMoreMenuClicked(event) {
      const moreMenuItem = event.currentTarget;
      this.handleMenuSelection(moreMenuItem);
      this.$router.push('/more');
    },

    handleMenuSelection(menuItem) {
      if (menuItem) {
        this.$store.dispatch('navigation/setNewMenuItem', menuItem);
      }
    },
  },
};
</script>

<style lang="scss" scoped>
@import "../style/menu";
</style>
