<template>
  <footer role="contentinfo">
    <div :class="$style['nhsuk-footer']">
      <div :class="$style['nhsuk-width-container']">
        <h2 :class="$style['nhsuk-u-visually-hidden']">
          {{ $t('webFooter.hiddenHeaderText') }}
        </h2>
        <ul v-for="(link, index) in links" :key="index" :class="$style['nhsuk-footer__list']">
          <li :class="$style['nhsuk-footer__list-item']">
            <a :class="$style['nhsuk-footer__list-item-link']" :href="link.url" target="_blank">
              {{ $t(link.localeLabel) }}
            </a>
          </li>
        </ul>
        <p :class="$style['nhsuk-footer__copyright']">
          {{ '&copy; '+ $t('webFooter.copyrightText') }}
        </p>
      </div>
    </div>
  </footer>
</template>

<script>
import { footerLinks } from '@/lib/common-links';

export default {
  name: 'WebFooter',
  data() {
    return {
      links: footerLinks(this.$env),
    };
  },

};
</script>

<style module lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/all.scss';
  @import '~nhsuk-frontend/packages/components/footer/_footer.scss';
  .nhsuk-footer ul {
    width : fit-content;
    a {
      -moz-text-decoration-line: underline;
      text-decoration-line: underline;
    }
    a:hover {
      -moz-text-decoration-line: none;
      text-decoration-line: none;
    }
  }
</style>
