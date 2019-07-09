<template>

  <div v-if="showTemplate" :class="[$style['pull-content'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">
    <appointment-guidance-menu/>

    <no-js-form :action="appointmentBookingPath" :value="formData">
      <generic-button
        id="btn_appointment"
        :class="[$style.button, $style.green]"
        tabindex="0"
        @click.stop.prevent="onBookButtonClicked">
        {{ $t('appointments.guidance.bookButtonText') }}
      </generic-button>
    </no-js-form>

    <generic-button v-if="$store.state.device.isNativeApp"
                    id="back_btn"
                    :class="[$style.button, $style.grey]"
                    @click="onBackButtonClicked">
      {{ $t('appointments.guidance.backButtonText') }}
    </generic-button>

    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="indexPath"
      :button-text="'appointments.guidance.backDesktopLinkText'"
      @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import { APPOINTMENT_BOOKING, APPOINTMENTS, INDEX, SYMPTOMS } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import AppointmentGuidanceMenu from '@/components/appointments/AppointmentGuidanceMenu';
import GenericButton from '@/components/widgets/GenericButton';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import NoJsForm from '@/components/no-js/NoJsForm';

export default {
  components: {
    GenericButton,
    NoJsForm,
    AppointmentGuidanceMenu,
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
