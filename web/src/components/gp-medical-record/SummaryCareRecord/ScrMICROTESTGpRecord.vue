<template>
  <menu-item id="allergies-and-reactions"
             data-purpose="allergies-and-reactions"
             :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
             :click-func="goToUrl"
             :click-param="'/gp-medical-record/allergies-and-reactions'"/>

  <!-- put the rest in here -->
</template>

<script>
import MenuItem from '@/components/MenuItem';

const ALLERGIESANDADVERSEREACTIONS = 'allergiesandadversereactions';
const ACUTEMEDICATIONS = 'acutemedications';
const CURRENTREPEATMEDICATIONS = 'currentrepeatmedications';
const DISCONTINUEDREPEATMEDICATIONS = 'discontinuedrepeatmedications';

export default {
  name: 'ScrMICROTESTGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      ALLERGIESANDADVERSEREACTIONS,
      ACUTEMEDICATIONS,
      CURRENTREPEATMEDICATIONS,
      DISCONTINUEDREPEATMEDICATIONS,
      isAllergiesAndAdverseReactionsCollapsed: process.client,
      isAcuteMedicationsCollapsed: process.client,
      isCurrentRepeatMedicationsCollapsed: process.client,
      isDiscontinuedRepeatMedicationsCollapsed: process.client,
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
