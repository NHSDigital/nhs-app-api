<template>
  <div v-if="showTemplate" class="pull-content">
    <message-dialog v-if="showValidationError" message-type="error">
      <message-text data-purpose="error-heading">
        {{ $t('appointments.cancel.noReasonDialogError') }}
      </message-text>
      <message-list data-purpose="error">
        <li>{{ $t('appointments.cancel.noReasonError') }}</li>
      </message-list>
    </message-dialog>
    <div :class="$style.info" data-purpose="info">
      <p>{{ $t('appointments.cancel.info') }}</p>
    </div>

    <appointment v-if="appointment" :appointment="appointment" :show-cancellation-link="false" />

    <div v-if="isReasonRequired" :class="$style.form">
      <label for="txt_reason">
        {{ $t('appointments.cancel.form_label') }}
      </label>

      <error-message v-if="showValidationError" id="error-label" :class="$style.form">
        {{ $t('appointments.cancel.noReasonError') }}
      </error-message>

      <select-dropdown v-model="selectedReason" :a-labelled-by="labelledBy"
                       :error-border="showValidationError"
                       select-id = "txt_reason" select-name="reason">
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
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import Appointment from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import Routes from '@/Routes';

export default {
  components: {
    Appointment,
    MessageDialog,
    MessageText,
    MessageList,
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
      labelledBy: undefined,
    };
  },
  mounted() {
    this.appointment = this.$store.state.myAppointments.selectedAppointment;
    this.cancellationReasons = this.$store.state.myAppointments.cancellationReasons;
    this.isReasonRequired = this.cancellationReasons.length > 0;

    if (!this.appointment) {
      this.$router.push('/appointments');
    }
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
        this.labelledBy = undefined;

        this.$store.dispatch('myAppointments/cancel', data)
          .then(() => {
            this.$store.dispatch('flashMessage/addSuccess', this.$t('appointments.cancel.successText'));
            this.$router.push(Routes.APPOINTMENTS.path);
          });
      } else {
        this.showValidationError = true;
        document.getElementById('txt_reason').focus();
        this.labelledBy = 'errorLabel';
      }
    },
    onBackButtonClicked() {
      this.$router.push('/appointments');
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/forms";
@import "../../style/info";
</style>
