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

    <menu-item id="recalls"
               data-purpose="recalls"
               :href="recallsPath"
               :text="$t('my_record.recalls.sectionHeader')"
               :click-func="goToUrl"
               :click-param="recallsPath" />

    <menu-item id="referrals"
               data-purpose="referrals"
               :href="referralsPath"
               :text="$t('my_record.referrals.sectionHeader')"
               :click-func="goToUrl"
               :click-param="referralsPath" />

    <menu-item id="encounters"
               data-purpose="encounters"
               :href="encountersPath"
               :text="$t('my_record.encounters.sectionHeader')"
               :click-func="goToUrl"
               :click-param="encountersPath" />

    <menu-item id="medicalHistory"
               data-purpose="medicalHistory"
               :href="medicalHistoryPath"
               :text="$t('my_record.medicalHistory.sectionHeader')"
               :click-func="goToUrl"
               :click-param="medicalHistoryPath" />
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import { IMMUNISATIONS, HEALTH_CONDITIONS, RECALLS, REFERRALS, ENCOUNTERS, MEDICAL_HISTORY } from '@/lib/routes';

export default {
  name: 'DcrMICROTESTGpRecord',
  components: {
    MenuItem,
  },
  data() {
    return {
      immunisationsPath: IMMUNISATIONS.path,
      healthConditionsPath: HEALTH_CONDITIONS.path,
      recallsPath: RECALLS.path,
      referralsPath: REFERRALS.path,
      encountersPath: ENCOUNTERS.path,
      medicalHistoryPath: MEDICAL_HISTORY.path,
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import '../../../style/medrecordtitle';
  @import '../../../style/desktopWeb/accessibility';
</style>
