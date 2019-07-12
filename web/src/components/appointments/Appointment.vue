<template>
  <div :class="$style['nhsuk-appointment-card']">
    <div :class="$style.appointmentGroup">
      <component :is="dateTimeHeader" :class="$style['date-time-header']">
        <div :class="$style.date"
             data-label="date">{{ formatDate(appointment.startTime) }}</div>
        <div :class="$style.time"
             data-label="start time">{{ formatTime(appointment.startTime) }}</div>
      </component>
    </div>

    <div :class="$style.appointmentGroup">
      <h3>{{ this.$t('appointments.index.appointmentTypeLabel') }}</h3>
      <p :class="$style.session" data-label="slot type">
        {{ appointment.type }}
      </p>
      <p v-if="appointment.sessionName" :class="$style.sessionName" data-label="session name">
        {{ appointment.sessionName }}
      </p>
      <p v-if="showPhoneNumber()" :class="$style.telephone" data-label="phone number">
        {{ telephoneMessage }}{{ appointment.telephoneNumber }}
      </p>
      <p v-for="(clinician, index) in appointment.clinicians"
         :key="clinician" :class="$style.person">
        <span :data-label="'clinician ' + (index + 1)">
          {{ clinician }}
        </span>
      </p>
    </div>

    <div :class="$style.appointmentGroup">
      <h3>{{ this.$t('appointments.index.locationLabel') }}</h3>
      <p :class="$style.location">
        <span data-label="location">{{ appointment.location }}</span>
      </p>
    </div>

    <span v-if="showCancellationLink && !cancellationDisabled && !appointment.disableCancellation"
          :class="$style.appointmentGroup">
      <hr :class="$style.cancel"
          aria-hidden="true">
      <p>
        <a :class="[$style['nhsuk-action-link__link'], $style['cancel-link']]"
           :href="appointmentCancellingPath"
           :aria-label="this.$t('appointments.index.cancelButtonText') + ' - ' +
             formatDate(appointment.startTime) + ' ' + formatTime(appointment.startTime)"
           @click.stop.prevent="onCancel">
          {{ this.$t('appointments.index.cancelButtonText') }}
        </a>
      </p>
    </span>

    <span v-if="showCancellationLink && (cancellationDisabled || appointment.disableCancellation)"
          :class="$style.appointmentGroup">
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
import channel from '@/lib/channel';

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
    telephoneMessage: {
      type: String,
      default: undefined,
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
    showPhoneNumber() {
      return (this.appointment || {}).channel === channel.Telephone;
    },
  },
};
</script>

<style module lang="scss" scoped>
 @import '~nhsuk-frontend/packages/core/settings/colours';
 @import '~nhsuk-frontend/packages/core/tools/mixins';
 @import '~nhsuk-frontend/packages/core/tools/spacing';
 @import '~nhsuk-frontend/packages/core/settings/spacing';
 @import '~nhsuk-frontend/packages/core/tools/sass-mq';
 @import '~nhsuk-frontend/packages/core/tools/typography';
 @import '~nhsuk-frontend/packages/core/settings/typography';
 @import "../../style/desktopWeb/accessibility";

  .nhsuk-appointment-card {

    .appointmentGroup {
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
      display: block;
      line-height: 1.5em;

      span.time {
      }
    }

    p.person,
    p.location,
    p.telephone,
    p.cancel-disabled {
    }

    a.nhsuk-action-link__link {
      font-weight: bold;
      color: $color_nhsuk-red;
    }

    p.sessionName {
      font-size: 1em;
    }

    a.nhsuk-action-link__link,
    a.nhsuk-action-link__link-disabled {
      margin: 0.5em 0;
    }

    a.cancel-link,
    a.cancel-disabled {
      margin: 0.5em 0;
    }

    a.nhsuk-action-link__link {
      &:focus {
        @include linkFocusStyle;
        color: $color_nhsuk-red;

      }

      &:hover {
        @include linkHoverStyle;
        color: $color_nhsuk-red;
      }
    }
  }

</style>
