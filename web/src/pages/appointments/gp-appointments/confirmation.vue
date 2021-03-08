<template>
  <div v-if="showTemplate">
    <div v-if="error">
      <booking-confirmation-errors :error="error" />
    </div>
    <div v-else>
      <div v-if="showError" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full" role="alert" aria-atomic="true">
          <message-dialog message-type="error"
                          :focusable="true">
            <message-text data-purpose="error-heading">
              {{ $t('appointments.confirmation.error.thereIsAProblem') }}
            </message-text>
            <message-list>
              <li v-if="showTelephoneError" data-purpose="telephone-error">
                {{ $t('appointments.confirmation.error.enterATelephoneNumber') }}
              </li>
              <li v-if="showReasonError" data-purpose="reason-error">
                {{ $t('appointments.confirmation.error.enterAReason') }}
              </li>
            </message-list>
          </message-dialog>
        </div>
      </div>
      <div class="nhsuk-grid-row" data-purpose="info">
        <div class="nhsuk-grid-column-full">
          <p class="nhsuk-u-padding-bottom-2">
            {{ $t('appointments.confirmation.checkYourDetails') }}
          </p>
        </div>
      </div>
      <CardGroup class="nhsuk-grid-row">
        <CardGroupItem class="nhsuk-grid-column-one-half">
          <Card>
            <appointment-slot v-if="slot" :appointment="slot"
                              :show-cancellation-link="false"
                              data-purpose="appointment-info"
                              date-time-header="h2"/>
          </Card>
        </CardGroupItem>
      </CardGroup>
      <div v-if="showPhoneNumber" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <div role="form" data-purpose="phone-number" :class="telephoneErrorStyle">
            <fieldset class="nhsuk-fieldset nhsuk-form-group--error">
              <legend id="telephone-input-label" class="nhsuk-fieldset__legend">
                <strong>{{ $t('appointments.confirmation.chooseAPhoneNumber') }}</strong>
              </legend>
              <error-message v-if="showTelephoneError"
                             id="telephone-error-label">
                {{ $t('appointments.confirmation.error.enterATelephoneNumber') }}
              </error-message>
              <div v-for="(patientTelephoneNumber, index) in patientTelephoneNumbers"
                   :key="index" class="nhsuk-radios__item">
                <input :id="patientTelephoneNumber.telephoneNumber"
                       v-model="telephoneNumber"
                       :value="patientTelephoneNumber.telephoneNumber"
                       :aria-describedby="showTelephoneError ? 'telephone-error-label' : undefined"
                       type="radio"
                       name="radio"
                       class="nhsuk-radios__input"
                       @change="selected">
                <label :for="patientTelephoneNumber.telephoneNumber"
                       class="nhsuk-label nhsuk-radios__label"
                       @keypress.enter.stop="selected" @click.stop="selected">
                  {{ patientTelephoneNumber.telephoneNumber }}
                </label>
              </div>
              <div v-if="patientTelephoneNumbers.length > 0" class="nhsuk-radios__item">
                <input :id="'otherPhoneNumberRadioInput'"
                       type="radio"
                       name="radio"
                       class="nhsuk-radios__input"
                       @change.stop="selected">
                <label :for="'otherPhoneNumberRadioInput'"
                       class="nhsuk-label nhsuk-radios__label"
                       @keypress.enter.stop="selected" @click.stop="selected">
                  {{ $t('appointments.confirmation.useOtherPhoneNumber') }}
                </label>
              </div>
              <div v-if="showPhoneNumberTextBox">
                <p id="telephone-number-desc" class="nhsuk-u-padding-bottom-2">
                  {{ $t('appointments.confirmation.thisNumberWillOnlyBeUsed') }}
                </p>
                <generic-text-input id="telephoneNumberText"
                                    ref="telephone"
                                    v-model="otherTelephoneNumber"
                                    :a-labelled-by="telephoneNumberTextAriaLabelledBy"
                                    :text-area-classes="defaultClasses"
                                    :required="true"
                                    :error="showTelephoneError"
                                    :class="showReasonError"
                                    name="telephoneNumber"
                                    pattern=".*[^ ].*"
                                    type="tel"/>
              </div>
            </fieldset>
          </div>
        </div>
      </div>
      <div v-if="showBookingReason" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full nhsuk-u-padding-top-3">
          <div role="form" data-purpose="booking-reason" :class="reasonTextErrorStyle">
            <label id="booking-reason-label" class="nhsuk-fieldset__legend"
                   for="reasonText">
              <strong>
                {{ $t('appointments.confirmation.giveAReason.giveAReason') }}
                {{ bookingReasonOptional ?
                  $t('appointments.confirmation.giveAReason.optionalSuffix') : '' }}
              </strong>
            </label>
            <p id="max-reason-desc">
              {{ $t('appointments.confirmation.giveAReason.textMustBeShorterThan') }}
            </p>
            <p>
              {{ $t('appointments.confirmation.giveAReason.textMayNotBeRead') }}
              {{ $t('appointments.confirmation.giveAReason.ifItIsUrgent') }}
            </p>
            <error-message v-if="showReasonError" id="reason-error-label">
              {{ $t('appointments.confirmation.error.enterAReason') }}
            </error-message>
            <generic-text-area id="reasonText"
                               ref="reason"
                               v-model="symptoms"
                               :a-labelled-by="reasonBoxAriaLabelledBy"
                               :text-area-classes="defaultClasses"
                               :required="!bookingReasonOptional"
                               :a-described-by="showReasonError ?
                                 'max-reason-desc reason-error-label' : 'max-reason-desc'"
                               :error.sync="showReasonError"
                               name="bookingReason"
                               maxlength="150"/>
          </div>
        </div>
      </div>
      <div class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <generic-button id="btn_book_appointment"
                          :button-classes="['nhsuk-button']"
                          click-delay="medium"
                          @click.prevent="onConfirmButtonClicked">
            {{ $t('appointments.confirmation.confirmAndBook') }}
          </generic-button>
        </div>
      </div>

      <div v-if="!$store.state.device.isNativeApp" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <desktop-generic-back-link :path="appointmentBookingPath"
                                     :button-text="'generic.back'"
                                     @clickAndPrevent="onCancelButtonClicked"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import moment from 'moment';

