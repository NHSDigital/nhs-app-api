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
          :is-selected="value.includes(repeatPrescription.id)"
          :required="false"
          name="nojs.repeatPrescriptionCourses.selectedCoursesNoJs"
          @input="selectedValueChanged(repeatPrescription)">
          <span data-label="prescription-name"
                :aria-label="`${repeatPrescription.name}`"
                role="text">
            {{ repeatPrescription.name }}
          </span>
          <span :class="$style.prescriptionDescription"
                data-label="prescription-description">
            {{ repeatPrescription.details }}
          </span>
        </generic-checkbox>
      </div>
    </div>
  </div>
</template>
<script>
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
    repeatPrescriptionCourses() {
      const { repeatPrescriptionCourses } = this.$store.state.repeatPrescriptionCourses;
      return (repeatPrescriptionCourses || []).length ? repeatPrescriptionCourses : null;
    },
  },
  methods: {
    selectedValueChanged(checkbox) {
      this.$store.dispatch('repeatPrescriptionCourses/select', checkbox.id);
      const storeSelected = this.$store.getters['repeatPrescriptionCourses.selectedIds'];
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
    display: block;
  }

  label {
    &>span {
      font-weight: normal;
      font-size: 1em;
    }
    padding-bottom: 0.250em;
  }
</style>
