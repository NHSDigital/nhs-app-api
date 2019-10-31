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
            :text="item.name"
            :click-func="onLinkedProfileClicked"
            :click-param="item.id"
            data-sid="linked-account">
            <div :class="[$style.dateOfBirth, 'nhsuk-u-margin-top-2']">
              {{ $t('linkedProfiles.informationHeaders.dob') }}
            </div>
            <div :id="'linked-account-dob-' + index"
                 class="nhsuk-u-margin-top-1"
                 data-sid="date-of-birth">
              {{ item.dateOfBirth | longDate }}
            </div>
          </menu-item>
        </menu-item-list>
      </div>
    </div>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import { LINKED_PROFILES_SUMMARY } from '@/lib/routes';
import { redirectTo } from '@/lib/utils';
import { find } from 'lodash/fp';

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItemList,
    MenuItem,
  },
  computed: {
    linkedAccounts() {
      return this.$store.state.linkedAccounts.items;
    },
    linkedProfileSummaryPath() {
      return LINKED_PROFILES_SUMMARY.path;
    },
  },
  asyncData({ store }) {
    store.dispatch('linkedAccounts/clear');
    return store.dispatch('linkedAccounts/load');
  },
  beforeDestroy() {
    this.$store.dispatch('linkedAccounts/clearLinkedAccounts');
  },
  methods: {
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
  font-weight: 400;
  font-size: 15px;
}
</style>
