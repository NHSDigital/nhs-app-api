<template>
  <div v-if="showTemplate" class="pull-content">
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

        <div :class="[$style.info, isDesktopWeb ? $style.desktopWeb : $style.web]"
             data-purpose="info">
          <p :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
            {{ $t('appointments.confirmation.info') }}</p>
        </div>

        <appointment-slot v-if="slot" :appointment="slot" :show-cancellation-link="false"
                          aria-label="selected appointment" />
        <div v-if="showPhoneNumber()" :class="[$style.form, $style.phoneNumberForm]"
             role="form" data-purpose="phone-number">
          <fieldset :class="$style.fieldsetTelephoneNumberRadio">
            <legend :class="$style.textPhoneNumberLabel">
              {{ $t('appointments.confirmation.telephoneNumberLabel') }}
            </legend>
            <error-message v-if="showTelephoneError"
                           id="telephone-error-label"
                           :class="isDesktopWeb ? $style.desktopWebError : undefined">
              {{ $t('appointments.confirmation.noPhoneNumberError') }}
            </error-message>
            <div v-if="isJavascriptOn">
              <div v-for="(patientTelephoneNumber, index) in patientTelephoneNumbers"
                   :key="index">
                <label :id="patientTelephoneNumber.telephoneNumber"
                       :class="[$style.telephoneNumberContainer,
                                isDesktopWeb ? $style.desktopWeb : $style.web]"
                       @keypress="onKeyDown" @click="selected">
                  <input v-model="telephoneNumber"
                         :id="'telephoneNumber' + index"
                         :value="patientTelephoneNumber.telephoneNumber"
                         :class="$style.customRadio"
                         type="radio"
                         name="radio"
                         @change="selected;hidePhoneNumberTextBox();">
                  <span :class="$style.radioButton"
                        :selected="isSelected"
                        :id="patientTelephoneNumber.telephoneNumber"/>
                  <span :class="[$style.patientTelephoneNumberLabel,
                                 isDesktopWeb ? $style.desktopWeb : $style.web]">
                    {{ patientTelephoneNumber.telephoneNumber }}</span>
                </label>
              </div>
              <label v-if="patientTelephoneNumbers.length > 0"
                     id="otherPhoneNumberRadio"
                     :class="[$style.telephoneNumberContainer,
                              isDesktopWeb ? $style.desktopWeb : $style.web]"
                     @keypress="onKeyDown">
                <input id="otherPhoneNumberRadioInput"
                       :checked="model === 'otherPhoneNumber'"
                       :class="$style.customRadio"
                       type="radio"
                       name="radio"
                       @change="otherPhoneNumberSelected()">
                <span id="otherPhoneNumberRadioButton"
                      :class="$style.radioButton"
                      :selected="isSelected"/>
                <span :class="[$style.patientTelephoneNumberLabel,
                               isDesktopWeb ? $style.desktopWeb : $style.web]">
                  Use other phone number</span>
              </label>
            </div>
            <div v-if="showPhoneNumberTextBox">
              <generic-text-input id="telephoneNumberText"
                                  ref="telephone"
                                  :text-area-classes="defaultClasses"
                                  :required="true"
                                  :initial-contents="otherTelephoneNumber"
                                  :class="showReasonError ?
                                  $style.desktopWebErrorBorder : undefined"
                                  v-model="otherTelephoneNumber"
                                  name="telephoneNumberField"
                                  pattern=".*[^ ].*"
                                  type="tel"/>
              <p :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
                {{ $t('appointments.confirmation.telephoneNumberDescription') }}
              </p>
            </div>
          </fieldset>
        </div>
        <div v-if="showBookingReason()" :class="[$style.form, $style.reasonForm]"
             role="form" data-purpose="booking-reason">
          <label :class="[$style.textReasonLabel, isDesktopWeb ? $style.desktopWeb : $style.web]"
                 for="reasonText">
            {{ $t('appointments.confirmation.headerLabel') }}
            {{ bookingReasonOptional() ? $t('appointments.confirmation.headerLabelSuffix') : '' }}
          </label>

          <error-message v-if="showReasonError" id="reason-error-label">
            {{ $t('appointments.confirmation.noReasonError') }}
          </error-message>
          <generic-text-area id="reasonText"
                             ref="reason"
                             :a-labelled-by="reasonBoxAriaLabelledBy"
                             :initial-contents="symptoms"
                             :text-area-classes="defaultClasses"
                             :required="true"
                             :class="showReasonError ? $style.desktopWebErrorBorder : undefined"
                             v-model="symptoms"
                             name="bookingReason"
                             maxlength="150"/>

          <p id="max-reason-desc"
             :class="[$style.char, isDesktopWeb ? $style.desktopWeb : $style.web]">
            {{ $t('appointments.confirmation.reasonDesc.line1') }}
          </p>
          <p :class="[isDesktopWeb ? $style.desktopWeb : $style.web]">
            {{ $t('appointments.confirmation.reasonDesc.line2') }}
            <br >
            {{ $t('appointments.confirmation.reasonDesc.line3') }}
          </p>
        </div>

        <generic-button id="btn_book_appointment"
                        :class="[$style.button, $style.green,
                                 isDesktopWeb ? $style.desktopWebConfirmButton : $style.web]"
                        @click.prevent="onConfirmButtonClicked">
          {{ $t('appointments.confirmation.confirmButtonText') }}
        </generic-button>
      </form-post>

      <generic-button v-if="$store.state.device.isNativeApp" id="btn_cancel_appointment"
                      :class="[$style.button , $style.grey]"
                      @click.stop.prevent="onCancelButtonClicked">
        {{ $t('appointments.confirmation.changeButtonText') }}
      </generic-button>

      <desktopGenericBackLink
        v-if="!$store.state.device.isNativeApp"
        :path="appointmentBookingPath"
        :button-text="'appointments.confirmation.backButtonText'"/>
    </div>
  </div>