import AppointmentSlot from '@/components/appointments/Appointment';
import BookingConfirmationErrors from '@/components/errors/pages/appointments/BookingConfirmationErrors';
import Card from '@/components/widgets/card/Card';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import ErrorPageMixin from '@/components/errors/ErrorPageMixin';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';

import channel from '@/lib/channel';
import necessity from '@/lib/necessity';
import { redirectTo } from '@/lib/utils';
import {
  GP_APPOINTMENTS_PATH,
  APPOINTMENT_BOOKING_PATH,
  APPOINTMENT_CONFIRMATIONS_PATH,
  APPOINTMENT_BOOKING_SUCCESS_PATH,
} from '@/router/paths';
import { EventBus, FOCUS_ERROR_ELEMENT } from '@/services/event-bus';

export default {
  name: 'GpAppointmentsConfirmationPage',
  components: {
    BookingConfirmationErrors,
    AppointmentSlot,
    Card,
    CardGroup,
    CardGroupItem,
    DesktopGenericBackLink,
    ErrorMessage,
    GenericButton,
    GenericTextArea,
    GenericTextInput,
    MessageDialog,
    MessageText,
    MessageList,
  },
  mixins: [ErrorPageMixin],
  data() {
    return {
      appointmentBookingPath: APPOINTMENT_BOOKING_PATH,
      appointmentsPath: GP_APPOINTMENTS_PATH,
      confirmBookingPath: APPOINTMENT_CONFIRMATIONS_PATH,
      contactUsUrl: this.$store.$env.CONTACT_US_URL,
      coronaServiceUrl: this.$store.$env.CORONA_SERVICE_URL,
      isJavascriptOn: false,
      otherTelephoneNumber: '',
      patientTelephoneNumbers: get('availableAppointments.patientTelephoneNumbers')(this.$store.state),
      reasonError: false,
      slot: this.$store.state.availableAppointments.selectedSlot,
      symptoms: '',
      telephoneNumber: '',
      telephoneNumberError: false,
      showPhoneNumberTextBox:
        this.$store.state.availableAppointments.patientTelephoneNumbers.length <= 0,
    };
  },
  computed: {
    confirmationMessage() {
      return this.$t(this.confirmationMessageKey);
    },
    confirmationMessageKey() {
      return this.$store.state.myAppointments.disableCancellation
        ? 'appointments.confirmation.yourAppointmentHasBeenBookedViewHere'
        : 'appointments.confirmation.yourAppointmentHasBeenBookedViewOrCancelHere';
    },
    defaultClasses() {
      return this.showError ? undefined : undefined;
    },
    error() {
      return this.$store.state.availableAppointments.error;
    },
    reasonBoxAriaLabelledBy() {
      return this.showError ? 'booking-reason-label error-label max-reason-desc' : 'booking-reason-label max-reason-desc';
    },
    reasonRequired() {
      return !this.bookingReasonOptional;
    },
    reasonTextErrorStyle() {
      return this.showReasonError ? 'nhsuk-form-group--error' : '';
    },
    showError() {
      return this.showReasonError || this.showTelephoneError;
    },
    showReasonError() {
      return this.reasonError && !this.symptoms;
    },
    showTelephoneError() {
      return this.telephoneNumberError && !this.telephoneNumber;
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
    telephoneNumberTextAriaLabelledBy() {
      return this.showTelephoneError ? 'telephone-error-label telephone-number-desc' : 'telephone-number-desc';
    },
    telephoneErrorStyle() {
      return this.showTelephoneError ? 'nhsuk-form-group--error' : '';
    },
    showBookingReason() {
      return this.$store.state.availableAppointments
        .bookingReasonNecessity !== necessity.NotAllowed;
    },
    showPhoneNumber() {
      return (this.slot || {}).channel === channel.Telephone;
    },
    generateErrorId() {
      return `error-dialog-${this.error.status}`;
    },
    bookingReasonOptional() {
      return this.$store.state.availableAppointments
        .bookingReasonNecessity === necessity.Optional;
    },
  },
  watch: {
    symptoms(value, oldValue) {
      if (value.length > 150) {
        this.symptoms = oldValue;
      }
    },
  },
  mounted() {
    if (!this.slot) {
      redirectTo(this, this.appointmentsPath);
    }
  },
  methods: {
    async confirmTheAppointmentSlot(slot, reason, telephoneNumberField, otherTelephoneNumberField) {
      if (!slot) {
        throw new ErrorMessage('Slot should not be null');
      }

      await this.$store.dispatch('availableAppointments/book', {
        SlotId: slot.id,
        SlotType: slot.type,
        SessionName: slot.sessionName,
        BookingReason: reason,
        StartTime: slot.startTime,
        EndTime: slot.endTime,
        TelephoneNumber: (telephoneNumberField !== null && telephoneNumberField !== '')
          ? telephoneNumberField : otherTelephoneNumberField,
      });
    },
    hidePhoneNumberTextBox() {
      this.showPhoneNumberTextBox = false;
    },
    onCancelButtonClicked() {
      redirectTo(this, this.appointmentBookingPath);
    },
    async onConfirmButtonClicked() {
      this.reasonError = false;
      this.telephoneNumberError = false;
      this.$nextTick(async () => {
        if (this.slot.channel === channel.Telephone) {
          this.telephoneNumber = this.telephoneNumber.trim();
          if ((this.telephoneNumber.length === 0)
            && (this.otherTelephoneNumber.trim().length === 0)) {
            this.telephoneNumberError = true;
          }
        }

        const isMandatory = this.$store.state.availableAppointments
          .bookingReasonNecessity === necessity.Mandatory;
        this.symptoms = this.symptoms.trim();
        if (this.symptoms.length === 0 && isMandatory) {
          this.reasonError = true;
        }

        if (this.showError) {
          EventBus.$emit(FOCUS_ERROR_ELEMENT);
          return;
        }

        await this.confirmTheAppointmentSlot(this.slot, this.symptoms,
          this.telephoneNumber, this.otherTelephoneNumber.trim());

        if (this.error) {
          return;
        }
        this.$store.dispatch('analytics/trackUserProperty', {
          key: 'gpBookingSlot',
          value: moment(this.slot.startTime).format('dddd | HH:mm:ss'),
        });
        redirectTo(this, APPOINTMENT_BOOKING_SUCCESS_PATH);
      });
    },
    otherPhoneNumberSelected() {
      this.telephoneNumber = '';
      this.showPhoneNumberTextBox = true;
    },
    selected(event) {
      if (event.currentTarget.id === 'otherPhoneNumberRadioInput') {
        this.otherPhoneNumberSelected();
      } else {
        this.telephoneNumber = event.currentTarget.id;
        this.hidePhoneNumberTextBox();
      }
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import "@/style/custom/inline-block-a";
</style>
