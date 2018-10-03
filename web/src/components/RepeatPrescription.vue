<template>
  <div data-purpose="repeat-prescription">
    <generic-checkbox :checkbox-id="prescriptionDetails.id"
                      :selected="selected"
                      v-model="selected"
                      name="prescription"
                      @click="check">

      <label :for="'prescription-' + prescriptionDetails.id">
        <span data-label="prescription-name">
          {{ prescriptionDetails.name }}
        </span>
        <p :class="$style.prescriptionDescription" data-label="prescription-description">
          {{ prescriptionDetails.details }}
        </p>
      </label>

    </generic-checkbox>
  </div>
</template>
<script>
import { mapGetters } from 'vuex';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';

export default{
  name: 'RepeatPrescription',
  components: {
    GenericCheckbox,
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
  @import "../style/accessibility";

  .prescriptionDescription {
    padding-top: 0.250em;
    padding-bottom: 0.250em;
  }

  label {
    padding-bottom: 0.250em;
  }
</style>
