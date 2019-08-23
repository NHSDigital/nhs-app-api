<template>
  <div v-if="showTemplate"
       :class="[$style['pull-content'], !$store.state.device.isNativeApp && $style.desktopWeb]">
    <sjr-if journey="onlineConsultations">
      <appointment-guidance-menu/>
    </sjr-if>
    <sjr-if journey="onlineConsultations" :disabled="true">
      <div :class="$style.info" data-purpose="info">
        <strong>1. {{ $t('appointments.guidance.li1.header') }}</strong>
        <p>{{ $t('appointments.guidance.li1.text') }}</p>
        <strong>2. {{ $t('appointments.guidance.li2.header') }}</strong>
        <p>{{ $t('appointments.guidance.li2.text') }}</p>
        <strong>3. {{ $t('appointments.guidance.li3.header') }}</strong>
        <p>{{ $t('appointments.guidance.li3.text') }}</p>
      </div>
      <analytics-tracked-tag :text="$t('appointments.guidance.symptomButtonText')"
                             :destination="symptomsPath"
                             :tabindex="-1"
                             data-purpose="generic-button">
        <no-js-form :action="symptomsPath" :value="formData">
          <generic-button id="btn_check_symptoms"
                          :class="$style.button"
                          :button-classes="['button']"
                          tabindex="0"
                          @click="onCheckSymptomClicked">
            {{ $t('appointments.guidance.symptomButtonText') }}
          </generic-button>
        </no-js-form>
      </analytics-tracked-tag>
    </sjr-if>

    <no-js-form :action="appointmentBookingPath" :value="formData">
      <generic-button id="btn_appointment"
                      :class="$style.button"
                      :button-classes="['button', 'green']"
                      tabindex="0"
                      @click.stop.prevent="onBookButtonClicked">
        {{ $t('appointments.guidance.bookButtonText') }}
      </generic-button>
    </no-js-form>

    <sjr-if journey="onlineConsultations">
      <generic-button v-if="$store.state.device.isNativeApp"
                      id="back_btn"
                      :class="$style.button"
                      :button-classes="['button', 'grey']"
                      @click="onBackButtonClicked">
        {{ $t('appointments.guidance.backButtonText') }}
      </generic-button>
      <desktopGenericBackLink v-else
                              :path="indexPath"
                              button-text="appointments.guidance.backDesktopLinkText"
                              @clickAndPrevent="onBackButtonClicked"/>
    </sjr-if>
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
  components: {
    AnalyticsTrackedTag,
    AppointmentGuidanceMenu,
    DesktopGenericBackLink,
    GenericButton,
    NoJsForm,
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

<style module lang="scss" scoped>
@import "../../style/info";

div {
  &.desktopWeb {
    .button {
      width: auto;
      min-width: 16em;
    }

    .info {
      font-size: 1em;
      margin-bottom: 1em;
      padding-top: 1em;
      max-width: 540px;

      p {
        font-family: $default-web;
        font-weight: lighter;
        max-width: 540px;
      }

      strong {
        font-family: $default-web;
        font-weight: normal;
      }
    }
  }
}
</style>
