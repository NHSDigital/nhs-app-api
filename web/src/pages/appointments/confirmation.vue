<template>
  <main v-if="showTemplate" :class="$style.main">
    <error-warning-dialog v-if="showValidationError" error-or-warning="error">
      <p>
        <span data-purpose="error-heading">
          {{ $t('appointments.confirmation.noReasonDialogError') }}
        </span><br>
        <span data-purpose="error">
          {{ $t('appointments.confirmation.noReasonError') }}
        </span>
      </p>
    </error-warning-dialog>

    <appointment-slot v-if="slot" :the-slot="slot" :always-deselect="true"
                      aria-label="selected appointment" />
    <div :class="$style.form" role="form">
      <label for="reasonText">{{ $t('appointments.confirmation.headerLabel') }}</label>
      <p>{{ $t('appointments.confirmation.label') }}</p>

      <error-message v-if="showValidationError" id="errorLabel">
        {{ $t('appointments.confirmation.noReasonError') }}
      </error-message>
      <textarea id="reasonText" ref="reason" v-model="symptoms"
                :aria-labelledby="reasonBoxAriaLabelledBy"
                :class="textareaClass" maxlength="150"/>
      <p id="maxReasonDesc">{{ $t('appointments.confirmation.maxReasonDesc') }}</p>
    </div>

    <button id="btn_book_appointment" :class="[$style.button, $style.green]"
            @click="onConfirmButtonClicked">
      {{ $t('appointments.confirmation.confirmButtonText') }}
    </button>
    <button id="btn_cancel_appointment" :class="[$style.button,$style.grey]"
            @click="onCancelButtonClicked">
      {{ $t('appointments.confirmation.changeButtonText') }}
    </button>
  </main>
</template>


<script>
/* eslint-disable import/extensions */
import AppointmentSlot from '@/components/appointments/AppointmentSlot';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';

export default {
  components: {
    ErrorWarningDialog,
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
      return this.showValidationError ? 'errorLabel maxReasonDesc' : 'maxReasonDesc';
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
    this.slot = this.$store.state.appointment.tempSelectedSlot;
    if (!this.slot) {
      this.$router.push('/appointments/booking');
    }
  },
  beforeDestroy() {
    this.$store.dispatch('appointment/reset');
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
      this.confirmTheBook(this.slot.id, this.slot.startTime, this.slot.endTime, this.symptoms);
    },
    confirmTheBook(slotId, startTime, endTime, reason) {
      const bookingData = {
        SlotId: slotId,
        StartTime: startTime,
        EndTime: endTime,
        BookingReason: reason,
      };
      this.$store.dispatch('appointment/bookAppointment', bookingData);
    },
    onCancelButtonClicked() {
      this.$router.push('/appointments/booking');
    },
  },
};
</script>

<style module lang="scss">
  @import "../../style/textstyles";
  @import "../../style/spacings";
  @import "../../style/colours";
  @import "../../style/buttons";

  .main {
    @include space(padding, all, $three);

    .form {
      margin-bottom: 24px;
      label {
        @include default_label;
        padding-top: 16px;
        padding-bottom: 8px;
      }

      p {
        @include default_text;
        padding-bottom: 8px;
      }

      textarea {
        width: 100%;
        min-height: 100px;
        box-sizing: border-box;
        background-color: $white;
        border-radius: 5px;
        padding: 16px;
        border: 1px $light_grey solid;
        outline: none;
        transition: all ease 0.5s;
        resize: none;
        @include default_text;

        &.error {
          border: 3px $error solid;
        }
      }
    }
  }

</style>
