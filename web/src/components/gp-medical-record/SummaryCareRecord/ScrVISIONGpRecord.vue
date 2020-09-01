<template>
  <div>
    <menu-item id="allergies-and-reactions"
               data-purpose="allergies-and-reactions"
               header-tag="h2"
               :href="allergiesAndReactionsPath"
               :text="$t('myRecord.summaryCareRecord.allergiesAndAdverseReactions')"
               :aria-label="
                 getAriaLabel($t('myRecord.summaryCareRecord.allergiesAndAdverseReactions'),
                              record.allergies.data.length)"
               :click-func="goToUrl"
               :click-param="allergiesAndReactionsPath"
               :count="record.allergies.data.length"/>

    <menu-item id="medicines"
               data-purpose="medicines"
               header-tag="h2"
               :href="medicinesPath"
               :text="$t('myRecord.summaryCareRecord.medicines')"
               :aria-label="
                 getAriaLabel($t('myRecord.summaryCareRecord.medicines'),
                              medicinesCount)"
               :click-func="goToUrl"
               :click-param="medicinesPath"
               :count="medicinesCount"/>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import {
  ALLERGIESANDREACTIONS_PATH,
  MEDICINES_PATH,
} from '@/router/paths';

export default {
  name: 'ScrVISIONGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      record: this.$store.state.myRecord.record,
      allergiesAndReactionsPath: ALLERGIESANDREACTIONS_PATH,
      medicinesPath: MEDICINES_PATH,
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
