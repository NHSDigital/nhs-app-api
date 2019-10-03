<template>
  <div>
    <menu-item id="immunisations"
               data-purpose="immunisations"
               :href="immunisationsPath"
               :text="$t('my_record.immunisations.sectionHeader')"
               :click-func="goToUrl"
               :click-param="immunisationsPath" />

  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { IMMUNISATIONS } from '@/lib/routes';

export default {
  name: 'DcrMICROTESTGpRecord',
  components: {
    MenuItem,
  },
  computed: {
    immunisationsPath() {
      return IMMUNISATIONS.path;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
