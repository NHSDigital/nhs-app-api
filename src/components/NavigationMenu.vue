<template>
  <nav class="menu">
    <ul>
      <li @click="onSymptomsMenuClicked">
        <symptoms-icon/>
        <label>Symptoms</label>
      </li>
      <li @click="onAppointmentsMenuClicked">
        <appointments-icon/>
        <label>Appointments</label>
      </li>
      <li @click="onPrescriptionsMenuClicked">
        <prescriptions-icon/>
        <label>Prescriptions</label>
      </li>
      <li @click="onMyRecordMenuClicked">
        <record-icon/>
        <label>My record</label>
      </li>
      <li @click="onMoreMenuClicked">
        <more-icon/>
        <label>More</label>
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
  data() {
    return {
      symptomsCheckerUrl: this.$config.SYMPTOM_CHECKER_URL,
      previousSelectedMenuItem: null,
    };
  },
  methods: {
    onSymptomsMenuClicked() {
      window.open(this.symptomsCheckerUrl, '_blank');
    },

    onAppointmentsMenuClicked(event) {
      const appointmentsMenu = event.currentTarget;
      this.handleMenuSelection(appointmentsMenu);
    },

    onPrescriptionsMenuClicked(event) {
      const prescriptionsMenu = event.currentTarget;
      this.handleMenuSelection(prescriptionsMenu);
    },

    onMyRecordMenuClicked(event) {
      const myRecordMenuItem = event.currentTarget;
      this.handleMenuSelection(myRecordMenuItem);
    },

    onMoreMenuClicked(event) {
      const moreMenuItem = event.currentTarget;
      this.handleMenuSelection(moreMenuItem);
      this.$router.push('more');
    },

    handleMenuSelection(menuItem) {
      this.clearSelectedMenuItem();
      this.selectMenuItem(menuItem);
    },

    clearSelectedMenuItem() {
      if (this.previousSelectedMenuItem) {
        this.previousSelectedMenuItem.classList.remove('active');
      }
    },

    selectMenuItem(menuItem) {
      if (menuItem) {
        menuItem.classList.add('active');
        this.previousSelectedMenuItem = menuItem;
      }
    },
  },
};
</script>

<style lang="scss" scoped>
  @import "../style/menu";

</style>
