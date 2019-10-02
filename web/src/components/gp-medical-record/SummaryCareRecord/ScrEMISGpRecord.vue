<template>
  <menu-item id="allergies-and-reactions"
             data-purpose="allergies-and-reactions"
             :href="allergiesAndReactionsPath"
             :text="$t('my_record.allergiesAndAdverseReactions.sectionHeader')"
             :click-func="goToUrl"
             :click-param="allergiesAndReactionsPath"/>

  <!-- put the rest in here -->
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { ALLERGIESANDREACTIONS } from '@/lib/routes';

const ALLERGIESANDADVERSEREACTIONS = 'allergiesandadversereactions';

export default {
  name: 'ScrEMISGpRecord',
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
