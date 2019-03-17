<template>
  <div v-if="showTemplate" :class="[$style['pull-content'],
                                    !$store.state.device.isNativeApp && $style.desktopWeb]">
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

        <div :class="$style.info"
             data-purpose="info">
          <p>
            {{ $t('appointments.confirmation.info') }}</p>
        </div>

        <appointment-slot v-if="slot" :appointment="slot" :show-cancellation-link="false"
                          aria-label="selected appointment"
                          date-time-header="h2" />
        <div v-if="showPhoneNumber()" :class="[$style.form, $style.phoneNumberForm]"
             role="form" data-purpose="phone-number">
          <fieldset :class="$style.fieldsetTelephoneNumberRadio">
            <legend id="telephone-input-label" :class="$style.textPhoneNumberLabel">
              {{ $t('appointments.confirmation.telephoneNumberLabel') }}
            </legend>
            <error-message v-if="showTelephoneError"
                           id="telephone-error-label">
              {{ $t('appointments.confirmation.noPhoneNumberError') }}
            </error-message>
            <div v-if="isJavascriptOn">
              <div v-for="(patientTelephoneNumber, index) in patientTelephoneNumbers"
                   :key="index" :class="$style.customRadioItem">
                <input :id="patientTelephoneNumber.telephoneNumber"
                       v-model="telephoneNumber"
                       :value="patientTelephoneNumber.telephoneNumber"
                       :class="$style.customRadio"
                       type="radio"
                       name="radio"
                       @change="selected">
                <label :for="patientTelephoneNumber.telephoneNumber"
                       :class="$style.customRadioLabel"
                       @keypress="onKeyDown" @click="selected">
                  {{ patientTelephoneNumber.telephoneNumber }}
                </label>
              </div >
              <div v-if="patientTelephoneNumbers.length > 0" :class="$style.customRadioItem">
                <input :id="'otherPhoneNumberRadioInput'"
                       :class="$style.customRadio"
                       type="radio"
                       name="radio"
                       @change="selected">
                <label :for="'otherPhoneNumberRadioInput'"
                       :class="$style.customRadioLabel"
                       @keypress="onKeyDown" @click="selected">
                  Use other phone number
                </label>
              </div>
            </div>
            <div v-if="showPhoneNumberTextBox">
              <generic-text-input id="telephoneNumberText"
                                  ref="telephone"
                                  v-model="otherTelephoneNumber"
                                  :a-labelled-by="telephoneNumberTextAriaLabelledBy"
                                  :text-area-classes="defaultClasses"
                                  :required="true"
                                  :class="showReasonError && $style.desktopWebErrorBorder"
                                  name="telephoneNumberField"
                                  pattern=".*[^ ].*"
                                  type="tel"/>
              <p id="telephone-number-desc">
                {{ $t('appointments.confirmation.telephoneNumberDescription') }}
              </p>
            </div>
          </fieldset>
        </div>
        <div v-if="showBookingReason()" :class="[$style.form, $style.reasonForm]"
             role="form" data-purpose="booking-reason">
          <label :class="$style.textReasonLabel"
                 for="reasonText">
            {{ $t('appointments.confirmation.headerLabel') }}
            {{ bookingReasonOptional() ? $t('appointments.confirmation.headerLabelSuffix') : '' }}
          </label>

          <error-message v-if="showReasonError" id="reason-error-label">
            {{ $t('appointments.confirmation.noReasonError') }}
          </error-message>
          <generic-text-area id="reasonText"
                             ref="reason"
                             v-model="symptoms"
                             :a-labelled-by="reasonBoxAriaLabelledBy"
                             :text-area-classes="defaultClasses"
                             :required="true"
                             :error.sync="showReasonError"
                             name="bookingReason"
                             maxlength="150"/>

          <p id="max-reason-desc"
             :class="$style.char">
            {{ $t('appointments.confirmation.reasonDesc.line1') }}
          </p>
          <p>
            {{ $t('appointments.confirmation.reasonDesc.line2') }}
            <br >
            {{ $t('appointments.confirmation.reasonDesc.line3') }}
          </p>
        </div>
        <div :class="$style.confirmButton">
          <generic-button id="btn_book_appointment"
                          :button-classes="[$store.state.device.isNativeApp
                                              ?'button':'button-desktop',
                                            'green']"
                          click-delay="medium"
                          @click.prevent="onConfirmButtonClicked">
            {{ $t('appointments.confirmation.confirmButtonText') }}
          </generic-button>
        </div>
      </form-post>

      <generic-button v-if="$store.state.device.isNativeApp" id="btn_cancel_appointment"
                      :button-classes="['button' , 'grey']"
                      @click.stop.prevent="onCancelButtonClicked">
        {{ $t('appointments.confirmation.changeButtonText') }}
      </generic-button>

      <desktopGenericBackLink
        v-else
        :path="appointmentBookingPath"
        :button-text="'appointments.confirmation.backButtonText'"
        @clickAndPrevent="onCancelButtonClicked"/>
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

