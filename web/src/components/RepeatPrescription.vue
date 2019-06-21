<template>
  <div>
    <div v-for="repeatPrescription in repeatPrescriptionCourses" :key="repeatPrescription.id">
      <div data-purpose="repeat-prescription">
        <input :value="JSON.stringify({
                 name: repeatPrescription.name,
                 details: repeatPrescription.details,
               })"
               :name="repeatPrescription.id" type="hidden">
        <generic-checkbox
          :checkbox-id="repeatPrescription.id"
          :value="repeatPrescription.id"
          :is-selected="value"
          :required="false"
          name="prescription"
          @input="selectedValueChanged(repeatPrescription)">
          <span data-label="prescription-name"
                :aria-label="`${repeatPrescription.name}. ${repeatPrescription.details}`"
                role="text">
            {{ repeatPrescription.name }}
          </span>
          <p :class="$style.prescriptionDescription"
             data-label="prescription-description"
             aria-hidden="true">
            {{ repeatPrescription.details }}
          </p>
        </generic-checkbox>
      </div>
    </div>
  </div>
</template>
<script>
import { mapGetters } from 'vuex';
import GenericCheckbox from '@/components/widgets/GenericCheckbox';

export default {
  name: 'RepeatPrescription',
  components: {
    GenericCheckbox,
  },
  props: {
    value: {
      type: Array,
      default: () => [],
    },

  },
  computed: {
    ...mapGetters({
      isValid: 'repeatPrescriptionCourses/isValid',
    }),
    repeatPrescriptionCourses() {
      const { repeatPrescriptionCourses } = this.$store.state.repeatPrescriptionCourses;
      if (typeof repeatPrescriptionCourses === 'undefined' || !repeatPrescriptionCourses || repeatPrescriptionCourses.length === 0) {
        return null;
      }
      return repeatPrescriptionCourses;
    },
  },
  methods: {
    selectedValueChanged(checkbox) {
      this.$store.dispatch('repeatPrescriptionCourses/select', checkbox.id);
      this.$store.dispatch('repeatPrescriptionCourses/validate', { isValid: this.isValid });
      const storeSelected = this.$store.state.repeatPrescriptionCourses.repeatPrescriptionCourses
        .filter(c => c.selected === true)
        .map(item => item.id);
      this.$store.dispatch('repeatPrescriptionCourses/updateSelected', storeSelected);
      this.$emit('input', storeSelected);
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
