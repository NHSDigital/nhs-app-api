<template>
  <div>
    <menu-item id="allergies-and-reactions"
               data-purpose="allergies-and-reactions"
               header-tag="h2"
               :href="allergiesAndReactionsPath"
               :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.allergiesAndAdverseReactions.sectionHeader'),
                              record.allergies.data.length)"
               :click-func="goToUrl"
               :click-param="'/gp-medical-record/allergies-and-reactions'"
               :count="record.allergies.data.length"/>

    <menu-item id="medicines"
               data-purpose="medicines"
               header-tag="h2"
               :href="medicinesPath"
               :text="$t('my_record.medicines.sectionHeader')"
               :aria-label="
                 getAriaLabel($t('my_record.medicines.sectionHeader'),
                              medicinesCount)"
               :click-func="goToUrl"
               :click-param="medicinesPath"
               :count="medicinesCount"/>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { ALLERGIESANDREACTIONS, MEDICINES } from '@/lib/routes';

export default {
  name: 'ScrVISIONGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      allergiesAndReactionsPath: ALLERGIESANDREACTIONS.path,
      medicinesPath: MEDICINES.path,
    };
  },
  computed: {
    medicinesCount() {
      return this.record.medications.data.acuteMedications.length +
             this.record.medications.data.currentRepeatMedications.length;
    },
  },
  methods: {
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>
