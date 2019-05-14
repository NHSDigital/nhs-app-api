<template>

  <div v-if="showTemplate" :class="[$style['pull-content'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">
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
                           data-purpose="generic-button">
      <generic-button id="btn_check_symptoms"
                      :class="$style.button"
                      tabindex="0"
                      @click="onCheckSymptomClicked">
        {{ $t('appointments.guidance.symptomButtonText') }}
      </generic-button>
    </analytics-tracked-tag>
    <no-js-form :action="appointmentBookingPath" :value="formData">
      <generic-button
        id="btn_appointment"
        :class="[$style.button, $style.green]"
        tabindex="0"
        @click.stop.prevent="onBookButtonClicked">
        {{ $t('appointments.guidance.bookButtonText') }}
      </generic-button>
    </no-js-form>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { APPOINTMENT_BOOKING, SYMPTOMS } from '@/lib/routes';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import GenericButton from '@/components/widgets/GenericButton';
import NoJsForm from '@/components/no-js/NoJsForm';
import { redirectTo } from '@/lib/utils';

export default {
  components: {
    AnalyticsTrackedTag,
    GenericButton,
    NoJsForm,
  },
  data() {
    return {
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
    onCheckSymptomClicked() {
      redirectTo(this, SYMPTOMS.path, null);
    },
    onBookButtonClicked() {
      redirectTo(this, APPOINTMENT_BOOKING.path, null);
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
   min-width: 16.875em;
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
