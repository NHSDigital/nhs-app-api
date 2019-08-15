<template xmlns:v-if="http://www.w3.org/1999/xhtml">
  <div v-if="showTemplate"
       :class="['pull-content', !$store.state.device.isNativeApp && $style.desktopWeb]" >
    <div :class="$style.info">
      <p v-for="(paragraph, index) of $t('notifications.paragraphs')" :key="index">
        {{ paragraph }}
      </p>
    </div>
    <labelled-toggle
      v-model="registered"
      checkbox-id="allow_notifications"
      :is-waiting="isWaiting"
      :label="$t('notifications.toggleLabel')"
    />
    <div>
      <p>
        <a href="#" :class="$style.iconLink">
          <external-link-arrow-right-icon />
          <span>{{ $t('notifications.settingsLinkText') }}</span>
        </a>
      </p>
    </div>
    <back-button />
  </div>
</template>

<script>
import BackButton from '@/components/BackButton';
import ExternalLinkArrowRightIcon from '@/components/icons/ExternalLinkArrowRightIcon';
import LabelledToggle from '@/components/widgets/LabelledToggle';

export default {
  components: {
    BackButton,
    ExternalLinkArrowRightIcon,
    LabelledToggle,
  },
  computed: {
    isWaiting() {
      return this.$store.state.notifications.isWaiting;
    },
    registered: {
      get() {
        return this.$store.state.notifications.registered;
      },
      set() {
        this.$store.dispatch('spinner/prevent', true);
        this.$store.dispatch('notifications/toggle');
      },
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../style/info";

.iconLink {
  display: flex;
  align-items: center;
  margin: 0 0 1em;
  svg {
    min-width: 2em;
    height: 2em;
    margin-right: 0.25em;
    align-self: flex-start;
  }
  span {
    line-height: 1.2em;
  }
}

.info {
  font-size: 1em;
  margin-bottom: 1em;
  padding-top: 1em;
  max-width: 540px;

  p {
    font-family: $default-web;
    font-weight: lighter;
    max-width: 540px;
  }

  strong {
    font-family: $default-web;
    font-weight: normal;
  }
}
</style>
