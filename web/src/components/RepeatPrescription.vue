<template>
  <div data-purpose="repeat-prescription">
    <div :class="$style.checkboxPanel">
      <div @click="check">
        <checked-icon :selected="selected" :id="prescriptionDetails.id" />
      </div>
      <input
        :value="prescriptionDetails.id"
        :class="$style.prescriptionCheckbox"
        :id="'prescription-' + prescriptionDetails.id"
        :checked="check"
        v-model="selected"
        type="checkbox"
        name="prescription">
      <label
        :class="$style.checkboxLabel"
        :for="'prescription-' + prescriptionDetails.id"
        @click="check">
        {{ prescriptionDetails.name }}
      </label>
    </div>
    <span
      :class="$style.prescriptionDescription"
      aria-label="prescription-description">
      {{ prescriptionDetails.details }}
    </span>
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
  computed: {
    ...mapGetters({
      isValid: 'repeatPrescriptionCourses/isValid',
    }),
    selected: {
      get() {
        return this.prescriptionDetails.selected;
      },
      set() {
        // not needed, computed value is for hidden input
      },
    },
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
  .prescriptionCheckbox {
    display: none;
  }
</style>
