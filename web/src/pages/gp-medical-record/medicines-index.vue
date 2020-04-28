<template>
  <div>
    <div class="nhsuk-u-margin-bottom-4">
      <menu-item-list>
        <menu-item id="acute-medicines"
                   header-tag="h2"
                   data-purpose="acute-medicines"
                   :href="acuteMedicinesPath"
                   :text="$t('my_record.medicines.acuteMedicines.sectionHeader')"
                   :aria-label="
                     getAriaLabel($t('my_record.medicines.acuteMedicines.sectionHeader'),
                                  acuteMedicinesCount)"
                   :click-func="goToUrl"
                   :click-param="acuteMedicinesPath"
                   :count="acuteMedicinesCount"/>

        <menu-item id="current-medicines"
                   data-purpose="current-medicines"
                   header-tag="h2"
                   :href="currentMedicinesPath"
                   :text="$t('my_record.medicines.currentMedicines.sectionHeader')"
                   :aria-label="
                     getAriaLabel($t('my_record.medicines.currentMedicines.sectionHeader'),
                                  currentMedicinesCount)"
                   :click-func="goToUrl"
                   :click-param="currentMedicinesPath"
                   :count="currentMedicinesCount"/>

        <menu-item id="discontinued-medicines"
                   data-purpose="discontinued-medicines"
                   header-tag="h2"
                   :href="discontinuedMedicinesPath"
                   :text="$t('my_record.medicines.discontinuedMedicines.sectionHeader')"
                   :click-func="goToUrl"
                   :click-param="discontinuedMedicinesPath"/>
      </menu-item-list>
    </div>

    <desktopGenericBackLink
      v-if="!$store.state.device.isNativeApp"
      :path="backPath"
      :button-text="'rp03.backButton'"
      @clickAndPrevent="backButtonClicked"/>
  </div>
</template>

<script>
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import ReloadRecordMixin from '@/components/gp-medical-record/ReloadRecordMixin';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { ACUTE_MEDICINES, CURRENT_MEDICINES, DISCONTINUED_MEDICINES, GP_MEDICAL_RECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    DesktopGenericBackLink,
    MenuItemList,
  },
  mixins: [ReloadRecordMixin],
  data() {
    return {
      backPath: GP_MEDICAL_RECORD.path,
      acuteMedicinesPath: ACUTE_MEDICINES.path,
      currentMedicinesPath: CURRENT_MEDICINES.path,
      discontinuedMedicinesPath: DISCONTINUED_MEDICINES.path,
    };
  },
  async asyncData({ store }) {
    if (!(store.state.myRecord.record || {}).medications) {
      await store.dispatch('myRecord/load');
    }
    return {
      acuteMedicinesCount:
        store.state.myRecord.record.medications.data.acuteMedications.length,
      currentMedicinesCount:
        store.state.myRecord.record.medications.data.currentRepeatMedications.length,
    };
  },
  methods: {
    backButtonClicked() {
      redirectTo(this, this.backPath);
    },
    getEffectiveDate(effectiveDate, defaultValue) {
      return effectiveDate && effectiveDate.value
        ? effectiveDate.value
        : defaultValue;
    },
    getNextDateFormatted(nextDate) {
      return nextDate.rawValue != null ?
        nextDate.rawValue : this.$options.filters.datePart(nextDate.value, nextDate.datePart);
    },
    getAriaLabel(sectionTitle, count) {
      return `${sectionTitle}, ${count} items`;
    },
  },
};
</script>
