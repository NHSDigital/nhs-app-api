<template>

  <div v-if="showTemplate" :class="[$style['pull-content'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">

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
    <generic-button id="btn_confirm_and_order_prescription"
                    :button-classes="['button' , 'green',
                                      !$store.state.device.isNativeApp && 'medium']"
                    click-delay="medium"
                    @click="onConfirmButtonClicked">
      {{ $t('rp04.confirmButton') }}
    </generic-button>

    <generic-button v-if="$store.state.device.isNativeApp"
                    id="back-to-prescriptions"
                    :button-classes="['button' , 'grey']"
                    @click="backToPrescriptionsClicked">
      {{ $t('rp04.backButton') }}
    </generic-button>
    <desktopGenericBackLink v-else
                            :path="prescriptionRepeatCoursesPath"
                            :button-text="'rp04.backButton'"
                            @clickAndPrevent="backToPrescriptionsClicked"/>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { PRESCRIPTIONS, PRESCRIPTION_REPEAT_COURSES } from '@/lib/routes';
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    GenericButton,
    DesktopGenericBackLink,
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
  @import "../../style/forms";
  @import "../../style/info";
  @import "../../style/panels";

.pull-content {
    &.desktopWeb {
      font-family: $frutiger-light;
      &>* {
        max-width: 540px;
      }
    }
    .panel {
      margin-bottom: 1em;
    }
    hr {
      opacity: unset;
    }
    .keep-line-breaks {
    white-space: pre-line;
  }
  }
</style>
