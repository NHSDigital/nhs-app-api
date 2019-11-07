<template>
  <div>
    <menu-item id="allergies-and-reactions"
               data-purpose="allergies-and-reactions"
               :href="allergiesAndReactionsPath"
               :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
               :click-func="goToUrl"
               :click-param="'/gp-medical-record/allergies-and-reactions'"/>

    <menu-item id="medicines"
               data-purpose="medicines"
               :href="medicinesPath"
               :text="$t('my_record.medicines.sectionHeader')"
               :click-func="goToUrl"
               :click-param="medicinesPath"/>
  </div>
  <!-- put the rest in here -->
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { ALLERGIESANDREACTIONS, MEDICINES } from '@/lib/routes';

export default {
  name: 'ScrMICROTESTGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      allergiesAndReactionsPath: ALLERGIESANDREACTIONS.path,
      medicinesPath: MEDICINES.path,
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
