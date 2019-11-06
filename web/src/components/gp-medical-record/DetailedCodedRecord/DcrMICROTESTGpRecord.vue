<template>
  <div>
    <menu-item id="immunisations"
               data-purpose="immunisations"
               :href="immunisationsPath"
               :text="$t('my_record.immunisations.sectionHeader')"
               :click-func="goToUrl"
               :click-param="immunisationsPath" />

    <menu-item id="healthConditions"
               data-purpose="healthConditions"
               :href="healthConditionsPath"
               :text="$t('my_record.healthConditions.sectionHeader')"
               :click-func="goToUrl"
               :click-param="healthConditionsPath" />
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { IMMUNISATIONS, HEALTH_CONDITIONS } from '@/lib/routes';

export default {
  name: 'DcrMICROTESTGpRecord',
  components: {
    MenuItem,
  },
  computed: {
    immunisationsPath() {
      return IMMUNISATIONS.path;
    },
    healthConditionsPath() {
      return HEALTH_CONDITIONS.path;
    },
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
