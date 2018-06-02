<template>
  <main class="main">
    <error-warning-dialog v-if="showValidationError" error-or-warning="error">
      <p>
        {{ $t('appointments.confirmation.noReasonDialogError') }}
      </p>
    </error-warning-dialog>

    <appointment-slot :slot-id="slotId" :always-deselect="true" aria-label="selected appointment" />
    <div class="form" role="form">
      <label for="reasonText">{{ $t('appointments.confirmation.headerLabel') }}</label>
      <p>{{ $t('appointments.confirmation.label') }}</p>

      <error-message v-if="showValidationError" id="errorLabel">
        {{ $t('appointments.confirmation.noReasonError') }}
      </error-message>
      <textarea
        id="reasonText"
        ref="reason"
        v-model="symptoms"
        :class="{error:showValidationError}"
        :aria-labelledby="reasonBoxAriaLabelledBy"
        maxlength="150"
      />
      <p id="maxReasonDesc">{{ $t('appointments.confirmation.maxReasonDesc') }}</p>
    </div>

    <button id="btn_book_appointment" class="button green" @click="onConfirmButtonClicked">
      {{ $t('appointments.confirmation.confirmButtonText') }}
    </button>
    <button id="btn_cancel_appointment" class="button grey" @click="onCancelButtonClicked">
      {{ $t('appointments.confirmation.changeButtonText') }}
    </button>
  </main>
</template>

<script>
/* eslint-disable import/extensions */
import AppointmentSlot from '@/components/AppointmentSlot';
import ErrorMessage from '@/components/ErrorMessage';
import ErrorWarningDialog from '@/components/errors/ErrorWarningDialog';

export default {
  middleware: ['auth', 'meta'],
  components: {
    ErrorWarningDialog,
    AppointmentSlot,
    ErrorMessage,
  },
  data() {
    return {
      slotId: null,
      symptoms: '',
      showValidationError: false,
    };
  },
  computed: {
    reasonBoxAriaLabelledBy() {
      return this.showValidationError ? 'errorLabel maxReasonDesc' : 'maxReasonDesc';
    },
  },
  mounted() {
    this.slotId = this.$store.state.appointmentSlots.selectedSlotId;
  },
  methods: {
    onConfirmButtonClicked() {
      this.symptoms = this.symptoms.trim();
      if (this.symptoms.length === 0) {
        this.showValidationError = true;
        this.$refs.reason.focus();
        return;
      }
      this.showValidationError = false;
      this.$router.push('/appointments/booked');
    },
    onCancelButtonClicked() {
      this.$router.push('/appointments');
    },
  },
};
</script>

<style lang="scss">
  @import "../../style/textstyles";
  @import "../../style/spacings";
  @import "../../style/colours";
  @import "../../style/buttons";

  .main {
    @include space(padding, all, $three);

    .form {
      margin-bottom: 24px;
      label {
        @include default_label;
        padding-top: 16px;
        padding-bottom: 8px;
      }

      p {
        @include default_text;
        padding-bottom: 8px;
      }

      textarea {
        width: 100%;
        min-height: 100px;
        box-sizing: border-box;
        background-color: $white;
        border-radius: 5px;
        padding: 16px;
        border: 1px $light_grey solid;
        outline: none;
        transition: all ease 0.5s;
        resize: none;
        @include default_text;

        &.error {
          border: 3px $error solid;
        }
      }
    }
  }

</style>
