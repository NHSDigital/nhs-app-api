<template>
  <div v-if="showTemplate">
    <form-post :action="confirmBookingPath">
      <input :value="confirmationMessageKey" type="hidden" name="successMessageKey">
      <input :value="slotEndTime" type="hidden" name="endTime">
      <input :value="slotId" type="hidden" name="slotId">
      <input :value="slotStartTime" type="hidden" name="startTime">

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
          <div role="form" data-purpose="phone-number">
            <fieldset class="nhsuk-fieldset nhsuk-form-group--error">
              <legend id="telephone-input-label" class="nhsuk-fieldset__legend">
                {{ $t('appointments.confirmation.telephoneNumberLabel') }}
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
                         @keypress="onKeyDown" @click="selected">
                    {{ patientTelephoneNumber.telephoneNumber }}
                  </label>
                </div>
                <div v-if="patientTelephoneNumbers.length > 0" class="nhsuk-radios__item">
                  <input :id="'otherPhoneNumberRadioInput'"
                         type="radio"
                         name="radio"
                         class="nhsuk-radios__input"
                         @change="selected">
                  <label :for="'otherPhoneNumberRadioInput'"
                         class="nhsuk-label nhsuk-radios__label"
                         @keypress="onKeyDown" @click="selected">
                    Use other phone number
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
                                    name="telephoneNumberField"
                                    pattern=".*[^ ].*"
                                    type="tel"/>
              </div>
            </fieldset>
          </div>
        </div>
      </div>

      <div v-if="showBookingReason()" class="nhsuk-grid-row">
        <div class="nhsuk-grid-column-full nhsuk-u-padding-top-3">
          <div role="form" data-purpose="booking-reason">
            <label id="booking-reason-label" class="nhsuk-fieldset__legend"
                   for="reasonText">
              {{ $t('appointments.confirmation.headerLabel') }}
              {{ bookingReasonOptional() ? $t('appointments.confirmation.headerLabelSuffix') : '' }}
            </label>
            <p id="max-reason-desc" class="nhsuk-u-padding-bottom-2">
              {{ $t('appointments.confirmation.reasonDesc.line1') }}
            </p>
            <p class="nhsuk-u-padding-bottom-2">
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
    </form-post>

    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <generic-button v-if="$store.state.device.isNativeApp" id="btn_cancel_appointment"
                        :button-classes="['nhsuk-button', 'nhsuk-button--secondary']"
                        @click.stop.prevent="onCancelButtonClicked">
          {{ $t('appointments.confirmation.changeButtonText') }}
        </generic-button>

        <desktopGenericBackLink v-else :path="appointmentBookingPath"
                                :button-text="'appointments.confirmation.backButtonText'"
                                @clickAndPrevent="onCancelButtonClicked"/>
      </div>
    </div>
  </div>
</template>


<script>
import { redirectTo } from '@/lib/utils';
import { APPOINTMENT_BOOK_NOJS, APPOINTMENT_BOOKING, APPOINTMENTS } from '@/lib/routes';
import moment from 'moment';
import get from 'lodash/fp/get';
import AppointmentSlot from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import Necessity from '@/lib/necessity';
import FormPost from '@/components/FormPost';
import channel from '@/lib/channel';
import DesktopGenericBackLink from '../../components/widgets/DesktopGenericBackLink';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';
import Card from '@/components/widgets/card/Card';

export default {
  layout: 'nhsuk-layout',

  components: {
    DesktopGenericBackLink,
    GenericTextInput,
    MessageDialog,
    MessageText,
    MessageList,
    AppointmentSlot,
    ErrorMessage,
    GenericTextArea,
    GenericButton,
    FormPost,
    Card,
    CardGroupItem,
    CardGroup,
  },
  data() {
    return {
      symptoms: '',
      slot: this.$store.state.availableAppointments.selectedSlot,
      reasonError: false,
      telephoneNumber: '',
      otherTelephoneNumber: '',
      telephoneNumberError: false,
      patientTelephoneNumbers: get('availableAppointments.patientTelephoneNumbers')(this.$store.state),
      showPhoneNumberTextBox: true,
      isJavascriptOn: false,
      appointmentBookingPath: APPOINTMENT_BOOKING.path,
    };
  },
  computed: {
    cancelBookingPath() {
      return APPOINTMENT_BOOKING.path;
    },
    appointmentPath() {
      return APPOINTMENTS.path;
    },
    confirmBookingPath() {
      return APPOINTMENT_BOOK_NOJS.path;
    },
    reasonBoxAriaLabelledBy() {
      return this.showError ? 'booking-reason-label error-label max-reason-desc' : 'booking-reason-label max-reason-desc';
    },
    telephoneNumberTextAriaLabelledBy() {
      return this.showTelephoneError ? 'telephone-error-label telephone-number-desc' : 'telephone-number-desc';
    },
    defaultClasses() {
      return this.showError ? undefined : undefined;
    },
    showReasonError() {
      return this.reasonError && !this.symptoms;
    },
    reasonRequired() {
      return !this.bookingReasonOptional();
    },
    showTelephoneError() {
      return this.telephoneNumberError && !this.telephoneNumber;
    },
    showError() {
      return this.showReasonError || this.showTelephoneError;
    },
    confirmationMessageKey() {
      return this.$store.state.myAppointments.disableCancellation
        ? 'appointments.index.successAndCancellationDisabledText'
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
      redirectTo(this, APPOINTMENT_BOOKING.path, null);
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
    otherPhoneNumberSelected() {
      this.telephoneNumber = '';
      this.showPhoneNumberTextBox = true;
    },
    hidePhoneNumberTextBox() {
      this.showPhoneNumberTextBox = false;
    },
    selected(event) {
      if (event.currentTarget.id === 'otherPhoneNumberRadioInput') {
        this.otherPhoneNumberSelected();
      } else {
        this.telephoneNumber = event.currentTarget.id;
        this.hidePhoneNumberTextBox();
      }
      event.stopPropagation();
    },
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.selected(e);
      }
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
        .bookingReasonNecessity === Necessity.Mandatory;
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
        this.$store.dispatch('flashMessage/addSuccess', this.confirmationMessage);
        redirectTo(this, this.appointmentPath, null);
      } catch (error) {
        /*
        empty catch block as the
        ApiError.vue (component) handles and
        surfaces appropriate error content based on the http status code returned from the API
        */
      }
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
    onCancelButtonClicked() {
      redirectTo(this, this.cancelBookingPath, null);
    },
  },
};
</script>
