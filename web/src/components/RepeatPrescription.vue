<template>
  <div data-purpose="repeat-prescription">
    <div :class="$style['checkbox-panel']">
      <div class="clickme" @click="check">
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
        :for="'prescription-' + prescriptionDetails.id"
        @click="check">
        {{ prescriptionDetails.name }}
      </label>
    </div>
    <p
      :class="$style.prescriptionDescription"
      data-label="prescription-description">
      {{ prescriptionDetails.details }}
    </p>
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

<style module lang="scss" scoped>
  @import "../style/forms";

  .prescriptionDescription {
    padding-left: 2.5em;
    padding-top: 0em;
    padding-bottom: 0.250em;
  }
  .prescriptionCheckbox {
    display: none;
  }

  label {
    padding-bottom: 0.250em;
  }
</style>