export default {
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
    confirmBookingPath() {
      return APPOINTMENT_BOOK_NOJS.path;
    },
    reasonBoxAriaLabelledBy() {
      return this.showError ? 'error-label max-reason-desc' : 'max-reason-desc';
    },
    telephoneNumberTextAriaLabelledBy() {
      return this.showTelephoneError ? 'telephone-error-label telephone-number-desc' : 'telephone-number-desc';
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
      return this.showReasonError || this.showTelephoneError;
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
      redirectTo(this, APPOINTMENT_BOOKING.path, null);
    }

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
    onConfirmButtonClicked(e) {
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

      this.confirmTheBook(
        this.slot, this.symptoms, this.telephoneNumber,
        this.otherTelephoneNumber.trim(),
      );
      this.$store.dispatch('availableAppointments/clear');
    },
    confirmTheBook(slot, reason, telephoneNumberField, otherTelephoneNumberField) {
      if (!slot) return;

      const bookingData = {
        SlotId: slot.id,
        BookingReason: reason,
        StartTime: slot.startTime,
        EndTime: slot.endTime,
        TelephoneNumber: (telephoneNumberField !== null && telephoneNumberField !== '')
          ? telephoneNumberField : otherTelephoneNumberField,
      };
      this.$store.dispatch('availableAppointments/book', bookingData)
        .then(() => {
          if (process.client) {
            this.$store.dispatch('analytics/trackUserProperty', { key: 'gpBookingSlot', value: moment(slot.startTime).format('dddd | HH:mm:ss') });
          }
          this.$store.dispatch('flashMessage/addSuccess', this.confirmationMessage);
          redirectTo(this, APPOINTMENTS.path, null);
        });
    },
    onCancelButtonClicked() {
      redirectTo(this, this.cancelBookingPath, null);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/forms";
@import "../../style/info";
@import "../../style/desktopWeb/inputcontrol";

.reasonForm {
 &.web {
  margin-bottom: 24px;
 }
}

.phoneNumberForm {
 &.web {
  margin-bottom: 6px;
 }
 &.desktopWeb{
  margin-top: 1em;
}
}

.textReasonLabel {
  padding-top: 8px;
  font-weight: bold;
  padding-bottom: 1em;
  margin-top: 1em;

}

.textPhoneNumberLabel {
  padding-top: 8px;
  font-weight: bold;
  padding: 1em 0 0.5em 0;
  margin-top: 1em;
}

.patientPhoneNumberRadioButton {
  float: left;
  margin-right: 1em;
}
.patientPhoneNumberRadioLabel {
  padding-top: 0;
}

.patientTelephoneNumberLabel {
  margin-left: 0.5em;
}

.telephoneNumberContainer {
  display: block;
  position: relative;
  padding-left: 35px;

  cursor: pointer;
  font-size: 1em;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
  margin-bottom: 12px;
}

.fieldsetTelephoneNumberRadio {
  border: 0;
}

.customRadio {
    cursor: pointer;
    height: 32px;
    left: 0;
    margin: 0;
    opacity: 0;
    position: absolute;
    top: 0;
    width: 32px;
    z-index: 1;
}

.customRadioLabel {
    -ms-touch-action: manipulation;
    cursor: pointer;
    display: inline-block;
    margin-bottom: 0;
    padding: 8px 12px 4px;
    touch-action: manipulation;
}
.customRadio + .customRadioLabel::before {
    background: $white;
    border: 2px #425563 solid;
    border-radius: 50%;
    box-sizing: border-box;
    content: '';
    height: 32px;
    left: 0;
    position: absolute;
    top: 0;
    width: 32px;
}
.customRadio + .customRadioLabel::after {
    background: black;
    border: 8px solid #212b32;
    border-radius: 50%;
    content: '';
    height: 0;
    left: 8px;
    opacity: 0;
    position: absolute;
    top: 8px;
    width: 0;
}
.customRadio:checked + .customRadioLabel::after {
    opacity: 1;
}

.customRadio:focus + .customRadioLabel::before {
  box-shadow: 0 0 0 4px #ffb81C;
  outline: 4px solid transparent;
  outline-offset: 4px;
}

.customRadioItem {
    display: block;
    position: relative;
    min-height: 32px;
    margin-bottom: 8px;
    padding: 0 0 0 32px;
    clear: left;
}

.confirmButton {
  margin-top: 1em;
 }

.errorBorder {
  max-width: 540px;
}

div {
 &.desktopWeb {
  p {
   font-family: $default-web;
   font-weight: lighter;
   max-width: 540px;
  }
  .info {
   font-size: 1em;
   margin-bottom: 1em;
  }
  .telephoneNumberContainer {
   margin-bottom: 0px;
  }
  .patientTelephoneNumberLabel {
    font-family: $default-web;
    font-weight: lighter;
  }
  .textReasonLabel {
   padding-top: 1em;
    font-family: $default-web;
    font-weight: lighter;
    margin-top: 1em;
  }
 }
}

</style>
