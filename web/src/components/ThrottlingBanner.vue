<template>
  <div :class="$style.banner">
    <status-label :class="$style.statusLabel" :label-text="$t('th02.banner.labelText')"/>
    <p :class="$style.statusMessage">{{ $t('th02.banner.message') }}</p>
  </div>
</template>

<script>
import StatusLabel from '@/components/widgets/StatusLabel';

export default {
  components: {
    StatusLabel,
  },
};
</script>

<style module lang="scss" scoped>
@import '../style/messages';
@import '../style/throttling/throttlingbanner';
</style>
