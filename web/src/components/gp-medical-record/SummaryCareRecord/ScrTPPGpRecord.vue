<template>
  <menu-item id="allergies-and-reactions"
             data-purpose="allergies-and-reactions"
             :href="allergiesAndReactionsPath"
             :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
             :click-func="goToUrl"
             :click-param="'/gp-medical-record/allergies-and-reactions'"/>

  <!-- put the rest in here -->
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { ALLERGIESANDREACTIONS } from '@/lib/routes';

const ALLERGIESANDADVERSEREACTIONS = 'allergiesandadversereactions';

export default {
  name: 'ScrTPPGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      ALLERGIESANDADVERSEREACTIONS,
    };
  },
  computed: {
    allergiesAndReactionsPath() {
      return ALLERGIESANDREACTIONS.path;
    },
  },
};
</script>


<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
