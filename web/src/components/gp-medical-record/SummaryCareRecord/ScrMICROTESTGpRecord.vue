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

export default {
  name: 'ScrMICROTESTGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      ALLERGIESANDADVERSEREACTIONS,
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
