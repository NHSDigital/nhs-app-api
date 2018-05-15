<template>
  <div id="mainDiv">
    <main class="main">
      <appointment-slot :slotId="slotId" :alwaysDeselect="true" />
      <div class="form">
        <label>{{$t('appointments.confirmation.headerLabel')}}</label>
        <p>{{$t('appointments.confirmation.label')}}</p>
        <textarea id="txt_reason"></textarea>
      </div>
      <button class="button grey" id="btn_cancel_appointment" @click="onCancelButtonClicked">
        {{$t('appointments.confirmation.changeButtonText')}}
      </button>
    </main>
  </div>
</template>

<script>
import AppointmentSlot from '@/components/appointments/AppointmentSlot';

export default {
  components: {
    AppointmentSlot,
  },
  data() {
    return {
      slotId: null,
    };
  },
  methods: {
    onCancelButtonClicked() {
      this.$router.go(-1);
    },
  },
  mounted() {
    this.slotId = this.$store.state.appointmentSlots.selectedSlotId;
  },
};
</script>

<style lang="scss">
  @import '../../style/textstyles';
  @import '../../style/spacings';
  @import '../../style/colours';
  @import '../../style/buttons';

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
        @include default_text;

        &.error {
          border: 1px $error solid;
          color: $error;
        }
      }
    }
  }

</style>
