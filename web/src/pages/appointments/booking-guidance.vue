<template>

  <div v-if="showTemplate" :class="[$style['pull-content'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">
    <appointment-guidance v-if="isOnlineConsultationsEnabled"/>
    <div v-else>
      <h2 id="guidance_sub_header">
        {{ $t('appointments.guidance.header') }}</h2>
      <div :class="$style.info"
           data-purpose="info">
        <p>
          {{ $t('appointments.guidance.text') }}</p>

        <strong>
          1. {{ $t('appointments.guidance.li1.header') }}</strong>
        <p>
          {{ $t('appointments.guidance.li1.text') }}</p>

        <strong>
          2. {{ $t('appointments.guidance.li2.header') }}</strong>
        <p>
          {{ $t('appointments.guidance.li2.text') }}</p>

        <strong>
          3. {{ $t('appointments.guidance.li3.header') }}</strong>
        <p>
          {{ $t('appointments.guidance.li3.text') }}</p>
      </div>
      <analytics-tracked-tag :text="$t('appointments.guidance.symptomButtonText')"
                             :destination="symptomsPath"
                             :tabindex="-1"
                             data-purpose="generic-button">
        <generic-button id="btn_check_symptoms"
                        :class="$style.button"
                        tabindex="0"
                        @click="onCheckSymptomClicked">
          {{ $t('appointments.guidance.symptomButtonText') }}
        </generic-button>
      </analytics-tracked-tag>
    </div>

    <no-js-form :action="appointmentBookingPath" :value="formData">
      <generic-button
        id="btn_appointment"
        :class="[$style.button, $style.green]"
        tabindex="0"
        @click.stop.prevent="onBookButtonClicked">
        {{ $t('appointments.guidance.bookButtonText') }}
      </generic-button>
    </no-js-form>

    <generic-button v-if="$store.state.device.isNativeApp && isOnlineConsultationsEnabled"
                    id="back_btn"
                    :class="[$style.button, $style.grey]"
                    @click="onBackButtonClicked">
      {{ $t('appointments.guidance.backButtonText') }}
    </generic-button>

    <desktopGenericBackLink
      v-if="isOnlineConsultationsEnabled && !$store.state.device.isNativeApp"
      :path="indexPath"
      :button-text="'appointments.guidance.backDesktopLinkText'"
      @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import flow from 'lodash/fp/flow';

/* eslint-disable import/extensions */
import { APPOINTMENT_BOOKING, APPOINTMENTS, INDEX, SYMPTOMS } from '@/lib/routes';
import { redirectTo, isTruthy } from '@/lib/utils';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import AppointmentGuidance from '@/components/appointments/AppointmentGuidanceMenu';
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import NoJsForm from '@/components/no-js/NoJsForm';

const isOnlineConsultationsEnabled = flow(
  get('$store.app.$env.ONLINE_CONSULTATIONS_ENABLED'),
  isTruthy,
);

export default {
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    NoJsForm,
    AppointmentGuidance,
    DesktopGenericBackLink,
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
    isOnlineConsultationsEnabled() {
      return isOnlineConsultationsEnabled(this);
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
@import "../../style/buttons";
@import "../../style/info";
@import "../../style/textstyles";
div {
 &.desktopWeb {
  h2 {
   font-family: $default-web;
   font-weight: bold;
  }

  .info {
   font-size: 1em;
   margin-bottom: 1em;

   p {
    font-family: $default-web;
    font-weight: lighter;
    max-width: 540px;
   }

   strong {
    font-family: $default-web;
    font-weight: normal;
    max-width: 540px;
   }
  }

  .button {
   @include button;
   box-sizing: border-box;
   padding: 0.625em;
   background-color: $nhs_blue;
   border: none;
   border-radius: 0.125em;
   outline: none;
   transition: all ease 0.5s;
   cursor: pointer;
   width: auto;
   min-width: 16em;
   padding-left: 2em;
   padding-right: 2em;
   max-width: 960px;
   display: block;
   width: auto;

   :focus {
    outline-color: $focus_highlight;
    box-shadow: inset 0 0 0 4px $focus_highlight;
    outline-offset: -5px;
   }
  }
  .green {
   background-color: $light_green;
   box-shadow: 0 0.125em 0 0 $dark_green;
   :focus {
    outline-color: $focus_highlight;
    box-shadow: inset 0 0 0 4px $focus_highlight;
    outline-offset: -5px;
   }
  }

 }
}

.button:focus{
 outline-color: $focus_highlight;
 box-shadow: inset 0 0 0 4px $focus_highlight;
}

.button.green:focus{
 outline-color: $focus_highlight;
 box-shadow: inset 0 0 0 4px $focus_highlight;
}

.button.green:hover{
 outline-color: $focus_highlight;
 box-shadow: inset 0 0 0 4px $focus_highlight;
}
</style>
