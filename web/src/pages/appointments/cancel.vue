<template>
  <main v-if="showTemplate" :class="$style.main">
    <error-warning-dialog v-if="showValidationError" error-or-warning="error">
      <p>
        <span data-purpose="error-heading">
          {{ $t('appointments.cancel.noReasonDialogError') }}
        </span><br>
        <span data-purpose="error">
          {{ $t('appointments.cancel.noReasonError') }}
        </span>
      </p>
    </error-warning-dialog>
    <div :class="$style.info">
      <p>{{ $t('appointments.cancel.info') }}</p>
    </div>

    <appointment v-if="appointment" :appointment="appointment" :show-cancellation-link="false" />

    <div v-if="isReasonRequired" :class="$style.form">
      <label for="txt_reason">
        {{ $t('appointments.cancel.form_label') }}
      </label>

      <error-message v-if="showValidationError" id="errorLabel">
        {{ $t('appointments.cancel.noReasonError') }}
      </error-message>

      <select-dropdown v-model="selectedReason" select-id = "txt_reason" select-name="reason">
        <option disabled="" selected="" value="">
          {{ $t('appointments.cancel.dropdownDefaultOption') }}
        </option>
        <option v-for="reason in cancellationReasons" :key="reason.id" :value="reason.id">
          {{ reason.displayName }}
        </option>
      </select-dropdown>
    </div>

    <button id="btn_cancel_appointment"
            :class="[$style.button, $style.green]"
            @click="onCancelButtonClicked">
      {{ $t('appointments.cancel.cancelButtonText') }}
    </button>
    <button id="btn_back_appointment"
            :class="[$style.button, $style.grey]"
            @click="onBackButtonClicked">
      {{ $t('appointments.cancel.backButtonText') }}
    </button>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import Appointment from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';
import SelectDropdown from '@/components/widgets/SelectDropdown';

export default {
  middleware: ['auth', 'meta'],
  components: {
    Appointment,
    ErrorWarningDialog,
    ErrorMessage,
    SelectDropdown,
  },
  data() {
    return {
      appointment: null,
      cancellationReasons: [],
      showValidationError: false,
      isReasonRequired: true,
      selectedReason: '',
    };
  },
  mounted() {
    this.appointment = this.$store.state.myAppointments.selectedAppointment;
    this.cancellationReasons = this.$store.state.myAppointments.cancellationReasons;
    this.isReasonRequired = this.cancellationReasons.length > 0;

    if (!this.appointment) {
      this.$router.push('/appointments');
    }

    this.$store.dispatch('errors/setApiErrorButtonPath', '/appointments');
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearSelectedAppointment');
  },
  methods: {
    onCancelButtonClicked() {
      if (this.cancellationReasons.length === 0 || this.selectedReason) {
        this.showValidationError = false;

        const data = {
          appointmentId: this.appointment.id,
          cancellationReasonId: this.selectedReason,
        };

        this.$store.dispatch('myAppointments/cancel', data);
      } else {
        this.showValidationError = true;
        this.$refs.reason.focus();
      }
    },
    onBackButtonClicked() {
      this.$router.push('/appointments');
    },
  },
};
</script>

<style module lang="scss">
@import "../../style/spacings";
@import "../../style/textstyles";
@import "../../style/buttons";

.main {
  @include space(padding, all, $three);
  &.error {
    border: 3px $error solid;
  }

  .form {
      margin-bottom: 24px;
      label {
        @include default_label;
        padding-top: 16px;
        padding-bottom: 8px;
      }
  }

  .info p {
    display: block;
    font-weight: normal;
    font-size: 1em;
    line-height: 1.5em;
    color: #4A4A4A;
    font-size: 1em;
    margin-bottom: 1em;
  }
}

</style>
