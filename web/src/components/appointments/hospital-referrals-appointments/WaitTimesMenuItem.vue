<template>
  <div v-if="isWaitTimesEnabled">
    <h2 id="wait-times-header" class="nhsuk-u-padding-top-0 nhsuk-u-padding-bottom-0">
      {{ $t('wayfinder.waitTimes.title') }}
    </h2>
    <div>
      <menu-item-list class="nhsuk-grid-column-full nhsuk-u-padding-bottom-9 nhsuk-u-margin-bottom-0">
        <menu-item id="btn_wait_times"
                   header-tag="h2"
                   data-purpose="text_link"
                   :href="wayfinderWaitingListsPath"
                   :click-func="redirectToWaitingLists"
                   :text="headerText()"
                   :description="descriptionText()"
                   :aria-label="headerText() |
                     join(descriptionText() ,'. ')"/>
      </menu-item-list>
    </div>
  </div>
</template>

<script>

import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import {
  WAYFINDER_WAITING_LISTS_PATH,
} from '@/router/paths';
import { redirectTo } from '@/lib/utils';

export default {
  name: 'WaitTimesMenuItem',
  components: {
    MenuItem,
    MenuItemList,
  },
  props: {
    isWaitTimesEnabled: {
      type: Boolean,
      default: () => false,
    },
  },
  data() {
    return {
      wayfinderWaitingListsPath: WAYFINDER_WAITING_LISTS_PATH,
    };
  },
  methods: {
    headerText() {
      return this.$t('wayfinder.waitTimes.title');
    },
    descriptionText() {
      return this.$t('wayfinder.waitTimes.description');
    },
    redirectToWaitingLists() {
      redirectTo(this, this.wayfinderWaitingListsPath);
    },
  },
};
</script>
