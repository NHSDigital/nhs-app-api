<template>
  <div v-if="showTemplate">
    <no-js-form :action="confirmBookingPath" :value="{}" method="post">
      <input :value="confirmationMessageKey" type="hidden" name="confirmationMessageKey">
      <input :value="slotEndTime" type="hidden"
             name="nojs.availableAppointments.selectedSlot.endTime">
      <input :value="slotId" type="hidden" name="nojs.availableAppointments.selectedSlot.id">
      <input :value="slotStartTime" type="hidden"
             name="nojs.availableAppointments.selectedSlot.startTime">
      <input :value="true" type="hidden" name="isSubmitted">
      <div v-if="showError" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <message-dialog message-type="error" role="alert">
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
        </div>
      </div>

      <div class="nhsuk-grid-row" data-purpose="info">
        <div class="nhsuk-grid-column-full">
          <p class="nhsuk-u-padding-bottom-2">
            {{ $t('appointments.confirmation.info') }}
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

      <div v-if="showPhoneNumber()" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full">
          <div role="form" data-purpose="phone-number" :class="telephoneErrorStyle">
            <fieldset class="nhsuk-fieldset nhsuk-form-group--error">
              <legend id="telephone-input-label" class="nhsuk-fieldset__legend">
                <strong>{{ $t('appointments.confirmation.telephoneNumberLabel') }}</strong>
              </legend>
              <error-message v-if="showTelephoneError"
                             id="telephone-error-label">
                {{ $t('appointments.confirmation.noPhoneNumberError') }}
              </error-message>
              <div v-if="isJavascriptOn">
                <div v-for="(patientTelephoneNumber, index) in patientTelephoneNumbers"
                     :key="index" class="nhsuk-radios__item">
                  <input :id="patientTelephoneNumber.telephoneNumber"
                         v-model="telephoneNumber"
                         :value="patientTelephoneNumber.telephoneNumber"
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
                    {{ $t('appointments.confirmation.useOtherPhoneNumberLabel') }}
                  </label>
                </div>
              </div>
              <div v-if="showPhoneNumberTextBox">
                <p id="telephone-number-desc" class="nhsuk-u-padding-bottom-2">
                  {{ $t('appointments.confirmation.telephoneNumberDescription') }}
                </p>
                <generic-text-input id="telephoneNumberText"
                                    ref="telephone"
                                    v-model="otherTelephoneNumber"
                                    :a-labelled-by="telephoneNumberTextAriaLabelledBy"
                                    :text-area-classes="defaultClasses"
                                    :required="true"
                                    :class="showReasonError"
                                    name="telephoneNumber"
                                    pattern=".*[^ ].*"
                                    type="tel"/>
              </div>
            </fieldset>
          </div>
        </div>
      </div>

      <div v-if="showBookingReason()" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full nhsuk-u-padding-top-3">
          <div role="form" data-purpose="booking-reason" :class="reasonTextErrorStyle">
            <label id="booking-reason-label" class="nhsuk-fieldset__legend"
                   for="reasonText">
              <strong>
                {{ $t('appointments.confirmation.headerLabel') }}
                {{ bookingReasonOptional() ?
                  $t('appointments.confirmation.headerLabelSuffix') : '' }}
              </strong>
            </label>
            <p id="max-reason-desc">
              {{ $t('appointments.confirmation.reasonDesc.line1') }}
            </p>
            <p>
              {{ $t('appointments.confirmation.reasonDesc.line2') }}
              {{ $t('appointments.confirmation.reasonDesc.line3') }}
            </p>
            <error-message v-if="showReasonError" id="reason-error-label">
              {{ $t('appointments.confirmation.noReasonError') }}
            </error-message>
            <generic-text-area id="reasonText"
                               ref="reason"
                               v-model="symptoms"
                               :a-labelled-by="reasonBoxAriaLabelledBy"
                               :text-area-classes="defaultClasses"
                               :required="reasonRequired"
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
            {{ $t('appointments.confirmation.confirmButtonText') }}
          </generic-button>
        </div>
      </div>
    </no-js-form>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <desktopGenericBackLink v-if="!$store.state.device.isNativeApp"
                                :path="appointmentBookingPath"
                                :button-text="'appointments.confirmation.backButtonText'"
                                @clickAndPrevent="onCancelButtonClicked"/>
      </div>
    </div>
  </div>
</template>

<script>
import get from 'lodash/fp/get';
import moment from 'moment';
import channel from '@/lib/channel';
import necessity from '@/lib/necessity';
import AppointmentSlot from '@/components/appointments/Appointment';
import Card from '@/components/widgets/card/Card';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import NoJsForm from '@/components/no-js/NoJsForm';
import { createUri } from '@/lib/noJs';
import { getMessage } from '@/lib/errors';
import { redirectTo } from '@/lib/utils';
import {
  APPOINTMENTS,
  APPOINTMENT_BOOKING,
  APPOINTMENT_CONFIRMATIONS,
  APPOINTMENT_BOOKING_SUCCESS,
} from '@/lib/routes';

