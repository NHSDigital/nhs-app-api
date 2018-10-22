<template>
  <div v-if="showTemplate" class="pull-content">
    <div>
      <form :action="confirmBookingPath" method="post">
        <input :value="$store.state.session.csrfToken" type="hidden" name="csrfToken">
        <input :value="slotEndTime" type="hidden" name="endTime">
        <input :value="slotId" type="hidden" name="slotId">
        <input :value="slotStartTime" type="hidden" name="startTime">

        <message-dialog v-if="showError" message-type="error">
          <message-text data-purpose="error-heading">
            {{ $t('appointments.confirmation.noReasonDialogError') }}
          </message-text>
          <message-list data-purpose="error">
            <li>{{ $t('appointments.confirmation.noReasonError') }}</li>
          </message-list>
        </message-dialog>

        <div :class="$style.info" data-purpose="info">
          <p>{{ $t('appointments.confirmation.info') }}</p>
        </div>

        <appointment-slot v-if="slot" :appointment="slot" :show-cancellation-link="false"
                          aria-label="selected appointment" />
        <div v-if="showBookingReason()" :class="[$style.form, $style.reasonForm]" role="form">
          <label :class="$style.textReasonLabel" for="reasonText">
            {{ $t('appointments.confirmation.headerLabel') }}
            {{ bookingReasonOptional() ? $t('appointments.confirmation.headerLabelSuffix') : '' }}
          </label>

          <error-message v-if="showError" id="error-label">
            {{ $t('appointments.confirmation.noReasonError') }}
          </error-message>
          <generic-text-area id="reasonText"
                             ref="reason"
                             :a-labelled-by="reasonBoxAriaLabelledBy"
                             :initial-contents="symptoms"
                             :text-area-classes="defaultClasses"
                             v-model="symptoms"
                             name="bookingReason"
                             maxlength="150"
                             required="true"/>

          <p id="max-reason-desc" :class="$style.char">
            {{ $t('appointments.confirmation.reasonDesc.line1') }}
          </p>
          <p>
            {{ $t('appointments.confirmation.reasonDesc.line2') }}
            <br >
            {{ $t('appointments.confirmation.reasonDesc.line3') }}
          </p>
        </div>

        <generic-button id="btn_book_appointment" :class="[$style.button, $style.green]"
                        @click="onConfirmButtonClicked">
          {{ $t('appointments.confirmation.confirmButtonText') }}
        </generic-button>
      </form>

      <form :action="cancelBookingPath" method="get">
        <generic-button id="btn_cancel_appointment" :class="[$style.button,$style.grey]"
                        @click="onCancelButtonClicked">
          {{ $t('appointments.confirmation.changeButtonText') }}
        </generic-button>
      </form>
    </div>
  </div>
</template>


<script>
import qs from 'qs';
import AppointmentSlot from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericButton from '@/components/widgets/GenericButton';
import { APPOINTMENTS, APPOINTMENT_BOOKING, APPOINTMENT_BOOK_NOJS } from '@/lib/routes';
import Necessity from '@/lib/necessity';

export default {
  components: {
    MessageDialog,
    MessageText,
    MessageList,
    AppointmentSlot,
    ErrorMessage,
    GenericTextArea,
    GenericButton,
  },
  asyncData({ req, store }) {
    const data = {
      slot: null,
      symptoms: '',
      submissionError: false,
    };

    const query = req ? qs.parse(req.url.substr(req.url.indexOf('?') + 1)) : {};
    const { slot, bookingReasonNecessity } = query;
    if (slot) {
      data.slot = JSON.parse(slot);
    }

    if (bookingReasonNecessity) {
      store.dispatch('availableAppointments/setBookingReasonNecessity', bookingReasonNecessity);
    }

    return data;
  },
  computed: {
    cancelBookingPath() {
      return APPOINTMENT_BOOKING.path;
    },
    confirmBookingPath() {
      return APPOINTMENT_BOOK_NOJS.path;
    },
    reasonBoxAriaLabelledBy() {
      return this.showError ? 'error-label max-reason-desc' : 'max-reason-desc';
    },
    defaultClasses() {
      return this.showError ? [this.$style.error] : undefined;
    },
    showError() {
      return this.submissionError && !this.symptoms;
    },
    confirmationMessage() {
      return this.$store.state.myAppointments.disableCancellation
        ? this.$t('appointments.index.succcessAndCancellationDisabledText')
        : this.$t('appointments.index.successText');
    },
    slotEndTime() {
      if (!this.slot) return undefined;
      return this.slot.endTime;
    },
    slotId() {
      if (!this.slot) return undefined;
      return this.slot.id;
    },
    slotStartTime() {
      if (!this.slot) return undefined;
      return this.slot.startTime;
    },
  },
  watch: {
    symptoms(val, oldValue) {
      if (val.length > 150) {
        this.symptoms = oldValue;
      }
    },
  },
  mounted() {
    this.slot = this.$store.state.availableAppointments.selectedSlot;
    if (!this.slot) {
      this.$router.push(APPOINTMENT_BOOKING.path);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('availableAppointments/deselect');
  },
  methods: {
    showBookingReason() {
      return this.$store.state.availableAppointments
        .bookingReasonNecessity !== Necessity.NotAllowed;
    },
    bookingReasonOptional() {
      return this.$store.state.availableAppointments
        .bookingReasonNecessity === Necessity.Optional;
    },
    onConfirmButtonClicked(e) {
      e.preventDefault();
      const isMandatory = this.$store.state.availableAppointments
        .bookingReasonNecessity === Necessity.Mandatory;
      this.symptoms = this.symptoms.trim();
      if (this.symptoms.length === 0 && isMandatory) {
        this.submissionError = true;
        window.scrollTo(0, 0);
        return;
      }
      this.submissionError = false;
      this.confirmTheBook(this.slot, this.symptoms);
    },
    confirmTheBook(slot, reason) {
      if (!slot) return;

      const bookingData = {
        SlotId: slot.id,
        BookingReason: reason,
        StartTime: slot.startTime,
        EndTime: slot.endTime,
      };
      this.$store.dispatch('availableAppointments/book', bookingData)
        .then(() => {
          this.$store.dispatch('flashMessage/addSuccess', this.confirmationMessage);
          this.$router.push(APPOINTMENTS.path);
        });
    },
    onCancelButtonClicked(e) {
      this.$router.push(APPOINTMENT_BOOKING.path);
      e.preventDefault();
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/forms";
@import "../../style/info";

.reasonForm {
  margin-bottom: 24px;
}

.textReasonLabel {
  padding-top: 8px;
}
</style>
