<template>
  <div v-if="showTemplate" :class="[$style['no-padding'], 'pull-content']">
    <h2>{{ $t('myAccount.aboutUsHeading') }}</h2>
    <ul :class="[$style['list-menu'], $style.myAccountList]">
      <li v-for="(link, index) in links" :key="index" :class="$style.listMenuItem">
        <analytics-tracked-tag :href="link.url"
                               :text="$t(link.localeLabel)"
                               tag="a" target="_blank">
          {{ $t(link.localeLabel) }}
        </analytics-tracked-tag>
      </li>
    </ul>
  </div>
</template>

<script>
/* eslint-disable import/extensions */
import { accountLinks } from '@/lib/common-links';
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'AboutUs',
  components: {
    AnalyticsTrackedTag,
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
  @import "../../style/listmenu";
  @import "../../style/colours";
  @import "../../style/webshared";
  @import "../../style/nhsukoverrides";

.myAccountList {
  @include inner-container-width;

  .listMenuItem {
    font-family: $default-web;
    font-weight: lighter;

    a {
      @extend .focusBorder;
      &:hover {
        color: #000;
      }
    }
  }
}
.no-padding {
  margin-top: -0.5em;
  margin-left: -1em;
  margin-right: -1em;
  padding-bottom: 1em;

  p,
  h2 {
    margin-left: 0.7em;
    margin-top: 0.5em;
  }
}
</style>
