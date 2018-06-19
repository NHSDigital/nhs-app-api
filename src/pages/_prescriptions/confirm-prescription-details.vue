<template>
  <div id="mainDiv">
    <spinner />
    <main class="content">
      <div :class="$style.panel">
        <div :class="$style.panelContent">
          <b><h5 :class="$style.panelHeader">
            {{ $t('prescriptions.confirmPrescriptionOrder.header') }}
          </h5></b>
          <hr>
          <div
            v-for="selectedPrescription in selectedPrescriptions"
            :key="selectedPrescription.courseId"
            data-purpose="selected-prescription"
          >
            <label
              :class="$style.formLabel"
              data-purpose="prescription-name">
              {{ selectedPrescription.name }}
            </label>
            <p
              :class="$style.prescriptionDescription"
              data-purpose="prescription-description">
              {{ selectedPrescription.dosage }} - {{ selectedPrescription.quantity }}
            </p>
          </div>
        </div>
      </div>
      <button
        id="btn_confirm_and_order_prescription"
        class="button green"
        @click="onConfirmButtonClicked">
        {{ $t('prescriptions.confirmPrescriptionOrder.confirmButtonText') }}
      </button>
      <nuxt-link to="/prescriptions/repeat-courses" tag="button" type="submit" class="button grey">
        {{ $t('prescriptions.confirmPrescriptionOrder.changeButton') }}
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
      return this.$store.state.repeatPrescriptionCourses.selectedPrescriptions;
    },
    ...mapGetters({
      selectedPrescriptions: 'repeatPrescriptionCourses/selectedPrescriptions',
    }),
  },
  created() {
    if (this.selectedPrescriptions.length === 0) {
      this.$router.push('/prescriptions');
    }
  },
  mounted() {
    this.$store.dispatch('errors/setApiErrorButtonPath', '/prescriptions');
  },
  methods: {
    onConfirmButtonClicked() {
      const repeatPrescriptionOrder = {
        CourseIds: this.selectedPrescriptions.map(x => x.id),
        SpecialRequest: null,
      };
      this.$store.dispatch('repeatPrescriptionCourses/orderRepeatPrescription', repeatPrescriptionOrder);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/html";
  @import "../../style/fonts";
  @import "../../style/buttons";
  @import "../../style/elements";

  .panel {
   @include panel;
  }
  .panelHeader {
    @include panelHeader;
    font-family: $frutiger-bold!important;
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
    font-family: $frutiger-bold!important;
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
