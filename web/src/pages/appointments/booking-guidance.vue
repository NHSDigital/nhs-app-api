<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <sjr-if journey="onlineConsultations">
          <appointment-guidance-menu/>
        </sjr-if>
        <sjr-if journey="onlineConsultations" :disabled="true">
          <div id="info" data-purpose="info">
            <strong>1. {{ $t('appointments.guidance.li1.header') }}</strong>
            <p class="nhsuk-u-padding-bottom-4">{{ $t('appointments.guidance.li1.text') }}</p>
            <strong>2. {{ $t('appointments.guidance.li2.header') }}</strong>
            <p class="nhsuk-u-padding-bottom-4">{{ $t('appointments.guidance.li2.text') }}</p>
            <strong>3. {{ $t('appointments.guidance.li3.header') }}</strong>
            <p class="nhsuk-u-padding-bottom-4">{{ $t('appointments.guidance.li3.text') }}</p>
          </div>
          <analytics-tracked-tag :text="$t('appointments.guidance.symptomButtonText')"
                                 :destination="symptomsPath"
                                 :tabindex="-1"
                                 data-purpose="generic-button">
            <no-js-form :action="symptomsPath" :value="formData">
              <generic-button id="btn_check_symptoms"
                              :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                              tabindex="0"
                              @click="onCheckSymptomClicked">
                {{ $t('appointments.guidance.symptomButtonText') }}
              </generic-button>
            </no-js-form>
          </analytics-tracked-tag>
        </sjr-if>
      </div>
    </div>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <no-js-form :action="appointmentBookingPath" :value="formData">
          <generic-button
            id="btn_appointment"
            :button-classes="['nhsuk-button']"
            tabindex="0"
            @click.stop.prevent="onBookButtonClicked">
            {{ $t('appointments.guidance.bookButtonText') }}
          </generic-button>
        </no-js-form>
      </div>
    </div>

    <div v-if="!$store.state.device.isNativeApp"
         class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <sjr-if journey="onlineConsultations">
          <desktopGenericBackLink :path="indexPath"
                                  button-text="appointments.guidance.backDesktopLinkText"
                                  @clickAndPrevent="onBackButtonClicked"/>
        </sjr-if>
      </div>
    </div>
  </div>
</template>

<script>
import { APPOINTMENT_BOOKING, APPOINTMENTS, INDEX, SYMPTOMS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import GenericButton from '@/components/widgets/GenericButton';
import NoJsForm from '@/components/no-js/NoJsForm';
import SjrIf from '@/components/SjrIf';

export default {
  layout: 'nhsuk-layout',
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    NoJsForm,
    AppointmentGuidanceMenu,
    DesktopGenericBackLink,
    SjrIf,
  },
  data() {
    return {
      indexPath: INDEX.path,
      symptomsPath: SYMPTOMS.path,
    };
  },
  computed: {
    appointmentBookingPath() {
      return APPOINTMENT_BOOKING.path;
    },
    formData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
    },
  },
  methods: {
    onBookButtonClicked() {
      redirectTo(this, APPOINTMENT_BOOKING.path, null);
    },
    onCheckSymptomClicked() {
      redirectTo(this, SYMPTOMS.path, null);
    },
    onBackButtonClicked() {
      redirectTo(this, APPOINTMENTS.path, null);
    },
  },
};

</script>
