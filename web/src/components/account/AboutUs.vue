<template>
  <div v-if="showTemplate">
    <h2 class="nhsuk-u-margin-bottom-0">{{ $t('myAccount.aboutUsHeading') }}</h2>
    <menu-item-list id="about-us-links">
      <menu-item v-for="(link, index) in links"
                 :id="'account-menu-item' + index"
                 :key="index"
                 :header-tag="headerTag"
                 :target="'_blank'"
                 :href="link.url"
                 :text="$t(link.localeLabel)"
                 :aria-label="$t(link.localeLabel)"/>
    </menu-item-list>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { accountLinks } from '@/lib/common-links';
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';

export default {
  name: 'AboutUs',
  components: {
    MenuItem,
    MenuItemList,
  },
  props: {
    headerTag: {
      type: String,
      default: 'h2',
    },
  },
  data() {
    return {
      links: accountLinks(this.$store.$env),
    };
  },
};
</script>
