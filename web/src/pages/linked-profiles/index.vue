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
            :click-param="item.id">
            <div :class="[$style.dateOfBirth, 'nhsuk-u-margin-top-2']">
              {{ $t('linkedProfiles.dob') }}
            </div>
            <div :id="'linked-account-dob-' + index"
                 class="nhsuk-u-margin-top-1">
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

export default {
  layout: 'nhsuk-layout',
  components: {
    MenuItemList,
    MenuItem,
  },
  computed: {
    hasConnectionProblem() {
      return this.$store.state.errors.hasConnectionProblem;
    },
    showApiError() {
      return this.$store.getters['errors/showApiError'];
    },
    showLinkedAccounts() {
      return (
        this.$store.state.linkedAccounts.hasLoaded &&
        this.$store.state.linkedAccounts.items.length === 0
      );
    },
    linkedAccounts() {
      return this.$store.state.linkedAccounts.items;
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
    onLinkedProfileClicked() {
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
