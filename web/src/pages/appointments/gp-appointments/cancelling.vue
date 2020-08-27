<template>
  <div v-if="showTemplate">
    <div v-if="error">
      <error-container v-if="error.status===400" :id="generateErrorId()">
        <error-title title="appointments.error.thereIsAProblemAppointments"
                     header="appointments.error.thereIsAProblem" />
        <error-paragraph from="appointments.error.tryAgainOrContactSurgeryOrOneOneOne" />
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true" />
      </error-container>
      <error-container v-else-if="error.status===403" :id="generateErrorId()">
        <error-title title="appointments.error.thereIsAProblemWithTheServiceAppointments"
                     header="appointments.error.contactYouSurgeryToCancel"/>
        <error-paragraph from="appointments.error.youCannotCancelRightNow" />
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true" />
      </error-container>
      <error-container v-else-if="error.status===409" :id="generateErrorId()">
        <error-title title="appointments.error.youCannotCancelThisAppointment"/>
        <error-paragraph from="appointments.error.theAppointmentMayBeCancelledOrInThePast" />
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true" />
      </error-container>
      <error-container v-else-if="error.status===461" :id="generateErrorId()">
        <error-title title="appointments.error.contactYouSurgeryToCancel"/>
        <error-paragraph from="appointments.error.itIsTooLateToCancel" />
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true" />
      </error-container>
      <error-container v-else-if="error.status===500 || error.status===502 || error.status===504"
                       :id="generateErrorId()">
        <error-title title="appointments.error.thereIsAProblemAppointments"
                     header="appointments.error.thereIsAProblem" />
        <error-paragraph from="appointments.error.tryAgainOrContactUs"
                         :variable="error.serviceDeskReference"/>
        <error-paragraph from="appointments.error.ifTheProblemContinuesAndYouNeedToBookOrCancel"/>
        <error-link from="generic.contactUsButton.text"
                    :action="contactUsUrl"
                    target="_blank"/>
        <error-link from="generic.backButton.text"
                    :action="appointmentsPath"
                    :desktop-only="true"/>
      </error-container>
    </div>

    <div v-else class="pull-content">
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <message-dialog v-if="showError" message-type="error" role="alert">
            <message-text data-purpose="error-heading">
              {{ $t('appointments.cancel.thereIsAProblem') }}
            </message-text>
            <message-list data-purpose="reason-error">
              <li>{{ $t('appointments.cancel.selectAReason') }}</li>
            </message-list>
          </message-dialog>

          <div data-purpose="info">
            <p class="nhsuk-u-padding-bottom-3">
              {{ $t('appointments.cancel.checkDetailsBeforeCancelling') }}</p>
          </div>
        </div>
      </div>

      <CardGroup class="nhsuk-grid-row">
        <CardGroupItem class="nhsuk-grid-column-one-half">
          <Card>
            <appointment v-if="appointment" :appointment="appointment"
                         :show-cancellation-link="false"
                         data-purpose="appointment-info"
                         :telephone-message="$t('appointments.upcoming.weWillCallYouOn')"
                         date-time-header="h2" />
          </Card>
        </CardGroupItem>
      </CardGroup>

      <div v-if="isReasonRequired" :class="{ showError: 'nhsuk-form-group--error' }">
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-full">
            <label for="txt_reason" class="nhsuk-body-m nhsuk-u-margin-bottom-2">
              <strong>{{ $t('appointments.cancel.reasonForCancelling') }}</strong>
            </label>

            <error-message v-if="showError" id="error-label" :class="$style.form">
              {{ $t('appointments.cancel.selectAReason') }}
            </error-message>

            <select-dropdown v-model="selectedReason" :a-labelled-by="labelledBy"
                             :class="[$style.reason, showError && $style.errorBorder]"
                             select-id="txt_reason" select-name="reason">
              <option disabled="" selected="" value="">
                {{ $t('appointments.cancel.selectReason') }}
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
            {{ $t('appointments.cancel.cancelAppointment') }}
          </generic-button>

          <desktopGenericBackLink
            v-if="!$store.state.device.isNativeApp"
            :path="appointmentsPath"
            :button-text="'generic.backButton.text'"
            @clickAndPrevent="onBackButtonClicked"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Appointment from '@/components/appointments/Appointment';
import Card from '@/components/widgets/card/Card';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorContainer from '@/components/errors/ErrorContainer';
import ErrorLink from '@/components/errors/ErrorLink';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import ErrorParagraph from '@/components/errors/ErrorParagraph';
import ErrorTitle from '@/components/errors/ErrorTitle';
import GenericButton from '@/components/widgets/GenericButton';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageList from '@/components/widgets/MessageList';
import MessageText from '@/components/widgets/MessageText';
import SelectDropdown from '@/components/widgets/SelectDropdown';

import { redirectTo } from '@/lib/utils';
import {
  GP_APPOINTMENTS_PATH,
  APPOINTMENT_CANCELLING_SUCCESS_PATH,
} from '@/router/paths';

export default {
  name: 'GpAppointmentsCancellingPage',
  components: {
    Appointment,
    Card,
    CardGroup,
    CardGroupItem,
    DesktopGenericBackLink,
    ErrorContainer,
    ErrorLink,
    ErrorMessage,
    ErrorParagraph,
    ErrorTitle,
    GenericButton,
    MessageDialog,
    MessageList,
    MessageText,
    SelectDropdown,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      appointmentsPath: GP_APPOINTMENTS_PATH,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      labelledBy: undefined,
      selectedReason: '',
      submissionError: false,
      appointment: this.$store.state.myAppointments.selectedAppointment,
      cancellationReasons: this.$store.state.myAppointments.cancellationReasons,
    };
  },
  computed: {
    isReasonRequired() {
      return this.cancellationReasons.length > 0;
    },
    error() {
      return this.$store.state.myAppointments.error;
    },
    showError() {
      return this.submissionError && !this.selectedReason;
    },
  },
  created() {
    if (!this.appointment) {
      redirectTo(this, this.appointmentsPath);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('myAppointments/clearSelectedAppointment');
  },
  methods: {
    generateErrorId() {
      return `error-dialog-${this.error.status}`;
    },
    onBackButtonClicked() {
      redirectTo(this, this.appointmentsPath);
    },
    async onCancelButtonClicked() {
      if (this.cancellationReasons.length === 0 || this.selectedReason) {
        this.submissionError = false;

        const data = {
          appointmentId: this.appointment.id,
          cancellationReasonId: this.selectedReason,
        };
        this.labelledBy = undefined;

        await this.$store.dispatch('myAppointments/cancel', data);
        if (this.error) {
          return;
        }
        redirectTo(this, APPOINTMENT_CANCELLING_SUCCESS_PATH);
      } else {
        this.submissionError = true;
        window.scrollTo(0, 0);
        this.labelledBy = 'error-label';
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/forms";
@import "../../../style/info";
@import "../../../style/desktopWeb/inputcontrol";
</style>
