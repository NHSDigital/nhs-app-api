<template>
  <main v-if="showTemplate" :class="$style.main">

    <div :class="$style['above-float-button']">

      <success-dialog v-if="justOrderedARepeatPrescription">
        <p>
          {{ $t('rp05.confirmationMessage') }}
        </p>
      </success-dialog>

      <div v-if="showNoPrescriptions" class="info" data-purpose="no-prescriptions-error">
        <p>
          <b>{{ $t('rp06.empty.subHeader') }}</b>
        </p>
        <p>
          {{ $t('rp06.empty.contactGp') }}
        </p>
        <p>
          {{ $t('rp06.empty.body') }}
        </p>
      </div>
      <ul v-if="showPrescriptions" data-purpose="prescriptions">
        <li
          v-for="prescriptionCourse in prescriptionCoursesToDisplay"
          :key="prescriptionCourse.id"
          :class="$style['prescription-course']">
          <div :class="$style.container">
            <div>
              <b>
                {{ $t('rp02.orderDate') }}
              </b>
              : <span aria-label="order-date">{{ prescriptionCourse.orderDate | shortDate }}</span>
            </div>
            <hr>
            <b aria-label="course-name">{{ prescriptionCourse.name }}</b>
            <div
              aria-label="dosage">
              {{ prescriptionCourse.dosage }} - {{ prescriptionCourse.quantity }}
            </div>
            <hr>
            <div>
              <b>
                {{ $t('rp02.status') }}
              </b>
              : <span aria-label="status">{{ prescriptionCourse.status }}</span>
            </div>
          </div>
        </li>
      </ul>
    </div>

    <floating-button-bottom v-if="hasLoaded" @on-click="onRepeatPrescriptionButtonClicked">
      {{ $t('rp01.orderPrescriptionButton') }}
    </floating-button-bottom>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import FloatingButtonBottom from '@/components/FloatingButtonBottom';
import SuccessDialog from '@/components/SuccessDialog';

export default {
  middleware: ['auth', 'meta'],
  components: {
    FloatingButtonBottom,
    SuccessDialog,
  },
  data() {
    return {
      justOrderedARepeatPrescription: false,
    };
  },
  computed: {
    showNoPrescriptions() {
      return (
        this.$store.state.prescriptions.hasLoaded &&
        this.$store.state.prescriptions.prescriptionCourses.length === 0
      );
    },
    showPrescriptions() {
      return (
        this.$store.state.prescriptions.hasLoaded &&
        this.$store.state.prescriptions.prescriptionCourses.length > 0
      );
    },
    prescriptionCoursesToDisplay() {
      return this.$store.state.prescriptions.prescriptionCourses;
    },
    hasLoaded() {
      return this.$store.state.prescriptions.hasLoaded;
    },
  },
  mounted() {
    this.$store.dispatch('prescriptions/clear');
    this.justOrderedARepeatPrescription =
    this.$store.state.repeatPrescriptionCourses.justOrderedARepeatPrescription;
    this.$store.dispatch('prescriptions/load', this.$config);

    this.$store.dispatch('errors/setApiErrorButtonPath', '');
  },
  methods: {
    onRepeatPrescriptionButtonClicked() {
      this.$router.push('/prescriptions/repeat-courses');
    },
  },
};
</script>

<style module lang="scss">
@import "../../style/html";
@import "../../style/elements";
@import "../../style/buttons";
@import "../../style/fonts";
@import "../../style/spacings";

.main {
  @include space(padding, all, $three);
}

.prescription-course {
  list-style: none;
  @include space(margin, bottom, $three);
}

.container {
  border: solid 1px $mid_grey;
  border-radius: 5px;
  background: $white;
  @include space(padding, all, $three);
  transition: all ease 0.5s;
  hr {
    height: 1px;
    border: none;
    background-color: $dark_grey;
    opacity: 0.2;
    @include space(margin, top, $two);
    @include space(margin, bottom, $two);
  }
}

.above-float-button {
  margin-bottom: $marginBottomFullScreen;
}
</style>
