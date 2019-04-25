<template>
  <div :class="[$style.panel, !$store.state.device.isNativeApp && $style.desktopWeb]">
    <component :is="dateTimeHeader" :class="$style['date-time-header']">
      <span role="text">
        <span :class="$style.date"
              data-label="date">{{ formatDate(appointment.startTime) }}&nbsp;</span>
        <span :class="$style.time"
              data-label="start time">{{ formatTime(appointment.startTime) }}</span>
      </span>
    </component>
    <hr aria-hidden="true">
    <p :class="[$style.session, appointment.sessionName && $style.reducedPadding]"
       data-label="slot type">
      {{ appointment.type }}
    </p>
    <p v-if="appointment.sessionName"
       :class="$style.sessionName"
       data-label="session name">
      {{ appointment.sessionName }}
    </p>
    <hr aria-hidden="true">

    <p :class="$style.location">
      <location-icon/>&nbsp;
      <span data-label="location">{{ appointment.location }}</span>
    </p>

    <p v-for="(clinician, index) in appointment.clinicians" :key="clinician"
       :class="$style.person">
      <clinician-icon/>&nbsp;
      <span :data-label="'clinician ' + (index +1)">
        {{ clinician }}
      </span>
    </p>

    <span v-if="showCancellationLink && !cancellationDisabled && !appointment.disableCancellation">
      <hr :class="$style.cancel"
          aria-hidden="true">
      <p>
        <a :class="$style['cancel-link']"
           :href="appointmentCancellingPath"
           @click.stop.prevent="onCancel">
          {{ this.$t('appointments.index.cancelButtonText') }}
        </a>
      </p>
    </span>
    <span v-if="showCancellationLink && (cancellationDisabled || appointment.disableCancellation)">
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
import LocationIcon from '@/components/icons/LocationIcon';
import ClinicianIcon from '@/components/icons/ClinicianIcon';
import { APPOINTMENT_CANCELLING } from '@/lib/routes';
import { createUri } from '@/lib/noJs';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'Appointment',
  components: {
    ClinicianIcon,
    LocationIcon,
  },
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

.panel {
  &.desktopWeb {
    hr {
      opacity: unset;
    }

    .date-time-header {
      font-family: $default-web;
      font-weight: lighter;
      display: block;
      font-size: 1em;
      line-height: 1.5em;
      span.time {
        font-family: $default-web;
        font-weight: lighter;
      }
    }

    p {
      font-family: $default-web;
      font-weight: lighter;
      max-width: 540px;
    }

    p.person,
    p.location,
    p.cancel-disabled {
      font-family: $default-web;
      font-weight: lighter;
    }

    a.cancel-link {
      font-family: $default-web;
      font-weight: normal;
    }

    :visited {
      color: #D40003;
      outline-color: $focus_highlight;
    }
  }

  p.sessionName {
    font-weight: lighter;
    font-size: 0.875em;
    padding-top: 0.25em;
    padding-bottom: 0.75em;
  }

  p.reducedPadding {
    padding-bottom: 0 !important;
  }

  h3 {
    padding-bottom: 0em;
    padding-top: 0em;

    span.date {
      display: block;
      padding-bottom: 0.5em;
      padding-top: 0.5em;
    }

    span.time {
      @include default_label;
      letter-spacing: $kernel;
      color: $light_grey;
      font-size: 1.250em;
      padding-bottom: 0.5em;
      padding-top: 0em;
    }
  }

  .cancel {
    margin-top: 0.5em;
  }
}

</style>
