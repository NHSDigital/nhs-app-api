<template>
  <div :class="$style['nhsuk-appointment-card']">
    <div class="nhsuk-u-margin-bottom-3">
      <component :is="dateTimeHeader" class="nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0">
        <p data-label="date" class="nhsuk-u-margin-bottom-0">
          {{ formatDate(appointment.startTime) }}
        </p>
        <p class="nhsuk-body-l nhsuk-u-margin-bottom-0" data-label="start time">
          {{ formatTime(appointment.startTime) }}
        </p>
      </component>
    </div>

    <div>
      <p class="nhsuk-u-margin-0">
        <strong>{{ this.$t('appointments.index.appointmentTypeLabel') }}</strong>
      </p>
      <p class="nhsuk-body-s nhsuk-u-margin-0" data-label="slot type">
        {{ appointment.type }}
      </p>
      <p v-if="appointment.sessionName"
         class="nhsuk-body-s nhsuk-u-margin-0" data-label="session name">
        {{ appointment.sessionName }}
      </p>
      <p v-if="showPhoneNumber()" class="nhsuk-body-s nhsuk-u-margin-0" data-label="phone number">
        {{ telephoneMessage }}{{ appointment.telephoneNumber }}
      </p>
      <p v-for="(clinician, index) in appointment.clinicians" :key="clinician"
         class="nhsuk-body-s nhsuk-u-margin-0" :class="$style.person">
        <span :data-label="'clinician ' + (index + 1)">
          {{ clinician }}
        </span>
      </p>
    </div>

    <div class="nhsuk-u-margin-top-3">
      <p class="nhsuk-u-margin-0"><strong>
        {{ this.$t('appointments.index.locationLabel') }}
      </strong></p>
      <p class="nhsuk-body-s nhsuk-u-margin-0" :class="$style.location">
        <span data-label="location">{{ appointment.location }}</span>
      </p>
    </div>

    <span v-if="showAddToCalendar()" :class="$style.appointmentGroup">
      <hr class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-1" aria-hidden="true">
      <desktopGenericBackLink id="add-event-to-calendar-link"
                              :path="appointmentAddToCalendarPath"
                              :button-text="'appointments.index.addToCalendarText'"
                              clazz="nhsuk-body-s nhsuk-u-margin-bottom-0"
                              @clickAndPrevent="onAddToCalendar"/>
    </span>

    <span v-if="showCancellationLink && !cancellationDisabled && !appointment.disableCancellation"
          :class="$style.appointmentGroup">
      <hr class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-1" aria-hidden="true">
      <p class="nhsuk-body-s nhsuk-u-margin-bottom-0">
        <a :class="$style['nhsuk-action-link__link']"
           :href="appointmentCancellingPath"
           :aria-label="this.$t('appointments.index.cancelButtonText') + ' - ' +
             formatDate(appointment.startTime) + ' ' + formatTime(appointment.startTime)"
           @click.stop.prevent="onCancel">
          {{ this.$t('appointments.index.cancelButtonText') }}
        </a>
      </p>
    </span>

    <span v-if="showCancellationLink && (cancellationDisabled || appointment.disableCancellation)">
      <hr class="nhsuk-u-margin-top-3 nhsuk-u-margin-bottom-1" aria-hidden="true">
      <br>
      <p class="nhsuk-body-s nhsuk-u-margin-bottom-0">
        {{ this.$t('appointments.index.cancellationDisabledText') }}
      </p>
    </span>
  </div>
</template>

<script>
import moment from 'moment-timezone';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { APPOINTMENT_CANCELLING_PATH, APPOINTMENT_ADD_TO_CALENDAR_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import channel from '@/lib/channel';

export default {
  name: 'Appointment',
  components: {
    DesktopGenericBackLink,
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
    showAddToCalendarLink: {
      default: false,
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
  data() {
    return {
      isNativeApp: this.$store.state.device.isNativeApp,
      isAddToCalendarEnabled: this.$store.$env.ADD_APPOINTMENT_TO_CALENDAR_ENABLED,
      appointmentAddToCalendarPath: APPOINTMENT_ADD_TO_CALENDAR_PATH,
      appointmentCancellingPath: APPOINTMENT_CANCELLING_PATH,
      nativeAppVersionSupportsAddingEventsToCalendar:
        window.nativeApp && window.nativeApp.addEventToCalendar,
    };
  },
  methods: {
    formatTime: dateTime => moment.tz(dateTime, 'Europe/London').format('h:mma'),
    formatDate: dateTime => moment.tz(dateTime, 'Europe/London').format('dddd D MMMM YYYY'),
    onCancel() {
      if (this.showCancellationLink) {
        this.$store.dispatch('myAppointments/select', this.appointment);
      }
      redirectTo(this, this.appointmentCancellingPath);
    },
    onAddToCalendar() {
      if (this.showAddToCalendar()) {
        this.$store.dispatch('myAppointments/select', this.appointment);
      }
      redirectTo(this, this.appointmentAddToCalendarPath);
    },
    showAddToCalendar() {
      return this.showAddToCalendarLink
             && this.isAddToCalendarEnabled
             && this.isNativeApp
             // the following check can be removed after 1.37 is no longer supported
             && this.nativeAppVersionSupportsAddingEventsToCalendar;
    },
    showPhoneNumber() {
      return (this.appointment || {}).channel === channel.Telephone;
    },
  },
};
</script>

<style module lang="scss" scoped>
 @import '~nhsuk-frontend/packages/core/settings/colours';

  .nhsuk-appointment-card {

    .date-time-header {
      display: block;

      span.time {
      }
    }

    p {
      word-break: break-word;
    }

    a.nhsuk-action-link__link {
      font-weight: bold;
      color: $color_nhsuk-red;
      display:inline-block;
    }

    a.nhsuk-action-link__link {
      &:focus {
        color: $color_nhsuk-red;
      }

      &:hover {
        color: $color_nhsuk-red;
      }
    }
  }

</style>
