<template>
  <footer role="contentinfo">
    <div id="nhsuk-footer" class="nhsuk-footer">
      <div class="nhsuk-width-container">
        <h2 class="nhsuk-u-visually-hidden">
          {{ $t('navigation.footer.supportLinks') }}
        </h2>
        <ul class="nhsuk-footer__list">
          <li v-for="(link, index) in links" :key="index" class="nhsuk-footer__list-item">
            <a class="nhsuk-footer__list-item-link" :href="link.url"
               target="_blank" rel="noopener noreferrer">
              {{ $t(link.localeLabel) }}
            </a>
          </li>
        </ul>
        <p class="nhsuk-footer__copyright">
          {{ '&copy; '+ $t('navigation.footer.crownCopyright') }}
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
      links: footerLinks(this.$store.$env),
    };
  },

};
</script>

<style lang="scss" scoped>
  @import "@/style/custom/web-footer";
</style>
