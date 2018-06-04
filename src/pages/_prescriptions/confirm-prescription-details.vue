<template>
  <div id="mainDiv">
    <spinner />
    <main class="content">
      <div :class="$style.panel">
        <div :class="$style.panelContent">
          <h5 :class="$style.panelHeader">
            {{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.header') }}
          </h5>
          <hr>
          <div
            v-for="selectedPrescription in selectedPrescriptions"
            :key="selectedPrescription.courseId"
          >
            <label :class="$style.formLabel">{{ selectedPrescription.name }}</label>
            <p
              :class="$style.prescriptionDescription">
              {{ selectedPrescription.dosage }} - {{ selectedPrescription.quantity }}
            </p>
          </div>
          <hr>
          <h5 :class="$style.panelHeader">
            {{ $t('prescriptions.noRepeatPrescriptionsYouCanOrder.header') }}
          </h5>
        </div>
      </div>
      <nuxt-link
        id="btn_order_prescription"
        to="prescriptions/repeat-courses"
        tag="button"
        class="button green">
        Confirm and order repeat prescription
      </nuxt-link>
      <nuxt-link to="/prescriptions/repeat-courses" tag="button" type="submit" class="button grey">
        Change this repeat prescription
      </nuxt-link>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import Spinner from '@/components/Spinner';
import { mapGetters } from 'vuex';

export default {
  components: {
    Spinner,
  },
  computed: {
    selectedPrescriptions() {
      debugger;
      return this.$store.state.repeatPrescriptionCourses.selectedPrescriptions;
    },
    ...mapGetters({
      selectedPrescriptions: 'repeatPrescriptionCourses/selectedPrescriptions',
    }),
  },
};
</script>

<style module lang="scss">
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/buttons";
  @import "../../style/elements";

  .panel {
   @include panel;
  }
  .panelHeader {
    @include panelHeader;
  }

  .panelContent {
    display: table-cell;
    vertical-align: top;
  }

  .formLabel {
    display: block;
    font-weight: 700;
    padding-top: 16px;
    font-size: 16px;
    line-height: 22px;
    color: #4A4A4A;
    font-family: $frutiger-bold;
  }
  .prescriptionDescription {
    display: block;
    font-weight: normal;
    font-size: 16px;
    line-height: 22px;
    color: #4A4A4A;
    margin-bottom: 16px;
  }
</style>
