<template>
  <div v-if="showTemplate" class="nhsuk-grid-row">
    <div class="nhsuk-grid-column-full">
      <sjr-if journey="onlineConsultations">
        <appointment-guidance-menu id="appointment-guidance-menu" />
      </sjr-if>

      <sjr-if journey="onlineConsultations" :disabled="true">
        <div id="info" data-purpose="info">
          <strong>1. {{ $t('appointments.guidance.selfCare.selfCare') }}</strong>
          <p class="nhsuk-u-padding-bottom-4">
            {{ $t('appointments.guidance.selfCare.minorProblems') }}</p>
          <strong>2. {{ $t('appointments.guidance.checkYourSymptoms.checkYourSymptoms') }}</strong>
          <p class="nhsuk-u-padding-bottom-4">
            {{ $t('appointments.guidance.checkYourSymptoms.usingNhsOnline') }}</p>
          <strong>3. {{ $t('appointments.guidance.pharmacist.getAdviceFrom') }}</strong>
          <p class="nhsuk-u-padding-bottom-4">
            {{ $t('appointments.guidance.pharmacist.theyAre') }}</p>
        </div>
        <analytics-tracked-tag :text="$t('appointments.guidance.checkYourSymptoms.checkSymptoms')"
                               :destination="symptomsPath"
                               data-purpose="generic-button">
          <generic-button id="btn_check_symptoms"
                          :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                          tabindex="0"
                          @click="onCheckSymptomClicked">
            {{ $t('appointments.guidance.checkYourSymptoms.checkSymptoms') }}
          </generic-button>
        </analytics-tracked-tag>
      </sjr-if>

      <generic-button
        id="btn_appointment"
        :button-classes="['nhsuk-button']"
        tabindex="0"
        @click="onBookButtonClicked">
        {{ $t('appointments.bookAnAppointment') }}
      </generic-button>

      <sjr-if v-if="!$store.state.device.isNativeApp" journey="onlineConsultations">
        <desktopGenericBackLink :path="indexPath"
                                button-text="generic.back"
                                @clickAndPrevent="onBackButtonClicked"/>
      </sjr-if>
    </div>
  </div>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import SjrIf from '@/components/SjrIf';
import {
  INDEX_PATH,
  SYMPTOMS_PATH,
  APPOINTMENT_BOOKING_PATH,
  GP_APPOINTMENTS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'GpAppointmentsBookingGuidancePage',
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    AppointmentGuidanceMenu,
    DesktopGenericBackLink,
    SjrIf,
  },
  data() {
    return {
      indexPath: INDEX_PATH,
      symptomsPath: SYMPTOMS_PATH,
    };
  },
  methods: {
    onBookButtonClicked() {
      redirectTo(this, APPOINTMENT_BOOKING_PATH);
    },
    onCheckSymptomClicked() {
      redirectTo(this, this.symptomsPath);
    },
    onBackButtonClicked() {
      redirectTo(this, GP_APPOINTMENTS_PATH);
    },
  },
};
</script>