</template>


<script>
import get from 'lodash/fp/get';
import AppointmentSlot from '@/components/appointments/Appointment';
import ErrorMessage from '@/components/widgets/ErrorMessage';
import MessageDialog from '@/components/widgets/MessageDialog';
import MessageText from '@/components/widgets/MessageText';
import MessageList from '@/components/widgets/MessageList';
import GenericTextArea from '@/components/widgets/GenericTextArea';
import GenericButton from '@/components/widgets/GenericButton';
import GenericTextInput from '@/components/widgets/GenericTextInput';
import { APPOINTMENT_BOOK_NOJS, APPOINTMENT_BOOKING, APPOINTMENTS } from '@/lib/routes';
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
  props: {
    model: {
      type: String,
      default: undefined,
    },
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
      isDesktopWeb: (this.$store.state.device.source !== 'android'
        && this.$store.state.device.source !== 'ios'),
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
      return this.reasonError || this.telephoneNumberError;
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
    isSelected() {
      return this.model === this.telephoneNumber;
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
      this.$router.push(APPOINTMENT_BOOKING.path);
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
      this.model = 'otherPhoneNumber';
      this.showPhoneNumberTextBox = true;
    },
    hidePhoneNumberTextBox() {
      this.showPhoneNumberTextBox = false;
    },
    selected(event) {
      if (event.currentTarget.id === 'otherPhoneNumberRadio') {
        this.otherPhoneNumberSelected();
      } else {
        this.model = '';
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
          this.showError = true;
        }
      }

      const isMandatory = this.$store.state.availableAppointments
        .bookingReasonNecessity === Necessity.Mandatory;
      this.symptoms = this.symptoms.trim();
      if (this.symptoms.length === 0 && isMandatory) {
        this.reasonError = true;
        this.showError = true;
      }

      if (this.showError) {
        window.scrollTo(0, 0);
        return;
      }

      this.confirmTheBook(
        this.slot, this.symptoms, this.telephoneNumber
        , this.otherTelephoneNumber.trim(),
      );
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
          this.$store.dispatch('flashMessage/addSuccess', this.confirmationMessage);
          this.$router.push(APPOINTMENTS.path);
        });
    },
    onCancelButtonClicked() {
      this.$router.push(this.cancelBookingPath);
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/buttons";
@import "../../style/forms";
@import "../../style/info";

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
 padding-top: 1em;

 &.desktopWeb {
  font-family: $default-web;
  font-weight: lighter;
  margin-top: 1em;
 }
 &.web{
  padding-top: 8px;
  font-weight: bold;
  padding-bottom: 1em;
 }
}

.textPhoneNumberLabel {
 padding-top: 2em;

 &.desktopWeb {
  font-family: $default-web;
  font-weight: lighter;
 }

 &.web {
  padding-top: 8px;
  font-weight: bold;
  padding-bottom: 1em;
 }
}

.patientPhoneNumberRadioButton {
  float: left;
  margin-right: 1em;
}
.patientPhoneNumberRadioLabel {
  padding-top: 0em;
}

.patientTelephoneNumberLabel {
  margin-left: 0.5em;
 &.desktopWeb{
  font-family: $default-web;
  font-weight: lighter;
 }
}
/* The telephoneNumberContainer */
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
 &.web {
  margin-bottom: 12px;
 }
}

/* Hide the browser's default radio button */
.telephoneNumberContainer input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
}

/* Create a custom radio button */
.radioButton {
  position: absolute;
  top: 0;
  left: 0;
  height: 2.063em;;
  width: 2.063em;;
  background-color: $white;
  border-radius: 50%;
  border-color: black;
  border-style: solid;
  border-width: 2px;
}

/* On mouse-over, add a white background color */
.telephoneNumberContainer:hover input ~ .radioButton {
  background-color: white;
}

/* When the radio button is checked, add a white background */
.telephoneNumberContainer input:checked ~ .radioButton {
  background-color: white;
}

/* Create the indicator (the dot/circle - hidden when not checked) */
.radioButton:after {
  content: "";
  position: absolute;
  display: none;
}

/* Show the indicator (dot/circle) when checked */
.telephoneNumberContainer input:checked ~ .radioButton:after {
  display: block;
}

/* Style the indicator (dot/circle) */
.telephoneNumberContainer .radioButton:after {
  top: 0.3em;
  left: 0.3em;
  width: 1.2em;
  height: 1.2em;
  border-radius: 50%;
  background: black;
}
.fieldsetTelephoneNumberRadio {
  border: 0;
}

.customRadio:focus + .radioButton {
  box-shadow: 0 0 4px 1px #ffb81c;
  border-color: white;
}

.telephoneNumberContainer .customRadio:focus {
  box-shadow: none;
  border-color: black;
}

.button:focus{
 outline-color: $focus_highlight;
 box-shadow: inset 0 0 0 4px $focus_highlight;
}

.button.green:focus{
 outline-color: $focus_highlight;
 box-shadow: inset 0 0 0 4px $focus_highlight;
}


 .desktopWebError{
  font-family: $default-web;
  font-weight: normal;
 }
.desktopWebError{
 font-family: $default-web;
 font-weight: normal;
}

 .desktopWebErrorBorder{
  box-sizing: content-box;
  outline-color: red;
  box-shadow: 0 0 0 3px red;
  border-radius: 0.313em;
  outline-width: thick;
 }

</style>

