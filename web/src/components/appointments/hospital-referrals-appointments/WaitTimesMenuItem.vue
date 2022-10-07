<template>
  <div v-if="isWaitTimesEnabled">
    <h2 id="wait-times-header">
      {{ $t('wayfinder.waitTimes.title') }}
    </h2>
    <div>
      <menu-item-list class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
        <menu-item id="btn_wait_times"
                   header-tag="h2"
                   data-purpose="text_link"
                   :href="wayfinderWaitingListsPath"
                   :click-func="redirectToWaitingLists"
                   :text="$t('wayfinder.waitTimes.title')"
                   :description="$t('wayfinder.waitTimes.description')"/>
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
    redirectToWaitingLists() {
      redirectTo(this, this.wayfinderWaitingListsPath);
    },
  },
};
</script>
