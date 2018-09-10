<template>
  <div v-if="showTemplate" class="pull-content">

    <message-dialog v-if="showValidationError" message-type="error">
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
    <div :class="[$style.form, $style.reasonForm]" role="form">
      <label :class="$style.textReasonLabel" for="reasonText">
        {{ $t('appointments.confirmation.headerLabel') }}
      </label>

      <error-message v-if="showValidationError" id="error-label">
        {{ $t('appointments.confirmation.noReasonError') }}
      </error-message>
      <textarea id="reasonText" ref="reason" v-model="symptoms"
                :aria-labelledby="reasonBoxAriaLabelledBy"
                :class="textareaClass" maxlength="150" autocomplete="off"
                autocorrect="off" autocapitalize="off" spellcheck="false"/>
      <p id="max-reason-desc" :class="$style.char">
        {{ $t('appointments.confirmation.reasonDesc.line1') }}
      </p>
      <p>
        {{ $t('appointments.confirmation.reasonDesc.line2') }}
        <br >
        {{ $t('appointments.confirmation.reasonDesc.line3') }}
      </p>
    </div>

    <button id="btn_book_appointment" :class="[$style.button, $style.green]"
            @click="onConfirmButtonClicked">
      {{ $t('appointments.confirmation.confirmButtonText') }}
    </button>
    <button id="btn_cancel_appointment" :class="[$style.button,$style.grey]"
            @click="onCancelButtonClicked">
      {{ $t('appointments.confirmation.changeButtonText') }}
    </button>
  </div>
</template>


<script>
import AppointmentSlot from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import { APPOINTMENTS, APPOINTMENT_BOOKING } from '@/lib/routes';

export default {
  components: {
    MessageDialog,
    MessageText,
    MessageList,
    AppointmentSlot,
    ErrorMessage,
  },
  data() {
    return {
      slot: null,
      symptoms: '',
      showValidationError: false,
    };
  },
  computed: {
    reasonBoxAriaLabelledBy() {
      return this.showValidationError ? 'error-label max-reason-desc' : 'max-reason-desc';
    },
    textareaClass() {
      return this.showValidationError ? this.$style.error : undefined;
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
    onConfirmButtonClicked() {
      this.symptoms = this.symptoms.trim();
      if (this.symptoms.length === 0) {
        this.showValidationError = true;
        this.$refs.reason.focus();
        return;
      }
      this.showValidationError = false;
      this.confirmTheBook(this.slot, this.symptoms);
    },
    confirmTheBook(slot, reason) {
      const bookingData = {
        SlotId: slot.id,
        BookingReason: reason,
        StartTime: slot.startTime,
        EndTime: slot.endTime,
      };
      this.$store.dispatch('availableAppointments/book', bookingData)
        .then(() => {
          this.$store.dispatch('flashMessage/addSuccess', this.$t('appointments.index.successText'));
          this.$router.push(APPOINTMENTS.path);
        });
    },
    onCancelButtonClicked() {
      this.$router.push(APPOINTMENT_BOOKING.path);
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
