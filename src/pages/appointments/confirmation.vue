<template>
  <main class="main">
    <error-warning-dialog errorOrWarning="error" v-if="showValidationError">
      <p>
        {{$t('appointments.confirmation.noReasonDialogError')}}
      </p>
    </error-warning-dialog>

    <appointment-slot :slotId="slotId" :alwaysDeselect="true" aria-label="selected appointment" />
    <div class="form" role="form">
      <label for="reasonText">{{$t('appointments.confirmation.headerLabel')}}</label>
      <p>{{$t('appointments.confirmation.label')}}</p>

      <error-message v-if="showValidationError" id="errorLabel">
        {{$t('appointments.confirmation.noReasonError')}}
      </error-message>
      <textarea maxlength="150" v-model="symptoms" :class="{error:showValidationError}" id="reasonText" ref="reason" :aria-labelledby="reasonBoxAriaLabelledBy"
      />
      <p id="maxReasonDesc">{{$t('appointments.confirmation.maxReasonDesc')}}</p>
    </div>

    <button class="button green" id="btn_book_appointment" @click="onConfirmButtonClicked">
      {{$t('appointments.confirmation.confirmButtonText')}}
    </button>
    <button class="button grey" id="btn_cancel_appointment" @click="onCancelButtonClicked">
      {{$t('appointments.confirmation.changeButtonText')}}
    </button>
  </main>
</template>

<script>
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
  mounted() {
    this.slotId = this.$store.state.appointmentSlots.selectedSlotId;
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