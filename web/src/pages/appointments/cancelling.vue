<template>
  <div v-if="showTemplate" class="pull-content">
    <message-dialog v-if="showError" message-type="error" role="alert">
      <message-text data-purpose="error-heading">
        {{ $t('appointments.cancelling.noReasonDialogError') }}
      </message-text>
      <message-list data-purpose="reason-error">
        <li>{{ $t('appointments.cancelling.noReasonError') }}</li>
      </message-list>
    </message-dialog>
    <div :class="$style.info" data-purpose="info">
      <p>{{ $t('appointments.cancelling.info') }}</p>
    </div>

    <div :class="$style.appointmentContainer">
      <appointment v-if="appointment" :appointment="appointment" :show-cancellation-link="false"
                   aria-label="selected appointment"
                   date-time-header="h2" />
    </div>
    <form-post :action="appointmentCancelPath">
      <input :value="appointment.id" type="hidden" name="id">
      <div :class="[$style.form, $style.cancellationReason,
                    !$store.state.device.isNativeApp && $style.desktopWeb]">
        <div v-if="isReasonRequired">
          <label for="txt_reason">
            {{ $t('appointments.cancelling.form_label') }}
          </label>

          <error-message v-if="showError" id="error-label" :class="$style.form">
            {{ $t('appointments.cancelling.noReasonError') }}
          </error-message>

          <select-dropdown v-model="selectedReason" :a-labelled-by="labelledBy"
                           :class="[$style.reason, showError && $style.errorBorder]"
                           select-id="txt_reason" select-name="reason">
            <option disabled="" selected="" value="">
              {{ $t('appointments.cancelling.dropdownDefaultOption') }}
            </option>
            <option v-for="reason in cancellationReasons" :key="reason.id" :value="reason.id">
              {{ reason.displayName }}
            </option>
          </select-dropdown>

        </div>
        <generic-button id="btn_cancel_appointment"
                        :button-classes="['button', 'green']"
                        click-delay="medium"
                        @click.stop.prevent="onCancelButtonClicked($event)">
          {{ $t('appointments.cancelling.cancelButtonText') }}
        </generic-button>
      </div>
    </form-post>

    <generic-button v-if="$store.state.device.isNativeApp"
                    id="btn_back_appointment"
                    :button-classes="['button', 'grey']"
                    @click.stop.prevent="onBackButtonClicked">
      {{ $t('appointments.cancelling.backButtonText') }}
    </generic-button>
    <desktopGenericBackLink
      v-else
      :path="appointmentPath"
      :button-text="'appointments.cancelling.backButtonText'"
      @clickAndPrevent="onBackButtonClicked"/>
  </div>
</template>

<script>
import Appointment from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import GenericButton from '@/components/widgets/GenericButton';
import { APPOINTMENTS, APPOINTMENT_CANCEL_NOJS } from '@/lib/routes';
import FormPost from '@/components/FormPost';
import { redirectTo } from '@/lib/utils';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';

export default {
  components: {
    DesktopGenericBackLink,
    GenericButton,
    Appointment,
    MessageDialog,
    MessageText,
    MessageList,
    ErrorMessage,
    SelectDropdown,
    FormPost,
  },
  data() {
    return {
      appointment: null,
      cancellationReasons: [],
      submissionError: false,
      isReasonRequired: true,
      selectedReason: '',
      labelledBy: undefined,
    };
  },
  computed: {
    showError() {
      return this.submissionError && !this.selectedReason;
    },
    appointmentPath() {
      return APPOINTMENTS.path;
    },
    appointmentCancelPath() {
      return APPOINTMENT_CANCEL_NOJS.path;
    },
  },
  created() {
    this.appointment = this.$store.state.myAppointments.selectedAppointment;
    this.cancellationReasons = this.$store.state.myAppointments.cancellationReasons;
    this.isReasonRequired = this.cancellationReasons.length > 0;

    if (!this.appointment) {
      redirectTo(this, this.appointmentPath, null);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearSelectedAppointment');
  },
  methods: {
    onCancelButtonClicked() {
      if (this.cancellationReasons.length === 0 || this.selectedReason) {
        this.submissionError = false;

        const data = {
          appointmentId: this.appointment.id,
          cancellationReasonId: this.selectedReason,
        };
        this.labelledBy = undefined;

        this.$store.dispatch('myAppointments/cancel', data)
          .then(() => {
            this.$store.dispatch('flashMessage/addSuccess', this.$t('appointments.cancelling.successText'));
            redirectTo(this, APPOINTMENTS.path, null);
          });
      } else {
        this.submissionError = true;
        window.scrollTo(0, 0);
        this.labelledBy = 'error-label';
      }
    },
    onBackButtonClicked() {
      redirectTo(this, this.appointmentPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/forms";
@import "../../style/info";
@import "../../style/desktopWeb/inputcontrol";

.cancellationReason {
  &.desktopWeb {
    max-width: 540px;
  }

  .errorBorder{
    max-width: 540px;
  }

  .reason {
    margin-bottom: 2em;
  }
}

.appointmentContainer {
  margin-bottom: 1em;
}

</style>
