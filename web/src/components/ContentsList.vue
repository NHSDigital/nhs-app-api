<template>
  <nav class="nhsuk-contents-list"
       role="navigation"
       :aria-label="$t('components.contentsList.description')">
    <h2 class="nhsuk-u-visually-hidden">{{ $t('components.contentsList.header') }}</h2>
    <ol class="nhsuk-contents-list__list">
      <slot/>
    </ol>
  </nav>
</template>

<script>
export default {
  name: 'ContentsList',
};
</script>
