<template>
  <div v-if="showTemplate">
    <spinner />
    <main :class="$style.content">
      <div :class="$style.panel">
        <div :class="$style.panelContent">
          <b><h5 :class="$style.panelHeader">
            {{ $t('rp04.subHeader') }}
          </h5></b>
          <hr>
          <div
            v-for="selectedPrescription in selectedPrescriptions"
            :key="selectedPrescription.courseId"
            data-purpose="selected-prescription">
            <label
              :class="$style.formLabel"
              data-purpose="prescription-name">
              {{ selectedPrescription.name }}
            </label>
            <p
              :class="$style.prescriptionDescription"
              data-purpose="prescription-description">
              {{ selectedPrescription.details }}
            </p>
          </div>
          <hr>
          <div>
            <span :class="$style.formLabel">
              {{ $t('rp04.specialRequestsLabel') }}
            </span>
            <p v-if="specialRequest"
               id="specialRequestText"
               :class="$style.specialRequestText">{{ specialRequest }}</p>
            <p v-else id="specialRequestText">
              {{ $t('rp03.noSpecialRequestDefaultText') }}
            </p>
          </div>
        </div>
      </div>
      <button
        id="btn_confirm_and_order_prescription"
        :class="[$style.button, $style.green]"
        @click="onConfirmButtonClicked">
        {{ $t('rp04.confirmButton') }}
      </button>
      <nuxt-link :class="[$style.button, $style.grey]"
                 to="/prescriptions/repeat-courses" tag="button" type="submit" >
        {{ $t('rp04.backButton') }}
      </nuxt-link>
    </main>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import Spinner from '@/components/widgets/Spinner';

export default {
  components: {
    Spinner,
  },
  middleware: ['meta', 'auth'],
  data() {
    return {
      selectedPrescriptions: this.$store.getters['repeatPrescriptionCourses/selectedPrescriptions'],
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest,
    };
  },
  created() {
    if (this.selectedPrescriptions === null || this.selectedPrescriptions.length === 0) {
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
        SpecialRequest: this.specialRequest,
      };
      this.$store.dispatch('repeatPrescriptionCourses/orderRepeatPrescription', repeatPrescriptionOrder);
    },
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

  .specialRequestText {
    white-space: pre-wrap;
    word-break: break-all;
  }
</style>
