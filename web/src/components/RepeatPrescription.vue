<template>
  <div data-purpose="repeat-prescription">
    <input :value="JSON.stringify(nojsData)" :name="prescriptionDetails.id" type="hidden">
    <generic-checkbox-no-js v-model="selected"
                            :checkbox-id="prescriptionDetails.id"
                            :selected="selected"
                            name="prescription"
                            @click="check">
      <span data-label="prescription-name">
        {{ prescriptionDetails.name }}
      </span>
      <p :class="$style.prescriptionDescription" data-label="prescription-description">
        {{ prescriptionDetails.details }}
      </p>
    </generic-checkbox-no-js>
  </div>
</template>
<script>
import { mapGetters } from 'vuex';
import GenericCheckboxNoJs from '@/components/widgets/GenericCheckboxNoJs';

export default {
  name: 'RepeatPrescription',
  components: {
    GenericCheckboxNoJs,
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
    nojsData() {
      return {
        name: this.prescriptionDetails.name,
        details: this.prescriptionDetails.details,
      };
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
    &>span {
      font-weight: normal;
      font-size: 1em;
    }
    padding-bottom: 0.250em;
  }
</style>
