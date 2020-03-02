<template>
  <footer role="contentinfo">
    <div id="nhsuk-footer" class="nhsuk-footer">
      <div class="nhsuk-width-container">
        <h2 class="nhsuk-u-visually-hidden">
          {{ $t('webFooter.hiddenHeaderText') }}
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

<style lang="scss" scoped>
  @import '~nhsuk-frontend/packages/core/all.scss';
  @import '~nhsuk-frontend/packages/components/footer/_footer.scss';
  @import "../../style/desktopWeb/accessibility";

  .nhsuk-footer li {
    width: fit-content;
  }
  .nhsuk-footer ul {
    width: fit-content;
  }

  a {
    display: table;

    &:focus {
      @include linkFocusStyle;
    }

    &:hover {
      @include linkHoverStyle;
    }
  }
</style>