export default {
  layout: 'nhsuk-layout',

  components: {
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
    NoJsForm,
  },
  data() {
    return {
      appointmentBookingPath: APPOINTMENT_BOOKING.path,
      isJavascriptOn: false,
      otherTelephoneNumber: '',
      patientTelephoneNumbers: get('availableAppointments.patientTelephoneNumbers')(this.$store.state),
      reasonError: false,
      showPhoneNumberTextBox: true,
      slot: this.$store.state.availableAppointments.selectedSlot,
      symptoms: '',
      telephoneNumber: '',
      telephoneNumberError: false,
    };
  },
  computed: {
    appointmentPath() {
      return APPOINTMENTS.path;
    },
    cancelBookingPath() {
      return APPOINTMENT_BOOKING.path;
    },
    confirmBookingPath() {
      return APPOINTMENT_CONFIRMATIONS.path;
    },
    confirmationMessage() {
      return this.$t(this.confirmationMessageKey);
    },
    confirmationMessageKey() {
      return this.$store.state.myAppointments.disableCancellation
        ? 'appointments.index.successAndCancellationDisabledText'
        : 'appointments.index.successText';
    },
    defaultClasses() {
      return this.showError ? undefined : undefined;
    },
    formData() {
      return {
        myAppointments: {
          disableCancellation: this.$store.state.myAppointments.disableCancellation,
        },
      };
    },
    noJsTelephoneNumber() {
      if (!this.telephoneNumber) return undefined;
      return this.telephoneNumber;
    },
    reasonBoxAriaLabelledBy() {
      return this.showError ? 'booking-reason-label error-label max-reason-desc' : 'booking-reason-label max-reason-desc';
    },
    reasonRequired() {
      return !this.bookingReasonOptional();
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
  },
  watch: {
    symptoms(val, oldValue) {
      if (val.length > 150) {
        this.symptoms = oldValue;
      }
    },
  },
  async fetch({ app, store, req, redirect }) {
    const requestBody = get('body', req);
    const isSubmitted = get('isSubmitted', requestBody);
    let uri;


    if (isSubmitted) {
      const appointmentBookRequest = {
        BookingReason: requestBody.bookingReason,
        EndTime: store.state.availableAppointments.selectedSlot.endTime,
        SlotId: store.state.availableAppointments.selectedSlot.id,
        StartTime: store.state.availableAppointments.selectedSlot.startTime,
        TelephoneNumber: requestBody.telephoneNumber,
      };
      await store.dispatch('availableAppointments/book', appointmentBookRequest);

      if (!store.getters['errors/showApiError']) {
        uri = createUri({
          path: APPOINTMENTS.path,
          noJs: {
            flashMessage: {
              show: true,
              key: requestBody.confirmationMessageKey,
            },
          },
        });
        redirect(uri);
      } else {
        store.dispatch('header/updateHeaderText',
          getMessage({ $store: store, $i18n: app.i18n }, 'pageHeader'));
        store.dispatch('pageTitle/updatePageTitle',
          getMessage({ $store: store, $i18n: app.i18n }, 'pageTitle'));
      }
    }
  },
  mounted() {
    if (!this.slot) {
      redirectTo(this, APPOINTMENT_BOOKING.path);
    }
  },
  created() {
    if (process.client) {
      this.isJavascriptOn = true;
      this.showPhoneNumberTextBox =
        (this.$store.state.availableAppointments.patientTelephoneNumbers.length <= 0);
    }
  },
  beforeDestroy() {
    this.$store.dispatch('availableAppointments/deselect');
  },
  methods: {
    bookingReasonOptional() {
      return this.$store.state.availableAppointments
        .bookingReasonNecessity === necessity.Optional;
    },
    async confirmTheAppointmentSlot(slot, reason, telephoneNumberField, otherTelephoneNumberField) {
      if (!slot) {
        throw new ErrorMessage('Slot should not be null');
      }
      const bookingData = {
        SlotId: slot.id,
        BookingReason: reason,
        StartTime: slot.startTime,
        EndTime: slot.endTime,
        TelephoneNumber: (telephoneNumberField !== null && telephoneNumberField !== '')
          ? telephoneNumberField : otherTelephoneNumberField,
      };
      await this.$store.dispatch('availableAppointments/book', bookingData);
    },
    hidePhoneNumberTextBox() {
      this.showPhoneNumberTextBox = false;
    },
    onCancelButtonClicked() {
      redirectTo(this, this.cancelBookingPath);
    },
    async onConfirmButtonClicked(e) {
      e.preventDefault();
      this.reasonError = false;
      this.telephoneNumberError = false;

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
        window.scrollTo(0, 0);
        return;
      }

      try {
        await this.confirmTheAppointmentSlot(this.slot, this.symptoms,
          this.telephoneNumber, this.otherTelephoneNumber.trim());
        if (process.client) {
          this.$store.dispatch('analytics/trackUserProperty', {
            key: 'gpBookingSlot',
            value: moment(this.slot.startTime).format('dddd | HH:mm:ss'),
          });
        }
        let successPath;
        if (this.$store.getters['session/isProxying']) {
          successPath = APPOINTMENT_BOOKING_SUCCESS.path;
        } else {
          this.$store.dispatch('flashMessage/addSuccess', this.confirmationMessage);
          successPath = this.appointmentPath;
        }
        redirectTo(this, successPath);
      } catch (error) {
        /*
        empty catch block as the
        ApiError.vue (component) handles and
        surfaces appropriate error content based on the http status code returned from the API
        */
      }
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
    showBookingReason() {
      return this.$store.state.availableAppointments
        .bookingReasonNecessity !== necessity.NotAllowed;
    },
    showPhoneNumber() {
      return (this.slot || {}).channel === channel.Telephone;
    },
  },
};
</script>
