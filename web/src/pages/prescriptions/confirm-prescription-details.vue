<template>

  <div v-if="showTemplate" :class="[$style.content, 'pull-content']">

    <div :class="$style.info" data-purpose="info">
      <p>
        {{ $t('rp04.subHeader') }}
      </p>
    </div>
    <div :class="$style.panel">
      <div v-for="selectedPrescription in selectedPrescriptions"
           :key="selectedPrescription.courseId"
           data-purpose="selected-prescription">
        <b data-purpose="prescription-name">{{ selectedPrescription.name }}</b>
        <p data-purpose="prescription-description">{{ selectedPrescription.details }}</p>
      </div>
      <hr>
      <div v-if="specialRequestNecessity !== 'NotAllowed'">
        <b>{{ $t('rp04.specialRequestsLabel') }}</b>
        <p v-if="specialRequest"
           id="specialRequestText"
           :class="$style['keep-line-breaks']">{{ specialRequest }}
        </p>
        <p v-else id="specialRequestText">
          {{ $t('rp03.noSpecialRequestDefaultText') }}
        </p>
      </div>
    </div>
    <generic-button id="btn_confirm_and_order_prescription" :class="[$style.button, $style.green]"
                    @click="onConfirmButtonClicked">
      {{ $t('rp04.confirmButton') }}
    </generic-button>

    <generic-button id="back-to-prescriptions"
                    :class="[$style.button, $style.grey]"
                    @click="backToPrescriptionsClicked">
      {{ $t('rp04.backButton') }}
    </generic-button>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';
import GenericButton from '@/components/widgets/GenericButton';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
  },
  data() {
    return {
      selectedPrescriptions: this.$store.getters['repeatPrescriptionCourses/selectedPrescriptions'],
      specialRequest: this.$store.state.repeatPrescriptionCourses.specialRequest,
    };
  },
  computed: {
    specialRequestNecessity() {
      return this.$store.state.repeatPrescriptionCourses
        .specialRequestNecessity;
    },
    prescriptionRepeatCoursesPath() {
      return PRESCRIPTION_REPEAT_COURSES.path;
    },
  },
  created() {
    if (this.selectedPrescriptions === null || this.selectedPrescriptions.length === 0) {
      redirectTo(this, PRESCRIPTIONS.path, null);
    }
  },
  methods: {
    onConfirmButtonClicked() {
      const repeatPrescriptionOrder = {
        CourseIds: this.selectedPrescriptions.map(x => x.id),
        SpecialRequest: this.specialRequest,
      };
      this.$store.dispatch('repeatPrescriptionCourses/orderRepeatPrescription', repeatPrescriptionOrder)
        .then(() => {
          this.$store.dispatch('flashMessage/addSuccess', this.$t('rp05.confirmationMessage'));
          redirectTo(this, PRESCRIPTIONS.path, null);
        });
    },
    backToPrescriptionsClicked() {
      redirectTo(this, this.prescriptionRepeatCoursesPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/buttons";
  @import "../../style/forms";
  @import "../../style/info";
  @import "../../style/panels";

  .keep-line-breaks {
    white-space: pre-line;
  }
</style>
