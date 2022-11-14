<template>
  <section :class="[$style['custom-header-picture'], 'nhsuk-hero nhsuk-hero--image', 'nhsuk-hero--image-description']" >
    <div :class="['nhsuk-hero__overlay', $style['custom-hero-overlay']]">
      <div class="nhsuk-width-container">
        <div class="nhsuk-grid-row">
          <div class="nhsuk-grid-column-two-thirds">
            <div class="nhsuk-hero-content">
              <h1 ref="heroHeader" tabindex="-1" class="nhsuk-u-margin-bottom-4 nhsuk-heading-l">
                {{ $t('home.pictureBannerHeader') }}
              </h1>
              <dl class="nhsuk-summary-list nhsuk-summary-list--no-border
                nhs-app-summary-list-inline nhsuk-u-margin-bottom-0">
                <div class="nhsuk-summary-list__row">
                  <div class="nhsuk-summary-list__value" data-sid="hero-user-name" data-hj-suppress >
                    {{ displayName }}
                  </div>
                </div>
                <div v-if="nhsNumber" class="nhsuk-summary-list__row">
                  {{ $t('home.nhsNumber') }}:
                  <generic-voice-over-text-split :text="nhsNumber"
                                                 data-sid="hero-user-nhs-number" />
                </div>
                <div v-if="age" class="nhsuk-summary-list__row">
                  <div class="nhsuk-summary-list__value" data-sid="hero-user-age" >
                    {{ age }}
                  </div>
                </div>
                <div v-if="practice" class="nhsuk-summary-list__row" data-sid="hero-user-practice" >
                  <div class="nhsuk-summary-list__value">
                    {{ practice }}
                  </div>
                </div>
              </dl>
              <span class="nhsuk-hero__arrow" aria-hidden="true" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>
<script>
import GenericVoiceOverTextSplit from './widgets/GenericVoiceOverTextSplit';

export default {
  name: 'PictureBanner',
  components: { GenericVoiceOverTextSplit },
  props: {
    displayName: {
      type: String,
      required: true,
    },
    nhsNumber: {
      type: String,
      default: '',
    },
    age: {
      type: String,
      default: '',
    },
    practice: {
      type: String,
      default: '',
    },
  },
  updated() {
    this.focusHeroHeader();
  },
  mounted() {
    this.focusHeroHeader();
  },
  methods: {
    focusHeroHeader() {
      if (document.activeElement !== null) {
        document.activeElement.blur();
      }
      if (this.$refs.heroHeader) {
        this.$refs.heroHeader.focus();
      }
    },
  },
};
</script>
<style lang="scss" module scoped>
@import "@/style/custom/picture-banner";
</style>
