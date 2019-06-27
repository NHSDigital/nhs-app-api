<template>
  <div :class="[$style.panel, !$store.state.device.isNativeApp && $style.desktopWeb]">
    <div :class="$style.panelGroup">
      <component :is="dateTimeHeader" :class="$style['date-time-header']">
        <div :class="$style.date"
             data-label="date">{{ formatDate(appointment.startTime) }}</div>
        <div :class="$style.time"
             data-label="start time">{{ formatTime(appointment.startTime) }}</div>
      </component>
    </div>

    <div :class="$style.panelGroup">
      <h3>{{ this.$t('appointments.index.appointmentTypeLabel') }}</h3>
      <p :class="$style.session" data-label="slot type">
        {{ appointment.type }}
      </p>
      <p v-if="appointment.sessionName" :class="$style.sessionName" data-label="session name">
        {{ appointment.sessionName }}
      </p>
      <p v-for="(clinician, index) in appointment.clinicians"
         :key="clinician" :class="$style.person">
        <span :data-label="'clinician ' + (index + 1)">
          {{ clinician }}
        </span>
      </p>
    </div>

    <div :class="$style.panelGroup">
      <h3>{{ this.$t('appointments.index.locationLabel') }}</h3>
      <p :class="$style.location">
        <span data-label="location">{{ appointment.location }}</span>
      </p>
    </div>

    <span v-if="showCancellationLink && !cancellationDisabled && !appointment.disableCancellation"
          :class="$style.panelGroup">
      <hr :class="$style.cancel"
          aria-hidden="true">
      <p>
        <a :class="[$style['nhsuk-action-link__link'], $style['cancel-link']]"
           :href="appointmentCancellingPath"
           @click.stop.prevent="onCancel">
          {{ this.$t('appointments.index.cancelButtonText') }}
        </a>
      </p>
    </span>

    <span v-if="showCancellationLink && (cancellationDisabled || appointment.disableCancellation)"
          :class="$style.panelGroup">
      <hr :class="$style.cancel"
          aria-hidden="true">
      <p :class="$style['cancel-disabled']">
        {{ this.$t('appointments.index.cancellationDisabledText') }}
      </p>
    </span>
  </div>
</template>

<script>
import moment from 'moment-timezone';
import { APPOINTMENT_CANCELLING } from '@/lib/routes';
import { createUri } from '@/lib/noJs';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'Appointment',
  props: {
    appointment: {
      type: Object,
      required: true,
    },
    showCancellationLink: {
      default: true,
      type: Boolean,
    },
    cancellationDisabled: {
      default: false,
      type: Boolean,
    },
    dateTimeHeader: {
      type: String,
      default: 'h3',
      validator: value => ['h2', 'h3'].indexOf(value) !== -1,
    },
  },
  computed: {
    appointmentCancellingPath() {
      const noJsData = {
        myAppointments: {
          selectedAppointment: this.appointment,
          cancellationReasons: this.$store.state.myAppointments.cancellationReasons,
        },
      };
      return createUri({ path: APPOINTMENT_CANCELLING.path, noJs: noJsData });
    },
  },
  methods: {
    formatTime: dateTime => moment.tz(dateTime, 'Europe/London').format('h:mma'),
    formatDate: dateTime => moment.tz(dateTime, 'Europe/London').format('dddd D MMMM YYYY'),
    onCancel() {
      if (this.showCancellationLink) {
        this.$store.dispatch('myAppointments/select', this.appointment);
      }
      redirectTo(this, APPOINTMENT_CANCELLING.path, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/panels";
  @import "../../style/nhsuklinks";

  .panel {
    .panelGroup {
      padding-top: 0.25em;
      padding-bottom: 0.75em;
    }

    .date {
      font-weight: bold;
      font-size: 18px;
    }

    .time {
      font-weight: normal;
      font-size: 25px;
      line-height: 1.5em;
    }

    h3 {
      font-weight: bold;
      font-size: 16px;
    }

    .date-time-header {
      font-family: $default-web;
      display: block;
      line-height: 1.5em;

      span.time {
        font-family: $default-web;
      }
    }

    p {
      font-family: $default-web;
      padding-top: 0 !important;
      padding-bottom: 0 !important;
    }

    p.person,
    p.location,
    p.cancel-disabled {
      font-family: $default-web;
    }

    a.cancel-link {
      font-family: $default-web;
      font-weight: normal;
    }

    p.sessionName {
      font-size: 1em;
    }

    h3 {
      padding-bottom: 0;
      padding-top: 0;
    }

    hr {
      margin-bottom: 0.5em;
    }

    a.cancel-link,
    a.cancel-disabled {
      margin: 0.5em 0;
    }

    &.desktopWeb {
      p {
        max-width: 540px;
      }
    }
  }

</style>
