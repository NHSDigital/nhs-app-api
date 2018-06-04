<template>
  <div>
    <div :class="$style.checkboxPanel" @click="check">
      <checked-icon :selected="prescriptionDetails.selected" :id="prescriptionDetails.id" />
      <input
        type="hidden"
        @click="check">
      <label :class="$style.checkboxLabel">{{ prescriptionDetails.name }}</label>
    </div>
    <label
      :class="$style.prescriptionDescription">
      {{ prescriptionDetails.dosage }} - {{ prescriptionDetails.quantity }}
    </label>
  </div>
</template>
<script>
/* eslint-disable import/extensions */
import { mapGetters } from 'vuex';
import CheckedIcon from '../components/icons/CheckedIcon';

export default{
  name: 'RepeatPrescription',
  components: {
    CheckedIcon,
  },
  props: {
    prescriptionDetails: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      checked: false,
    };
  },
  computed: {
    ...mapGetters({
      isValid: 'repeatPrescriptionCourses/isValid',
    }),
  },
  methods: {
    check() {
      this.$store.dispatch('repeatPrescriptionCourses/select', this.prescriptionDetails.id);
      this.$store.dispatch('repeatPrescriptionCourses/validate', { isValid: this.isValid });
    },
  },
};
</script>

<style module lang="scss">
  @import "../style/fonts";
  .checkboxPanel {
    display: flex;
    margin-bottom: 16px;
  }

  .checkboxLabel {
    display: block!important;
    font-weight: 700!important;
    font-size: 16px!important;
    line-height: 22px!important;
    color: #4A4A4A!important;
    padding-top: 4px!important;
    font-family: $frutiger-bold;
  }
  .prescriptionDescription {
    display: block;
    font-weight: normal;
    font-size: 16px;
    line-height: 22px;
    color: #4A4A4A;
    padding-left: 42px;
    margin-bottom: 16px;
  }
</style>
