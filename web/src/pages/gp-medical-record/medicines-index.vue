<template>
  <div>
    <div class="nhsuk-u-margin-bottom-4">
      <menu-item-list>
        <menu-item id="acute-medicines"
                   data-purpose="acute-medicines"
                   :href="acuteMedicinesPath"
                   :text="$t('my_record.medicines.acuteMedicines.sectionHeader')"
                   :click-func="goToUrl"
                   :click-param="acuteMedicinesPath"/>

        <menu-item id="current-medicines"
                   data-purpose="current-medicines"
                   :href="currentMedicinesPath"
                   :text="$t('my_record.medicines.currentMedicines.sectionHeader')"
                   :click-func="goToUrl"
                   :click-param="currentMedicinesPath"/>

        <menu-item id="discontinued-medicines"
                   data-purpose="discontinued-medicines"
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
      @clickAndPrevent="backButtonClicked"
    />
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import DesktopGenericBackLink from '@/components/widgets/DesktopGenericBackLink';
import { ACUTE_MEDICINES, CURRENT_MEDICINES, DISCONTINUED_MEDICINES, MYRECORD } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import MenuItemList from '@/components/MenuItemList';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItem,
    DesktopGenericBackLink,
    MenuItemList,
  },
  data() {
    return {
      backPath: MYRECORD.path,
      acuteMedicinesPath: ACUTE_MEDICINES.path,
      currentMedicinesPath: CURRENT_MEDICINES.path,
      discontinuedMedicinesPath: DISCONTINUED_MEDICINES.path,
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
  },
};
</script>
