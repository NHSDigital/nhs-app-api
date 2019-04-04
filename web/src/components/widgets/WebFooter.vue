<template>
  <footer :class="$style.footer" role="contentinfo">
    <div :class="$style['footer-content']">
      <h2 :class="$style['visually-hidden']">
        {{ $t('webFooter.hiddenHeaderText') }}
      </h2>
      <ul>
        <li v-for="(link, index) in links" :key="index">
          <analytics-tracked-tag :href="link.url"
                                 :text="$t(link.localeLabel)"
                                 tag="a" target="_blank">
            {{ $t(link.localeLabel) }}
          </analytics-tracked-tag>
        </li>
      </ul>

      <p :class="$style['footer-copyright']">
        {{ '&copy; '+ $t('webFooter.copyrightText') }}
      </p>
    </div>
  </footer>
</template>

<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';
import { footerLinks } from '@/lib/common-links';

export default {
  name: 'WebFooter',
  components: {
    AnalyticsTrackedTag,
  },
  data() {
    return {
      links: footerLinks(this.$env),
    };
  },

};
</script>

<style module lang="scss" scoped>
@import "../../style/footer";

</style>
