<template>
  <div v-if="showTemplate">
    <div class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-u-margin-bottom-3">
          {{ $t('linkedProfiles.linkedInformation') }}
        </p>
        <menu-item-list>
          <menu-item
            v-for="(item, index) in linkedAccounts"
            :id="'linked-account-menu-item-' + index"
            :key="index"
            :text="item.fullName"
            :click-func="onLinkedProfileClicked"
            :click-param="item.id"
            :description="getDisplayedAgeText(item)"
            description-data-sid="age-months"
            :description-id="'linked-account-age-' + index"
            header-tag="h2"
            href="#"
            :aria-label="ariaLabelCaption(
              item.fullName,
              getDisplayedAgeText(item))"
            data-sid="linked-account"/>
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import CalculateAgeInMonthsAndYears from '@/plugins/mixinDefinitions/CalculateAgeInMonthsAndYears';
import { LINKED_PROFILES_SUMMARY } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import find from 'lodash/fp/find';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItemList,
    MenuItem,
  },
  mixins: [CalculateAgeInMonthsAndYears],
  data() {
    return {
      linkedAccounts: this.$store.state.linkedAccounts.items,
    };
  },
  computed: {
    linkedProfileSummaryPath() {
      return LINKED_PROFILES_SUMMARY.path;
    },
  },
  methods: {
    ariaLabelCaption(fullName, age) {
      return `${fullName}. ${age}`;
    },
    onLinkedProfileClicked(id) {
      const selectedLinkedAccount = find(item => item.id === id)(this.linkedAccounts);
      this.$store.dispatch('linkedAccounts/select', selectedLinkedAccount);

      this.$store.app.$analytics.trackButtonClick(this.linkedProfileSummaryPath, true);
      redirectTo(this, this.linkedProfileSummaryPath);
    },
  },
};
</script>
<style module lang="scss" scoped>
@import '../../style/colours';

.dateOfBirth {
  color: black;
}
</style>
