<template>
  <div v-if="showTemplate" class="pull-content">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <message-dialog v-if="showError" message-type="error" role="alert">
          <message-text data-purpose="error-heading">
            {{ $t('appointments.cancelling.noReasonDialogError') }}
          </message-text>
          <message-list data-purpose="reason-error">
            <li>{{ $t('appointments.cancelling.noReasonError') }}</li>
          </message-list>
        </message-dialog>
      </div>
    </div>
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <div data-purpose="info">
          <p class="nhsuk-u-padding-bottom-3">{{ $t('appointments.cancelling.info') }}</p>
        </div>
      </div>
    </div>

    <CardGroup class="nhsuk-grid-row">
      <CardGroupItem class="nhsuk-grid-column-one-half">
        <Card>
          <appointment v-if="appointment" :appointment="appointment"
                       :show-cancellation-link="false"
                       data-purpose="appointment-info"
                       :telephone-message="$t('appointments.index.upcoming.telephoneMessage')"
                       date-time-header="h2" />
        </Card>
      </CardGroupItem>
    </CardGroup>


    <form-post :action="appointmentCancelPath">
      <input :value="appointment.id" type="hidden" name="id">
      <div v-if="isReasonRequired" :class="reasonRequiredErrorStyle">
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
            <label for="txt_reason" class="nhsuk-body-m nhsuk-u-margin-bottom-2">
              <strong>{{ $t('appointments.cancelling.form_label') }}</strong>
            </label>
          </div>
        </div>
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
            <error-message v-if="showError" id="error-label" :class="$style.form">
              {{ $t('appointments.cancelling.noReasonError') }}
            </error-message>
          </div>
        </div>
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
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
        </div>
      </div>

      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full nhsuk-u-padding-top-6">
          <generic-button id="btn_cancel_appointment"
                          :button-classes="['nhsuk-button']"
                          click-delay="medium"
                          @click.stop.prevent="onCancelButtonClicked($event)">
            {{ $t('appointments.cancelling.cancelButtonText') }}
          </generic-button>
        </div>
      </div>
    </form-post>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <desktopGenericBackLink
          v-if="!$store.state.device.isNativeApp"
          :path="appointmentPath"
          :button-text="'appointments.cancelling.backButtonText'"
          @clickAndPrevent="onBackButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import Appointment from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import SelectDropdown from '@/components/widgets/SelectDropdown';
import { APPOINTMENTS, APPOINTMENT_CANCEL_NOJS, APPOINTMENT_CANCELLING_SUCCESS } from '@/lib/routes';
import FormPost from '@/components/FormPost';
import { redirectTo } from '@/lib/utils';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  layout: 'nhsuk-layout',

  components: {
    DesktopGenericBackLink,
    Appointment,
    ErrorMessage,
    GenericButton,
    MessageDialog,
    MessageText,
    MessageList,
    SelectDropdown,
    FormPost,
    CardGroup,
    CardGroupItem,
    Card,
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
    reasonRequiredErrorStyle() {
      return this.showError ? 'nhsuk-form-group--error' : '';
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
      redirectTo(this, this.appointmentPath);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearSelectedAppointment');
  },
  methods: {
    async onCancelButtonClicked() {
      if (this.cancellationReasons.length === 0 || this.selectedReason) {
        this.submissionError = false;

        const data = {
          appointmentId: this.appointment.id,
          cancellationReasonId: this.selectedReason,
        };
        this.labelledBy = undefined;
        try {
          await this.$store.dispatch('myAppointments/cancel', data);

          let successPath;
          if (this.$store.getters['session/isProxying']) {
            successPath = APPOINTMENT_CANCELLING_SUCCESS.path;
          } else {
            this.$store.dispatch('flashMessage/addSuccess', this.$t('appointments.cancelling.successText'));
            successPath = APPOINTMENTS.path;
          }
          redirectTo(this, successPath);
        } catch (error) {
          /*
          empty catch block as the
          ApiError.vue (component) handles and
          surfaces appropriate error content based on the http status code returned from the API
          */
        }
      } else {
        this.submissionError = true;
        window.scrollTo(0, 0);
        this.labelledBy = 'error-label';
      }
    },
    onBackButtonClicked() {
      redirectTo(this, this.appointmentPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/forms";
@import "../../style/info";
@import "../../style/desktopWeb/inputcontrol";

</style>
