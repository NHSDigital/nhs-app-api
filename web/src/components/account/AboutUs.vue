<template>
  <div v-if="showTemplate">
    <h2>{{ $t('myAccount.aboutUsHeading') }}</h2>

    <menu-item-list>
      <menu-item v-for="(link, index) in links"
                 :id="'account-menu-item' + index"
                 :key="index"
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
  data() {
    return {
      links: accountLinks(this.$env),
    };
  },
};
</script>

<style module lang="scss" scoped>
  @import "../../style/accessibility";
  @import '../../style/colours';
  @import '../../style/textstyles';
  @import '../../style/fonts';
  @import "../../style/webshared";
</style>
