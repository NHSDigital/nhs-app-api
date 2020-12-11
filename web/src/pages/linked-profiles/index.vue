<template>
  <div v-if="showTemplate">

    <no-linked-profiles v-if="!hasLinkedProfiles && !error" id="no-linked-profiles"/>

    <linked-profile-errors v-else-if="error"/>

    <div v-else class="nhsuk-grid-row">
      <div class="nhsuk-grid-column-full">
        <p class="nhsuk-u-margin-bottom-3">
          {{ $t('profiles.youCanAccessForTheFollowingPeople') }}
        </p>
        <menu-item-list>
          <menu-item
            v-for="(item, index) in linkedAccounts"
            :id="`linked-account-menu-item-${index}`"
            :key="index"
            :text="item.fullName"
            :click-func="onLinkedProfileClicked"
            :click-param="item.id"
            :description="getDisplayedAgeText(item)"
            description-data-sid="age-months"
            :description-id="`linked-account-age-${index}`"
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
import { LINKED_PROFILES_SUMMARY_PATH, INDEX_PATH } from '@/router/paths';
import { redirectTo } from '@/lib/utils';
import find from 'lodash/fp/find';
import NoLinkedProfiles from '@/components/linked-profiles/NoLinkedProfiles';
import LinkedProfileErrors from '@/components/linked-profiles/LinkedProfileErrors';

export default {
  components: {
    MenuItemList,
    MenuItem,
    NoLinkedProfiles,
    LinkedProfileErrors,
  },
  mixins: [CalculateAgeInMonthsAndYears],
  computed: {
    linkedAccounts() {
      return this.$store.state.linkedAccounts.items;
    },
    linkedProfileSummaryPath() {
      return LINKED_PROFILES_SUMMARY_PATH;
    },
    hasLinkedProfiles() {
      return this.$store.getters['linkedAccounts/hasLinkedAccounts'];
    },
    error() {
      return this.$store.state.linkedAccounts.error;
    },
  },
  watch: {
    '$route.query.ts': async function watchTimestamp() {
      await this.$store.dispatch('linkedAccounts/initialiseConfig');
    },
  },
  async created() {
    if (!this.$store.state.serviceJourneyRules.rules.supportsLinkedProfiles) {
      redirectTo(this, INDEX_PATH);
    }

    if (this.$route.query.hr) {
      this.$store.dispatch('session/setRetry', true);
    }

    await this.$store.dispatch('linkedAccounts/initialiseConfig');
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
