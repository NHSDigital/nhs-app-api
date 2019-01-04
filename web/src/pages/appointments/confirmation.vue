<template>
  <div v-if="showTemplate" class="pull-content">
    <div>
      <form-post :action="confirmBookingPath">
        <input :value="confirmationMessageKey" type="hidden" name="successMessageKey">
        <input :value="slotEndTime" type="hidden" name="endTime">
        <input :value="slotId" type="hidden" name="slotId">
        <input :value="slotStartTime" type="hidden" name="startTime">

        <message-dialog v-if="showError" message-type="error">
          <message-text data-purpose="error-heading">
            {{ $t('appointments.confirmation.errorDialog') }}
          </message-text>
          <div data-purpose="error-dialog-list">
            <message-list>
              <li v-if="showTelephoneError" data-purpose="telephone-error">
                <p>{{ $t('appointments.confirmation.noPhoneNumberError') }}</p>
              </li>
              <li v-if="showReasonError" data-purpose="reason-error">
                <p>{{ $t('appointments.confirmation.noReasonError') }}</p>
              </li>
            </message-list>
          </div>
        </message-dialog>

        <div :class="$style.info" data-purpose="info">
          <p>{{ $t('appointments.confirmation.info') }}</p>
        </div>

        <appointment-slot v-if="slot" :appointment="slot" :show-cancellation-link="false"
                          aria-label="selected appointment" />
        <div v-if="showPhoneNumber()" :class="[$style.form, $style.phoneNumberForm]"
             role="form" data-purpose="phone-number">
          <label :class="$style.textPhoneNumberLabel" for="telephoneNumberText">
            {{ $t('appointments.confirmation.telephoneNumberLabel') }}
          </label>
          <error-message v-if="showTelephoneError" id="telephone-error-label">
            {{ $t('appointments.confirmation.noPhoneNumberError') }}
          </error-message>
          <generic-text-input id="telephoneNumberText"
                              ref="telephone"
                              :initial-contents="telephoneNumber"
                              :text-area-classes="defaultClasses"
                              v-model="telephoneNumber"
                              name="telephoneNumberField"
                              required="true"
                              pattern=".*[^ ].*"
                              type="tel"/>
          <p>
            {{ $t('appointments.confirmation.telephoneNumberDescription') }}
          </p>
        </div>
        <div v-if="showBookingReason()" :class="[$style.form, $style.reasonForm]"
             role="form" data-purpose="booking-reason">
          <label :class="$style.textReasonLabel" for="reasonText">
            {{ $t('appointments.confirmation.headerLabel') }}
            {{ bookingReasonOptional() ? $t('appointments.confirmation.headerLabelSuffix') : '' }}
          </label>

          <error-message v-if="showReasonError" id="reason-error-label">
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
                        @click.prevent="onConfirmButtonClicked">
          {{ $t('appointments.confirmation.confirmButtonText') }}
        </generic-button>
      </form-post>

      <no-js-form :action="cancelBookingPath" :value="formData">
        <generic-button id="btn_cancel_appointment" :class="[$style.button,$style.grey]"
                        @click.stop.prevent="onCancelButtonClicked">
          {{ $t('appointments.confirmation.changeButtonText') }}
        </generic-button>
      </no-js-form>
    </div>
  </div>
</template>


<script>
import AppointmentSlot from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { APPOINTMENTS, APPOINTMENT_BOOKING, APPOINTMENT_BOOK_NOJS } from '@/lib/routes';
import Necessity from '@/lib/necessity';
import FormPost from '@/components/FormPost';
import NoJsForm from '@/components/no-js/NoJsForm';
import channel from '@/lib/channel';

export default {
  components: {
    GenericTextInput,
    MessageDialog,
    MessageText,
    MessageList,
    AppointmentSlot,
    ErrorMessage,
    GenericTextArea,
    GenericButton,
    FormPost,
    NoJsForm,
  },
  data() {
    return {
      symptoms: '',
      slot: this.$store.state.availableAppointments.selectedSlot,
      reasonError: false,
      telephoneNumber: '',
      telephoneNumberError: false,
    };
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
    showReasonError() {
      return this.reasonError && !this.symptoms;
    },
    showTelephoneError() {
      return this.telephoneNumberError && !this.telephoneNumber;
    },
    showError() {
      return this.reasonError || this.telephoneNumberError;
    },
    confirmationMessageKey() {
      return this.$store.state.myAppointments.disableCancellation
        ? 'appointments.index.succcessAndCancellationDisabledText'
        : 'appointments.index.successText';
    },
    confirmationMessage() {
      return this.$t(this.confirmationMessageKey);
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
    formData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
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
    showPhoneNumber() {
      return (this.slot || {}).channel === channel.Telephone;
    },
    onConfirmButtonClicked(e) {
      e.preventDefault();
      this.reasonError = false;
      this.telephoneNumberError = false;

      if (this.slot.channel === channel.Telephone) {
        this.telephoneNumber = this.telephoneNumber.trim();
        if (this.telephoneNumber.length === 0) {
          this.telephoneNumberError = true;
          this.showError = true;
        }
      }

      const isMandatory = this.$store.state.availableAppointments
        .bookingReasonNecessity === Necessity.Mandatory;
      this.symptoms = this.symptoms.trim();
      if (this.symptoms.length === 0 && isMandatory) {
        this.reasonError = true;
        this.showError = true;
      }

      if (this.showError) {
        window.scrollTo(0, 0);
        return;
      }

      this.confirmTheBook(this.slot, this.symptoms, this.telephoneNumber);
    },
    confirmTheBook(slot, reason, telephoneNumberField) {
      if (!slot) return;

      const bookingData = {
        SlotId: slot.id,
        BookingReason: reason,
        StartTime: slot.startTime,
        EndTime: slot.endTime,
        TelephoneNumber: telephoneNumberField,
      };
      this.$store.dispatch('availableAppointments/book', bookingData)
        .then(() => {
          this.$store.dispatch('flashMessage/addSuccess', this.confirmationMessage);
          this.$router.push(APPOINTMENTS.path);
        });
    },
    onCancelButtonClicked() {
      this.$router.push(this.cancelBookingPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/forms";
@import "../../style/info";

.reasonForm, .phoneNumberForm {
  margin-bottom: 24px;
}

.textReasonLabel, .textPhoneNumberLabel {
  padding-top: 8px;
}
</style>
